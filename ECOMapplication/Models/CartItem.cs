using Microsoft.EntityFrameworkCore.Storage.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECOMapplication.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        [Required] public int CartId { get; set; }
        [ForeignKey("CartId")] public Cart? Cart { get; set; }
        [Required] public int ProductId { get; set; }
        [ForeignKey("ProductId")] public Product? Product { get; set; }


        [Required]
        [Range(1,100, ErrorMessage ="Minimum of one item must be selected. Maximum item limit is 100.")]
        public int Quantity { get; set; }


        [Column(TypeName ="decimal(18,2)")][Required]
        public decimal UnitPrice { get; set; }


        [Column(TypeName = "decimal(18,2)")] 
        public decimal Discount { get; set; }


        [Column(TypeName = "decimal(18,2)")] 
        public decimal TotalPrice { get; set; }


        [Required] public DateTime CreatedAt { get; set; }
        [Required] public DateTime UpdatedAt { get; set; }
    }
}
