using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StratFlux.Models
{
#nullable disable
    public class MessageHistory
    {
        [Key]
        public string Id { get; set; }

        [Required, DisplayName("Message")]
        [MaxLength(1024, ErrorMessage = "Must be less than 1024 characters long.")]
        [RegularExpression(@"^[\-_\(\);:\$£""%\/&!\.A-Za-z0-9\s]*$", ErrorMessage = "Only letters, numbers and certain special characters are allowed.")]
        public string Message { get; set; }

        // This is the timestamp of when the message was sent.
        [Required, DisplayName("Time Message was Sent")]
        public DateTime TimeStamp { get; set; }

        // Boolean value to indicate other user in TblFriendList has read this message
        [Required, DisplayName("Read By Other User")]
        public bool ReadByOtherUser { get; set; }

        // This stores the two people who's messages these are
        [ForeignKey("Friends"), Required]
        public string FriendsId { get; set; }

        // This is the user's Id who sent this particular message
        [ForeignKey("SentByUser"), Required]
        public string SentByUserId { get; set; }

        public virtual FriendList Friends { get; set; }

        public virtual StratUser SentByUser { get; set; }
    }
}
