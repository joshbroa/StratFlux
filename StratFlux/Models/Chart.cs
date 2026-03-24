using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StratFlux.Models
{
#nullable disable
    public class Chart
    {
        [Key]
        public string Id { get; set; }

        [Required, DisplayName("Requires Stock Data")]

        // Boolean value to indicate if stock data (ohlc +  volume) will be displayed with other indicators
        public bool ContainsStockData { get; set; }

        [ForeignKey("Results"), Required]
        public string GeneralResultsId { get; set; }
        public GeneralResults Results { get; set; }
    }
}
