using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StratFlux.Models
{
#nullable disable
    public class Indicator
    {
        [Key]
        public string Id { get; set; }

        [Required, DisplayName("Indicator Name")]
        [MaxLength(20, ErrorMessage = "Must be less than 20 characters.")]
        [MinLength(1, ErrorMessage = "Must have a name.")]
        [RegularExpression(@"^(a-zA-Z0-9\-)+$", ErrorMessage = "Must only contain letters, numbers or a '-'.")]
        public string IndicatorName { get; set; }

        // Boolean value which indicates whether this indicator is a single line or two lines with area
        [Required, DisplayName("Has Area")]
        public bool HasArea { get; set; }

        [Required, DisplayName("Primary Line Colour")]
        [MaxLength(6, ErrorMessage = "Must be exactly 6 characters long (must be a valid hexadecimal RGB value).")]
        [MinLength(6, ErrorMessage = "Must be exactly 6 characters long (must be a valid hexadecimal RGB value).")]
        [RegularExpression(@"^(0-9A-F)+$", ErrorMessage = "Must be a valid hexadecimal RGB value.")]
        public string PrimaryLineColour { get; set; }

#nullable enable
        [DisplayName("Secondary Line Colour")]
        [MaxLength(6, ErrorMessage = "Must be exactly 6 characters long (must be a valid hexadecimal RGB value).")]
        [MinLength(6, ErrorMessage = "Must be exactly 6 characters long (must be a valid hexadecimal RGB value).")]
        [RegularExpression(@"^(0-9A-F)+$", ErrorMessage = "Must be a valid hexadecimal RGB value.")]
        public string? SecondaryLineColour { get; set; }

        [DisplayName("Positive Area Colour")]
        [MaxLength(6, ErrorMessage = "Must be exactly 6 characters long (must be a valid hexadecimal RGB value).")]
        [MinLength(6, ErrorMessage = "Must be exactly 6 characters long (must be a valid hexadecimal RGB value).")]
        [RegularExpression(@"^(0-9A-F)+$", ErrorMessage = "Must be a valid hexadecimal RGB value.")]
        public string? PositiveAreaColour { get; set; }

        [DisplayName("Negative Area Colour")]
        [MaxLength(6, ErrorMessage = "Must be exactly 6 characters long (must be a valid hexadecimal RGB value).")]
        [MinLength(6, ErrorMessage = "Must be exactly 6 characters long (must be a valid hexadecimal RGB value).")]
        [RegularExpression(@"^(0-9A-F)+$", ErrorMessage = "Must be a valid hexadecimal RGB value.")]
        public string? NegativeAreaColour { get; set; }

        // This returns a list of 3 integer values which represent the Red, Green Blue values respectively of a specified property's colour
        public int[] GetRgbValues(int colour)
        {
            // It is known that only 3 integer values need to be returned
            int[] values = new int[3];
            string hexColour;

            // If there is no area, only the primary line colour exists (or is the only colour relevant)
            // If one of the other nullable properties is null, it will select PrimaryLineColour by default
            if (HasArea == false || SecondaryLineColour == null || PositiveAreaColour == null || NegativeAreaColour == null)
            {
                hexColour = PrimaryLineColour;
            }
            else
            {
                // Colour 1 is primary line colour, colour 2 is secondary line colour
                // Colour 3 is positive area colour and colour 4 is negative area colour
                switch (colour)
                {
                    case 2:
                        hexColour = SecondaryLineColour;
                        break;
                    case 3:
                        hexColour = PositiveAreaColour;
                        break;
                    case 4:
                        hexColour = NegativeAreaColour;
                        break;
                    default:
                        hexColour = PrimaryLineColour;
                        break;
                }
            }

            // The colours are stored as 6 character long hex values
            // Each pair of characters are converted to integers and then appended to the return list
            for (int i = 0; i < 3; i++)
            {
                // The Convert.ToInt32() method takes in a string as well as the base being converted from
                // The substring expression returns 2 characters for each pair of characters in the hex string as 'i' increases
                values[i] = Convert.ToInt32(hexColour.Substring(i * 2, 2), 16);
            }

            return values;
        }
    }
}
