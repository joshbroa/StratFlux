using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StratFlux.Models
{
#nullable disable
    public class FriendList
    {
        [Key]
        public string Id { get; set; }

        [Required, DisplayName("User 1 Accepted")]
        public bool User1Accepted { get; set; }

        [Required, DisplayName("User 2 Accepted")]
        public bool User2Accepted { get; set; }

        [ForeignKey("User1"), Required]
        public string User1Id { get; set; }

        [ForeignKey("User2"), Required]
        public string User2Id { get; set; }

        public virtual StratUser User1 { get; set; }
        public virtual StratUser User2 { get; set; }

        // This method returns True if users are friends and False if the request is pending
        public bool CheckFriendStatus()
        {
            return User1Accepted && User2Accepted;
        }
    }
}
