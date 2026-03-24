using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace StratFlux.ViewModels.Login
{
    public class LoginViewModel
    {
        public class InputModel
        {
#nullable disable
            [Required, DisplayName("User Name or Email")]
            public string UserNameOrEmail { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DisplayName("Remember Me")]
            public bool RememberMe { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }
#nullable enable
    }
}
