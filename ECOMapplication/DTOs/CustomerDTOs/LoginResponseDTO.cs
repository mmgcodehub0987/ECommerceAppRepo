namespace ECOMapplication.DTOs.CustomerDTOs
{
    public class LoginResponseDTO
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
