using System.ComponentModel.DataAnnotations;

namespace ECOMapplication.DTOs.FeedBackDTOs
{
    public class FeedBackDeleteDTO
    {
        [Required(ErrorMessage = "FeedbackId is required.")]
        public int FeedbackId { get; set; }
        [Required(ErrorMessage = "CustomerId is required.")]
        public int CustomerId { get; set; }
    }
}
