using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http.Headers;

namespace ECOMapplication.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        [Required (ErrorMessage ="Order Id is required.")]public int OrderId { get; set; }
        [ForeignKey("OrderId")]public Order Order { get; set; }
        [Required(ErrorMessage ="Product Id is required.")]public int ProdctId { get; set; }
        [ForeignKey("ProductId")]public Product Product { get; set; }
        
        public int Quantity { get; set; }


        [Column(TypeName ="decimal(18,2)")]
        public decimal UnitPrice { get; set; }


        [Column(TypeName = "decimal(18,2)")] 
        public decimal Discount { get; set; }


        [Column(TypeName = "decimal(18,2)")] 
        public decimal TotalPrice { get; set; }

    }
}
