using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StratFlux.Models
{
#nullable disable
    public class Strategy
    {
        [Key]
        public string Id { get; set; }

        [Required, DisplayName("Strategy Name")]
        [MaxLength(20, ErrorMessage = "Must be within 20 characters.")]
        [RegularExpression(@"^[_A-Za-z0-9]+$", ErrorMessage = "Must only contain letters and/or numbers.")]
        public string StrategyName { get; set; }
        
#nullable enable
        [MaxLength(100, ErrorMessage = "Must be within 100 characters.")]
        [RegularExpression(@"^[\-_\(\);:\$£""%\/&!\.A-Za-z0-9\s]*$")]
        public string? StrategyDescription { get; set; }
#nullable disable

        [Required, DisplayName("Strategy"), Column(TypeName = "jsonb")]
        public string StrategyJson { get; set; }

        [ForeignKey("User"), Required]
        public string UserId { get; set; }

        public StratUser User { get; set; }
    }
}
