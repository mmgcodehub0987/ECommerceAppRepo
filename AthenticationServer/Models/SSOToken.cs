namespace AthenticationServer.Models
{
    public class SSOToken
    { 
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public bool IsExpired { get; set; }
    }
}
