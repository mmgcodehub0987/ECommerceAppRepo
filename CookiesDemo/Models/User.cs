namespace CookiesDemo.Models
{
    public class User
    {
        public int? Id { get; set; }
        public string? UserName { get; set; } = null!; //Case sensitive
        public string? Password { get; set; } = null!;
        public string? FullName { get; set; } = null!;
    }
}
