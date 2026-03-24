using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace StratFlux.ViewModels.Backtests
{
    public class NewBacktestViewModel
    {
#nullable disable
        public class InputModel
        {
            [Required, DisplayName("Result Name")]
            [MaxLength(20, ErrorMessage = "Must be within 20 characters.")]
            [RegularExpression(@"^[_A-Za-z0-9]+$", ErrorMessage = "Must only contain letters and/or numbers.")]
            public string ResultsName { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [Required(ErrorMessage = "You must choose backtesting settings in order to run the backtest.")]
        public string BacktestingSettingsId { get; set; }

        [Required(ErrorMessage = "You must choose a strategy in order to run the backtest.")]
        public string StrategyId { get; set; }
#nullable enable
    }
}
