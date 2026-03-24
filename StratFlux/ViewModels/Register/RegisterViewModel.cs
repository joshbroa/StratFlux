using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StratFlux.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Xml.Linq;

namespace StratFlux.ViewModels.Register
{
    public class RegisterViewModel
    {
        public class InputModel
        {
#nullable disable
            [Required, DisplayName("User Name")]
            [MaxLength(15, ErrorMessage = "Must be 30 characters or less.")]
            [MinLength(3, ErrorMessage = "Must be at least 3 characters.")]
            [RegularExpression(@"^[a-zA-Z0-9\-_]+$", ErrorMessage = "Can only contain letters, numbers and '_' or '-'.")]
            public string UserName { get; set; }

            [Required, DisplayName("First Name")]
            [MaxLength(30, ErrorMessage = "Must be 30 characters or less.")]
            [MinLength(1, ErrorMessage = "Must be at least 1 character.")]
            [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Can only contain letters (capital or lower case).")]
            public string FirstName { get; set; }

            [Required, DisplayName("Last Name")]
            [MaxLength(30, ErrorMessage = "Must be 30 characters or less.")]
            [MinLength(1, ErrorMessage = "Must be at least 1 character.")]
            [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Can only contain letters (capital or lower case).")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

#nullable enable
    }
}
