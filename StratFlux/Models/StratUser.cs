using Microsoft.AspNetCore.Identity;
using StratFlux.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StratFlux.Models
{
#nullable disable
    public class StratUser : IdentityUser
    {
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

#nullable enable
        [DisplayName("Profile Picture")]
        public byte[]? ProfilePicture { get; set; }
    }
}
