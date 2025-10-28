using System.ComponentModel.DataAnnotations;
namespace ECOMapplication.Models
{
    public class Customer
    {
        public int Id { get; set; }


        [Required(ErrorMessage ="First Name is required.")]
        [StringLength(50, MinimumLength =2, ErrorMessage ="First Name must be between 2 and 50 characters")]
        public string FirstName { get; set; } = string.Empty;


        [Required(ErrorMessage ="Last Name is required.")]
        public string LastName { get; set; } = string.Empty;


        [Required(ErrorMessage ="Email is required.")]
        [EmailAddress(ErrorMessage ="Please enter the correct email.")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage ="Phone number is required.")]
        [Phone(ErrorMessage ="Please enter the correct phone number.")]
        public string PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage ="Date of Birth is required.")]
        public DateTime DateOfBirth { get; set; } = DateTime.MinValue;


        [Required(ErrorMessage ="Please enter the password.")] public string Password {  get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public ICollection<Address>? Adresses { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public Cart? Carts { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }


    }
}
