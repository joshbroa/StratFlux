using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Alpaca.Markets;
using StratFlux.ModelEnums;

#nullable disable
namespace StratFlux.Models
{
    public class BacktestingSettings
    {
        [Key]
        public string Id { get; set; }

        [Required, DisplayName("Backtesting Settings Name")]
        [MaxLength(20, ErrorMessage = "Must be within 20 characters.")]
        [RegularExpression(@"^[_A-Za-z0-9]+$", ErrorMessage = "Must only contain letters and/or numbers.")]
        public string BacktestingSettingsName { get; set; }

        [Required, DisplayName("Stock To Trade")]
        [MaxLength(5, ErrorMessage = "Must be 5 characters or less.")]
        [RegularExpression(@"^[A-Z]+$", ErrorMessage = "Must be all capital letters and no other characters.")]
        public string StockToTrade { get; set; }

        [Required, DisplayName("Time Resolution")]
        [RegularExpression(@"^[01234]$", ErrorMessage = "Must be a selection of one of the five items.")]
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
        [RegularExpression(@"^[01]$", ErrorMessage = "Must be a selection of one of the two items.")]
        public CommissionFeeType CommissionFeeType { get; set; }

        [Required, DisplayName("Commission Fee Amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Must be greater than 0 and within a reasonable range.")]
        public double CommissionFee { get; set; }

        [Required, DisplayName("Reset Position at End of Trading Day (Yes or No?)")]
        public bool ResetPosAtEoD { get; set; }

        [ForeignKey("User"), Required]
        public string UserId { get; set; }

        public virtual StratUser User { get; set; }

        // This method allows for the corresponding Time Resolution to be retrieved for use in the API
        public BarTimeFrame GetTimeResolution()
        {
            Dictionary<TimeResolution, BarTimeFrame> ResolutionMappings = new Dictionary<TimeResolution, BarTimeFrame>()
            {
                {TimeResolution.PerMinute, BarTimeFrame.Minute},
                {TimeResolution.Hourly, BarTimeFrame.Hour},
                {TimeResolution.Daily, BarTimeFrame.Day},
                {TimeResolution.Weekly, BarTimeFrame.Week},
                {TimeResolution.Monthly, BarTimeFrame.Month}
            };

            return ResolutionMappings[TimeResolution];
        }
    }
}
