using System.ComponentModel.DataAnnotations;

namespace AuthenticationServerOne.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required] public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        [Required] public string UserName { get; set; } = string.Empty;
        [Required] public string PasswordHash { get; set; } = string.Empty;


        [Required(ErrorMessage = "Email ID is required.")]
        [EmailAddress(ErrorMessage = "Emial Adress Format is incorrect.")]
        public string Email { get; set; } = string.Empty;
        [Required] public bool EmailConfirmed { get; set; }


        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Phone number format is incorrect.")]
        public string PhoneNumber { get; set; } = string.Empty;

        //relation
        public ICollection<SSOToken>? SSOTokens { get; set; }
        
    }
}
