using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StratFlux.Data;
using StratFlux.Models;
using StratFlux.ViewModels.Strategies;

namespace StratFlux.Controllers
{
    [Authorize]
    public class StrategiesController : Controller
    {
        private ApplicationDbContext _dbContext;
        private UserManager<StratUser> _userManager;

        public StrategiesController(ApplicationDbContext dbContext, UserManager<StratUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // Here, a list of the user's existing strategies is retrieved
        public async Task<IActionResult> Index()
        {
            StratUser user = await _userManager.GetUserAsync(User);

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

                ViewBag.IdToDisplayNameMappings = idToDisplayNameMappings;
                ViewBag.OrderedIds = orderedIds;
            }


            return View("Index");
        }

        public async Task<IActionResult> EditStrategy(string? Id)
        {
            if (Id == null)
            {
                var emptyStrategy = new StrategyViewModel();

                return View("Edit", emptyStrategy);
            }
            else
            {
                StratUser user = await _userManager.GetUserAsync(User);

                Strategy? strategy;

                if (_dbContext.Strategies != null)
                {
                    strategy = await _dbContext.Strategies.FirstOrDefaultAsync(strategy => strategy.Id == Id);
                }
                else
                {
                    return View("Index");
                }

                // Check if given strategy exist
                if (strategy != null)
                {
                    // Check if user has access to strategy
                    if (VerifyStrategyBelongsToUser(user, strategy))
                    {
                        // Create a new StrategyViewModel instance which will have values mapped from the entity model
                        StrategyViewModel strategyViewModel = new StrategyViewModel();
                        strategyViewModel.Input = new StrategyViewModel.InputModel();
                        MapToViewModel(strategy, ref strategyViewModel);

                        // Let user edit strategy
                        return View("Edit", strategyViewModel);
                    }
                }
            }

            // If we get to this point, there is no strategy to edit so just return list of strategies
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SaveStrategy(string? Id, StrategyViewModel newStrategy)
        {
            StratUser user = await _userManager.GetUserAsync(User);

            Strategy? strategy;

            if (_dbContext.Strategies != null)
            {
                strategy = await _dbContext.Strategies.FirstOrDefaultAsync(strategy => strategy.Id == Id);
            }
            else
            {
                if (Id == null)
                {
                    return View("Edit", newStrategy);
                }
                else
                {
                    return await EditStrategy(Id);
                }
            }

            if (strategy != null)
            {
                if (VerifyStrategyBelongsToUser(user, strategy))
                {
                    // New strategy's data is mapped to the current strategy and overwrites the old data
                    MapFromViewModel(newStrategy, ref strategy);

                    _dbContext.Strategies.Update(strategy);

                    await _dbContext.SaveChangesAsync();

                    return await EditStrategy(Id);
                }
            }
            else
            {
                // If this point is reached, then the strategy does not exist so it will be created in the database
                strategy = new Strategy();

                MapFromViewModel(newStrategy, ref strategy);

                StratUser currentUser = await _userManager.GetUserAsync(User);

                strategy.User = currentUser;
                strategy.UserId = currentUser.Id;

                await _dbContext.Strategies.AddAsync(strategy);

                await _dbContext.SaveChangesAsync();

                return await EditStrategy(Id);
            }

            // If this point is reached, something probably didn't go right, so normal editing view will be returned with nothing saved
            return View("Edit", newStrategy);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStrategy(string? Id)
        {
            StratUser user = await _userManager.GetUserAsync(User);

            if (Id == null)
            {
                return await Index();
            }
            else
            {
                Strategy? strategy = null;

                // Checks strategies context exist
                if (_dbContext.Strategies != null)
                {
                    strategy = await _dbContext.Strategies.FirstOrDefaultAsync(strategy => strategy.Id == Id);

                    // Checks strategy exist
                    if (strategy != null)
                    {
                        // Checks the strategy belong to the user which sent the request
                        if (VerifyStrategyBelongsToUser(user, strategy))
                        {
                            // Deletes strategy
                            _dbContext.Strategies.Remove(strategy);
                            await _dbContext.SaveChangesAsync();

                            return await Index();
                        }
                    }
                }
            }

            return await Index();
        }

        // Returns true if the given strategy belongs to the given user, otherweise, returns false
        private bool VerifyStrategyBelongsToUser(StratUser user, Strategy strategy)
        {
            if (strategy.UserId == user.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void MapFromViewModel(StrategyViewModel strategy, ref Strategy newStrategy)
        {
            newStrategy.StrategyName = strategy.Input.StrategyName;
            newStrategy.StrategyDescription = strategy.Input.StrategyDescription;
            newStrategy.StrategyJson = strategy.Input.StrategyJson;
        }

        private void MapToViewModel(Strategy strategy, ref StrategyViewModel newStrategy)
        {
            newStrategy.Id = strategy.Id;

            newStrategy.Input.StrategyName = strategy.StrategyName;
            newStrategy.Input.StrategyDescription = strategy.StrategyDescription;
            newStrategy.Input.StrategyJson = strategy.StrategyJson;
        }
    }
}
