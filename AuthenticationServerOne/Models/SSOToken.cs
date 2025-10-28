using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AuthenticationServerOne.Models
{
    public class SSOToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public bool IsExpired => DateTime.UtcNow > ExpiryDate; //every time this is accessed it evaluates and returns a bool value;
        //public bool IsLocked { get; } = DateTime.UtcNow > ExpiryDate;

        [Required]public int UserId { get; set; }
        [ForeignKey("UserId")]
        public UserModel User { get; set; } = null!;
    }
}
