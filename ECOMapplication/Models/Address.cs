using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECOMapplication.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        [Required]public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }


        [Required(ErrorMessage ="Address Line 1 is required.")]
        [StringLength(80, ErrorMessage ="Address Line 1 is too long.")]
        public string AddressLine1 { get; set; } = string.Empty;


        [Required(ErrorMessage = "Address Line 2 is required.")]
        [StringLength(80,ErrorMessage ="Address Line 2 is too long")]
        public string AddressLine2 { get; set; } = string.Empty;


        [Required(ErrorMessage ="City is rquired.")]
        public string City { get; set; } = string.Empty;


        [Required(ErrorMessage ="State is required.")]
        public string State { get; set;}= string.Empty;


        [Required(ErrorMessage ="Country is required.")]
        public string Country { get; set; } = string.Empty;


        [Required(ErrorMessage ="Postal Code is required.")] 
        public string PostalCode { get; set; }= string.Empty;

    }
}
