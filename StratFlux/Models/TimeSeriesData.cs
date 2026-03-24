using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StratFlux.Models
{
#nullable disable
    public class TimeSeriesData
    {
        [Key]
        public string Id { get; set; }

        [Required, DisplayName("Time Stamp")]
        public DateTime TimeStamp { get; set; }

        [Required, DisplayName("Value")]
        public double Value { get; set; }

        // Will be secondary only if indicator is an enclosed area.
        [Required, DisplayName("Is Primary")]
        public bool IsPrimary { get; set; }

        [ForeignKey("ParentIndicator"), Required]
        public string IndicatorId { get; set; }

        public Indicator ParentIndicator { get; set; }
    }
}
