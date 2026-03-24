using StratFlux.ModelEnums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace StratFlux.ViewModels.BacktestingSettings
{
    public class BacktestingSettingsViewModel
    {
#nullable disable
        public class InputModel
        {
            [Required, DisplayName("Backtesting Settings Name")]
            [MaxLength(20, ErrorMessage = "Must be within 20 characters.")]
            [RegularExpression(@"^[_A-Za-z0-9]+$", ErrorMessage = "Must only contain letters and/or numbers.")]
            public string BacktestingSettingsName { get; set; }

            [Required, DisplayName("Stock To Trade")]
            [MaxLength(5, ErrorMessage = "Must be 5 characters or less.")]
            [RegularExpression(@"^[A-Z]+$", ErrorMessage = "Must be all capital letters and no other characters.")]
            public string StockToTrade { get; set; }


            [Required, DisplayName("Time Resolution")]
            public TimeResolution TimeResolution { get; set; }

            [Required, DisplayName("Time Frame (Start)")]
            public DateTime TimeFrameStart { get; set; }

            [Required, DisplayName("Time Frame (End)")]
            public DateTime TimeFrameEnd { get; set; }

            [Required, DisplayName("Initial Capital")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Must be greater than 0 and within a reasonable range.")]
            [RegularExpression(@"^\d+(\.\d)?\d?$", ErrorMessage = "Must be a number within 2 decimal places.")]
            public double InitialCapital { get; set; }


            [Required, DisplayName("Order Size")]
            [Range(0, uint.MaxValue, ErrorMessage = "Must be greater than 0 and within a reasonable range.")]
            public uint OrderSize { get; set; }

#nullable enable
            [DisplayName("Pyramiding Limit")]
            [Range(0, uint.MaxValue, ErrorMessage = "Must be greater than 0 and within a reasonable range.")]
            public uint? PyramidingLimit { get; set; }
#nullable disable

            [Required, DisplayName("Commission Fee Type (Percentage or Absolute)")]
            public CommissionFeeType CommissionFeeType { get; set; }

            [Required, DisplayName("Commission Fee Amount")]
            [Range(0, double.MaxValue, ErrorMessage = "Must be greater than 0 and within a reasonable range.")]
            public double CommissionFee { get; set; }

            [Required, DisplayName("Reset Position at End of Trading Day (Yes or No?)")]
            public bool ResetPosAtEoD { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Id { get; set; }
    }
}
