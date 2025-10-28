using System.ComponentModel.DataAnnotations;

namespace CookiesDemo.Models
{
    public class LoginModel
    {
        [Required] public string Username { get; set; } = null!;
        [DataType(DataType.Password)] public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
