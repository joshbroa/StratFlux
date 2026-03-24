using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StratFlux.Backtesting;
using StratFlux.Data;
using StratFlux.Models;
using StratFlux.Services;
using StratFlux.ViewModels.Backtests;
using StratFlux.ViewModels.Strategies;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StratFlux.Controllers
{
    [Authorize]
    public class BacktestsController : Controller
    {
        private ApplicationDbContext _dbContext;
        private UserManager<StratUser> _userManager;
        private BacktestsNotificationService _notificationService;
        private IServiceScopeFactory _scopeFactory;

        public BacktestsController(
            ApplicationDbContext dbContext,
            UserManager<StratUser> userManager,
            BacktestsNotificationService notificationService,
            IServiceScopeFactory scopeFactory)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _notificationService = notificationService;
            _scopeFactory = scopeFactory;
        }

        public async Task<IActionResult> Index()
        {
            StratUser user = await _userManager.GetUserAsync(User);
            GeneralResults[]? existingResults = null;

            if (_dbContext.GeneralResults != null)
            {
                existingResults = await _dbContext.GeneralResults
                    .Where(results => results.UserId == user.Id)
                    .OrderBy(results => results.ResultsName)
                    .ToArrayAsync();
            }

            if (existingResults != null)
            {
                Dictionary<string, string> idToDisplayNameMappings = new Dictionary<string, string>();
                string[] orderedIds = new string[existingResults.Length];

                for (int i = 0; i < existingResults.Length; i++)
                {
                    GeneralResults results = existingResults[i];
                    if (results.ResultsName == null)
                    {
                        idToDisplayNameMappings.Add(results.Id, "No name available");
                    }
                    else
                    {
                        idToDisplayNameMappings.Add(results.Id, results.ResultsName);
                    }
                    orderedIds[i] = results.Id;
                }

                ViewBag.IdToDisplayNameMappings = idToDisplayNameMappings;
                ViewBag.OrderedIds = orderedIds;
            }

            return View("Index");
        }

        public async Task<IActionResult> NewBacktest()
        {
            StratUser user = await _userManager.GetUserAsync(User);
            BacktestingSettings[]? existingSettings = null;

            if (_dbContext.BacktestingSettings != null)
            {
                existingSettings = await _dbContext.BacktestingSettings
                    .Where(settings => settings.UserId == user.Id)
                    .OrderBy(settings => settings.BacktestingSettingsName)
                    .ToArrayAsync();
            }

            if (existingSettings != null)
            {
                Dictionary<string, string> idToDisplayNameMappings = new Dictionary<string, string>();
                string[] orderedIds = new string[existingSettings.Length];

                for (int i = 0; i < existingSettings.Length; i++)
                {
                    BacktestingSettings settings = existingSettings[i];
                    idToDisplayNameMappings.Add(settings.Id, settings.BacktestingSettingsName);
                    orderedIds[i] = settings.Id;
                }

                ViewBag.SettingsIdToDisplayNameMappings = idToDisplayNameMappings;
                ViewBag.SettingsOrderedIds = orderedIds;
            }

            Strategy[]? existingStrategies = null;

            if (_dbContext.Strategies != null)
            {
                existingStrategies = await _dbContext.Strategies
                    .Where(strategy => strategy.UserId == user.Id)
                    .OrderBy(strategy => strategy.StrategyName)
                    .ToArrayAsync();
            }

            if (existingStrategies != null)
            {
                Dictionary<string, string> idToDisplayNameMappings = new Dictionary<string, string>();
                string[] orderedIds = new string[existingStrategies.Length];

                for (int i = 0; i < existingStrategies.Length; i++)
                {
                    Strategy strategy = existingStrategies[i];
                    idToDisplayNameMappings.Add(strategy.Id, strategy.StrategyName);
                    orderedIds[i] = strategy.Id;
                }

                ViewBag.StrategiesIdToDisplayNameMappings = idToDisplayNameMappings;
                ViewBag.StrategiesOrderedIds = orderedIds;
            }

            NewBacktestViewModel newBacktest = new NewBacktestViewModel();
            return View("NewBacktest", newBacktest);
        }

        public async Task<IActionResult> ViewResult(string? Id)
        {
            if (Id == null)
            {
                // If the id is null, there is no result to view so just return user to the list of results
                return await Index();
            }
            else
            {
                StratUser user = await _userManager.GetUserAsync(User);
                GeneralResults? result;

                if (_dbContext.GeneralResults != null)
                {
                    result = await _dbContext.GeneralResults.FirstOrDefaultAsync(result => result.Id == Id);
                }
                else
                {
                    return await Index();
                }

                // Check if given result exists
                if (result != null)
                {
                    // Check if user has access to the result
                    if (VerifyResultBelongsToUser(user, result))
                    {
                        // Create a new GeneralResultsViewModel instance which will have values mapped from the entity model
                        GeneralResultsViewModel resultViewModel = new GeneralResultsViewModel();
                        MapToViewModel(result, ref resultViewModel);

                        // Let user view the result
                        return View("ViewResult", resultViewModel);
                    }
                }
            }

            // If we get to this point, there is no strategy to edit so just return list of strategies
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteResult(string? Id)
        {
            StratUser user = await _userManager.GetUserAsync(User);
            if (Id == null)
            {
                return await Index();
            }
            else
            {
                GeneralResults? result = null;

                // Checks general results context exist
                if (_dbContext.GeneralResults != null)
                {
                    result = await _dbContext.GeneralResults.FirstOrDefaultAsync(result => result.Id == Id);

                    // Checks result exist
                    if (result != null)
                    {
                        // Checks the result belongs to the user which sent the request
                        if (VerifyResultBelongsToUser(user, result))
                        {
                            // Deletes result
                            _dbContext.GeneralResults.Remove(result);
                            await _dbContext.SaveChangesAsync();
                            return await Index();
                        }
                    }
                }
            }
            return await Index();
        }

        public async Task<IActionResult> RunNewBacktest(NewBacktestViewModel newBacktest)
        {
            StratUser user = await _userManager.GetUserAsync(User);
            string userId = user.Id;

            // Asynchronous task started so that backtest can run and user can be immediately redirected to loading page
#pragma warning disable
            Task.Run(async () =>
            {
                bool successful;
                string errorOrResultsId;

                // New scope is created so that the backtesting engine can get it's own db context and user manager which has the same life time as the engine's
                using (IServiceScope scope = _scopeFactory.CreateScope())
                {
                    // This gets the new scoped db context and user manager
                    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    UserManager<StratUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<StratUser>>();

                    // Technically, the secrets service could be injected into the controller as it is a singleton and life time isn't a factor
                    // But this way is more secure and consistent which is better considering it is used to retrieve keys
                    AlpacaDataService secretsService = scope.ServiceProvider.GetRequiredService<AlpacaDataService>();

                    // The backtesting engine is instantiated with the database context, user manager, user id, strategy id and backtesting settings id as parameters
                    BacktestingEngine backtestingEngine = new BacktestingEngine(dbContext, userManager, secretsService, userId, newBacktest.StrategyId, newBacktest.BacktestingSettingsId);

                    (successful, errorOrResultsId) = await backtestingEngine.RunBacktest(newBacktest.Input.ResultsName);
                }

                if (successful)
                {
                    // If successful, then backtest is complete and General Results Id will be sent to user
                    await _notificationService.NotifyBacktestComplete(userId, errorOrResultsId);
                }
                else
                {
                    // If unsuccessful, then backtest did not complete and error message will be sent to user
                    await _notificationService.NotifyBacktestFailed(userId, errorOrResultsId);
                }
            });
#pragma warning restore

            return View("Loading");
        }

        // This function returns whether or not the given result belongs to the given user
        private bool VerifyResultBelongsToUser(StratUser user, GeneralResults result)
        {
            if (result.UserId == user.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // This function maps a referenced results view model's properties to a given result. This allows it to be used in the view.
        private void MapToViewModel(GeneralResults result, ref GeneralResultsViewModel resultViewModel)
        {
            resultViewModel.Id = result.Id;
            resultViewModel.ResultsName = result.ResultsName;
            resultViewModel.UnrealisedReturnLoss = result.UnrealisedReturnLoss;
            resultViewModel.NetReturnLoss = result.NetReturnLoss;
            resultViewModel.AverageReturnLoss = result.AverageReturnLoss;
            resultViewModel.AverageHoldingPeriod = result.AverageHoldingPeriod;
            resultViewModel.StandardDeviationOverTime = result.StandardDeviationOverTime;
            resultViewModel.InitialEquity = result.InitialEquity;
            resultViewModel.FinalEquity = result.FinalEquity;
            resultViewModel.MaxDrawDown = result.MaxDrawDown;
            resultViewModel.TotalCommissionAmount = result.TotalCommissionAmount;
            resultViewModel.TotalClosedTrades = result.TotalClosedTrades;
            resultViewModel.WinningTrades = result.WinningTrades;
            resultViewModel.LosingTrades = result.LosingTrades;
            resultViewModel.TimeResolution = result.TimeResolution;
            resultViewModel.TimeFrameStart = result.TimeFrameStart;
            resultViewModel.TimeFrameEnd = result.TimeFrameEnd;
            resultViewModel.StockTraded = result.StockTraded;
        }
    }
}