using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECOMapplication.Models
{
    public class Cancellation
    {
        public int Id { get; set; }
        [Required] public int OrderId { get; set; }
        [ForeignKey("OrderId")] public Order? Order { get; set; }


        [Required(ErrorMessage ="Cancellation Reason is required.")]
        [StringLength(500, ErrorMessage ="Cancellation Reason cannot exceed 500 characters")]
        public string Reason { get; set; } = string.Empty;


        [Required]
        public CancellationStatus CancellationStatus { get; set; }


        [Required]
        public DateTime RequestedAt { get; set; }


        [Required]
        public DateTime ProcessedAt { get; set; }


        public int ProcessedBy { get; set; }


        [Column(TypeName ="decimal(18,2)")]
        public decimal OrderAmount { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal CancellationCharges { get; set; } = decimal.Zero;


        [StringLength(500, ErrorMessage ="Remarks cannot exceed 500 characters.")]
        public string Remarks {  get; set; } = string.Empty;


        public Refund? Refund { get; set; }
    }
}
