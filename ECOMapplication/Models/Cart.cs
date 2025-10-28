using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECOMapplication.Models
{
    public class Cart
    {
        public int Id { get; set; }
        [Required]public int CustomerId { get; set; }
        [ForeignKey("CustomerId")] public Customer? Customer { get; set; }
        public bool IsCheckedOut { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }



    }
}
