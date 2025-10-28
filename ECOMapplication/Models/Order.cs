using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECOMapplication.Models
{
    public class Order
    {
        public int OrderId { get; set; }


        [Required(ErrorMessage ="Order Number is required.")]
        [StringLength(30, ErrorMessage ="Order Number cannot exceed 30 characters.")]
        public int OrderNumber { get; set; }


        [Required(ErrorMessage = "Order date is required.")]
        public DateTime OrderDate { get; set; }


        [Required(ErrorMessage ="Customer ID is required.")]
        public int CustomerId { get; set; }


        [ForeignKey("CustomerId")]public Customer Customer { get; set; }


        [Required(ErrorMessage ="Billing address Id is required.")] 
        public int BillingAddressId { get; set; }


        [Required(ErrorMessage = "Shipping address Id is required.")] 
        public int ShippingAddressId { get; set; }


        [ForeignKey("BillingAddressId")]public Address BillingAddress { get; set; }
        [ForeignKey("ShippingAddressId")]public Address ShippingAddress { get; set; }
        
        
        
        [Required]
        [Column(TypeName ="decimal(18,2)")]
        public decimal TotalBaseAmount { get; set; }   
        


        [Required]
        [Column(TypeName = "decimal(18,2)")] 
        public decimal DiscountAmoount { get; set; }


        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCost { get; set; }


        [Required]
        [Column(TypeName = "decimal(18,2)")] 
        public decimal TotalAmount { get; set; }


        [Required]
        [EnumDataType(typeof(OrderStatus), ErrorMessage ="Invalid Order Status")]
        public OrderStatus OrderStatus { get; set; }


        [Required]
        public ICollection<OrderItem> OrderItems { get; set; }
        public Payment Payment {  get; set; }
        public Cancellation Cancellation { get; set; }



    }
}
