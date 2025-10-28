using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECOMapplication.Models
{
    public class Payment
    {
        public int Id { get; set; }
        [Required] public int OrderId { get; set; }
        [ForeignKey("OrderId")] public Order Order { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        [Required]public decimal Amount { get; set; }
        [Required] public DateTime PaymentDate { get; set; }
        [Required] public PaymentStatus PaymentStatus { get; set; }
        public Refund Refund { get; set; }

    }
}
