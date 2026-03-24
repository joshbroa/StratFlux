using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace StratFlux.ViewModels.Strategies
{
    public class StrategyViewModel
    {
        public class InputModel
        {
#nullable disable
            [Required, DisplayName("Strategy Name")]
            [MaxLength(20, ErrorMessage = "Must be within 20 characters.")]
            [RegularExpression(@"^[_A-Za-z0-9]+$", ErrorMessage = "Must only contain letters and/or numbers.")]
            public string StrategyName { get; set; }

#nullable enable            
            [MaxLength(100, ErrorMessage = "Must be within 100 characters.")]
            [RegularExpression(@"^[\-_\(\);:\$£""%\/&!\.A-Za-z0-9\s]*$")]
            public string? StrategyDescription { get; set; }
#nullable disable

            [Required, DisplayName("Strategy")]
            public string StrategyJson { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Id { get; set; }
#nullable enable
    }
}
