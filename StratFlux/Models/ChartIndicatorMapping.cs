using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StratFlux.Models
{
    // This class removes the many to many relationship between TblCharts and TblIndicators
#nullable disable
    public class ChartIndicatorMapping
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("ChartParent"), Required]
        public string ChartId { get; set; }

        [ForeignKey("IndicatorParent"), Required]
        public string IndicatorId { get; set; }

        public Chart ChartParent { get; set; }

        public Indicator IndicatorParent { get; set; }
    }
}
