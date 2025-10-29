using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECOMapplication.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Product Name is required.")] 
        public string ProductName { get; set; } = string.Empty;
        
        
        [Required(ErrorMessage ="Description is required.")]
        [StringLength(800, ErrorMessage ="Description entered is too long")]
        [MinLength(15, ErrorMessage ="Minimum of 15 characters must be entered in description.")]
        public string Description { get; set; } = string.Empty;


        [Required(ErrorMessage = "Price is Required.")] 
        [Column(TypeName ="decimal(18,2)")]
        public decimal Price { get; set; }


        public int StockQuantity { get; set; }
        public string ImageURL { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public bool IsAvailable { get; set; }


        //
        [Required(ErrorMessage ="Category Id is required.")]
        public int CategoryId { get; set; }


        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;


        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }


    }
}
