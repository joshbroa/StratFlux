using StratFlux.ModelEnums;
using StratFlux.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace StratFlux.ViewModels.Backtests
{
    public class GeneralResultsViewModel
    {
#nullable disable
        [Key]
        public string Id { get; set; }
#nullable enable

        [DisplayName("Result Name")]
        [MaxLength(20, ErrorMessage = "Must be within 20 characters.")]
        [RegularExpression(@"^[_A-Za-z0-9]+$", ErrorMessage = "Must only contain letters and/or numbers.")]
        public string? ResultsName { get; set; }

        [DisplayName("Unrealised Return/Loss")]
        public double? UnrealisedReturnLoss { get; set; }

        [DisplayName("Net Return/Loss")]
        public double? NetReturnLoss { get; set; }

        [DisplayName("Average Return/Loss")]
        public double? AverageReturnLoss { get; set; }

        [DisplayName("Average Holding Period")]
        public TimeSpan? AverageHoldingPeriod { get; set; }

        [DisplayName("Standard Deviation Over Time")]
        public double? StandardDeviationOverTime { get; set; }

        [DisplayName("Initial Equity")]
        public double? InitialEquity { get; set; }

        [DisplayName("Final Equity")]
        public double? FinalEquity { get; set; }

        [DisplayName("Max Drawdown")]
        public double? MaxDrawDown { get; set; }

        [DisplayName("Total Commission Fees")]
        public double? TotalCommissionAmount { get; set; }

        [DisplayName("Total Closed Trades")]
        public int? TotalClosedTrades { get; set; }

        [DisplayName("Winning Trades")]
        public int? WinningTrades { get; set; }

        [DisplayName("Losing Trades")]
        public int? LosingTrades { get; set; }

        [DisplayName("Time Resolution")]
        public TimeResolution? TimeResolution { get; set; }

        [DisplayName("Time Frame (Start)")]
        public DateTime? TimeFrameStart { get; set; }

        [DisplayName("Time Frame (End)")]
        public DateTime? TimeFrameEnd { get; set; }

        [DisplayName("Stock To Trade")]
        [MaxLength(5, ErrorMessage = "Must be 5 characters or less.")]
        public string? StockTraded { get; set; }
    }
}
