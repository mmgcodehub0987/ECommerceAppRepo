using System.ComponentModel.DataAnnotations;

namespace ECOMapplication.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }


        public Customer Customer { get; set; }


        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }


        // Optional comment with maximum length
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string? Comment { get; set; }


        // Timestamp of feedback submission
        public DateTime CreatedAt { get; set; }

        
        // Timestamp of feedback updation
        public DateTime UpdatedAt { get; set; }
    }
}
