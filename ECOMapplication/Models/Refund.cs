using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECOMapplication.Models
{
    public class Refund
    {
        public int Id { get; set; }

        [Required (ErrorMessage ="Cancellation Id is required.")]
        public int CancellationId { get; set; }


        [ForeignKey("CancellationId")]
        public Cancellation Cancellation { get; set; }


        [Required (ErrorMessage ="Payement ID is required.")]
        public int PaymentId { get; set; }


        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }


        [Column(TypeName ="decimal(18,2)")]
        public decimal RefundAmount { get; set; }


        [Required]
        public RefundStatus RefundStatus { get; set; }


        [Required]
        public string RefundMethod { get; set; } = string.Empty;


        [StringLength(500, ErrorMessage = "Refund Reason cannot exceed 500")]
        public string RefunndReason { get; set; } = string.Empty;


        public string TransactionId { get; set; } = string.Empty;


        [Required]
        public DateTime InitiatedAt { get; set; }

        public DateTime CompletedAt { get; set; } = DateTime.MinValue;

        public int ProcessedBy { get; set; }
    }
}
