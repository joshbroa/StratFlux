using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StratFlux.Data;
using StratFlux.Models;
using StratFlux.ViewModels.BacktestingSettings;
using System.Configuration;

namespace StratFlux.Controllers
{
    // Authroize makes sure that the user is signed in before being able to do anything with this controller
    [Authorize]
    public class BacktestingSettingsController : Controller
    {
        private ApplicationDbContext _dbContext;
        private UserManager<StratUser> _userManager;

        public BacktestingSettingsController(ApplicationDbContext dbContext, UserManager<StratUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // Here, a list of the user's existing settings must be retrieved
        public async Task<IActionResult> Index()
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

                ViewBag.IdToDisplayNameMappings = idToDisplayNameMappings;
                ViewBag.OrderedIds = orderedIds;
            }


            return View("Index");
        }

        // A BacktestingSettings ViewModel is needed because the settings should not save upon a change in the browser
        // It should update when a save button is clicked
        public async Task<IActionResult> EditSettings(string? Id)
        {
            if (Id == null)
            {
                var emptySettings = new BacktestingSettingsViewModel();

                return View("Edit", emptySettings);
            }
            else
            {
                StratUser user = await _userManager.GetUserAsync(User);

                BacktestingSettings? settings;

                if (_dbContext.BacktestingSettings != null)
                {
                    settings = await _dbContext.BacktestingSettings.FirstOrDefaultAsync(settings => settings.Id == Id);
                }
                else
                {
                    return View("Index");
                }

                // Check if given settings exist
                if (settings != null)
                {
                    // Check if user has access to settings
                    if (VerifySettingsBelongToUser(user, settings))
                    {
                        // Create a new BacktestingSettingsViewModel instance which will have values mapped from the entity model
                        BacktestingSettingsViewModel settingsViewModel = new BacktestingSettingsViewModel();
                        settingsViewModel.Input = new BacktestingSettingsViewModel.InputModel();
                        MapToViewModel(settings, ref settingsViewModel);

                        // Let user edit settings
                        return View("Edit", settingsViewModel);
                    }
                }
            }

            // If we get to this point, there are no settings to edit so just return list of settings
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SaveSettings(string? Id, BacktestingSettingsViewModel newSettings)
        {
            StratUser user = await _userManager.GetUserAsync(User);

            BacktestingSettings? settings;

            if (_dbContext.BacktestingSettings != null)
            {
                settings = await _dbContext.BacktestingSettings.FirstOrDefaultAsync(settings => settings.Id == Id);
            }
            else
            {
                if (Id == null)
                {
                    return View("Edit", newSettings);
                }
                else
                {
                    return await EditSettings(Id);
                }
            }

            if (settings != null)
            {
                if (VerifySettingsBelongToUser(user, settings))
                {
                    // New settings replace old properties and object is not replaced via reference
                    MapFromViewModel(newSettings, ref settings);

                    _dbContext.BacktestingSettings.Update(settings);

                    await _dbContext.SaveChangesAsync();

                    return await EditSettings(Id);
                }
            }
            else
            {
                // If at this point, settings does not exist so it will be created in database
                settings = new BacktestingSettings();

                MapFromViewModel(newSettings, ref settings);

                StratUser currentUser = await _userManager.GetUserAsync(User);

                settings.User = currentUser;
                settings.UserId = currentUser.Id;

                await _dbContext.BacktestingSettings.AddAsync(settings);

                await _dbContext.SaveChangesAsync();

                return await EditSettings(Id);
            }

            // If this point is reached, something probably didn't go right, so normal editing view will be returned with nothing saved
            return View("Edit", newSettings);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSettings(string? Id)
        {
            StratUser user = await _userManager.GetUserAsync(User);

            if (Id == null)
            {
                return await Index();
            }
            else
            {
                BacktestingSettings? settings = null;

                // Checks settings context exist
                if (_dbContext.BacktestingSettings != null)
                {
                    settings = await _dbContext.BacktestingSettings.FirstOrDefaultAsync(settings => settings.Id == Id);

                    // Checks settings exist
                    if (settings != null)
                    {
                        // Checks the settings belong to the user which sent the request
                        if (VerifySettingsBelongToUser(user, settings))
                        {
                            // Deletes settings
                            _dbContext.BacktestingSettings.Remove(settings);
                            await _dbContext.SaveChangesAsync();

                            return await Index();
                        }
                    }
                }
            }

            return await Index();
        }

        // Returns true if the given settings belongs to the given user, otherweise, returns false
        private bool VerifySettingsBelongToUser(StratUser user, BacktestingSettings settings)
        {
            if (settings.UserId == user.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Method to convert ViewModel (for use in front end) to actual Model (for use in database)
        // Reference is used so instance of object is updated rather than overwritten
        private void MapFromViewModel(BacktestingSettingsViewModel settings, ref BacktestingSettings newSettings)
        {
            newSettings.BacktestingSettingsName = settings.Input.BacktestingSettingsName;
            newSettings.StockToTrade = settings.Input.StockToTrade;
            newSettings.TimeResolution = settings.Input.TimeResolution;

            // Ensures that the DateTime has the UTC time zone set, ready for reading/writing in the Postgres database
            newSettings.TimeFrameStart = DateTime.SpecifyKind(settings.Input.TimeFrameStart, DateTimeKind.Utc);
            newSettings.TimeFrameEnd = DateTime.SpecifyKind(settings.Input.TimeFrameEnd, DateTimeKind.Utc);
            
            newSettings.InitialCapital = settings.Input.InitialCapital;
            newSettings.OrderSize = settings.Input.OrderSize;
            newSettings.PyramidingLimit = settings.Input.PyramidingLimit;
            newSettings.CommissionFeeType = settings.Input.CommissionFeeType;
            newSettings.CommissionFee = settings.Input.CommissionFee;
            newSettings.ResetPosAtEoD = settings.Input.ResetPosAtEoD;
        }

        // Method to convert actual Model (for use in database) to ViewModel (for use in front end)
        private void MapToViewModel(BacktestingSettings settings, ref BacktestingSettingsViewModel newSettings)
        {
            newSettings.Id = settings.Id;

            newSettings.Input.BacktestingSettingsName = settings.BacktestingSettingsName;
            newSettings.Input.StockToTrade = settings.StockToTrade;
            newSettings.Input.TimeResolution = settings.TimeResolution;
            
            // Ensures that the DateTime has the UTC time zone set, ready for reading/writing in the Postgres database
            newSettings.Input.TimeFrameStart = DateTime.SpecifyKind(settings.TimeFrameStart, DateTimeKind.Utc);
            newSettings.Input.TimeFrameEnd = DateTime.SpecifyKind(settings.TimeFrameEnd, DateTimeKind.Utc);
            
            newSettings.Input.InitialCapital = settings.InitialCapital;
            newSettings.Input.OrderSize = settings.OrderSize;
            newSettings.Input.PyramidingLimit = settings.PyramidingLimit;
            newSettings.Input.CommissionFeeType = settings.CommissionFeeType;
            newSettings.Input.CommissionFee = settings.CommissionFee;
            newSettings.Input.ResetPosAtEoD = settings.ResetPosAtEoD;
        }
    }
}