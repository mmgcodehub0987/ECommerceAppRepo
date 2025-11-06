namespace ECOMapplication.DTOs.FeedBackDTOs
{
    public class ProductFeedbackResponseDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public List<CustomerFeedbackDTO> Feedbacks { get; set; }
    }
}
