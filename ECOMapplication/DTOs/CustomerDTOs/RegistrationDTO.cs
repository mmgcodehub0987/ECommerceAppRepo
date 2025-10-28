using System.ComponentModel.DataAnnotations;

namespace ECOMapplication.DTOs.CustomerDTOs
{
    public class RegistrationDTO
    {
        [Required(ErrorMessage = "First Name is required.")]
        [MinLength(2, ErrorMessage = "First Name must be at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        [MinLength(2, ErrorMessage = "Last Name must be at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "PhoneNumber is required.")]
        [Phone(ErrorMessage = "Invalid Phone Number.")] 
        public string PhoneNumber {  get; set; } =string.Empty;


        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")] 
        public string Password { get; set; } = string.Empty ;


        [Required(ErrorMessage = "DateOfBirth is required.")] 
        public DateTime DateOfBirth { get; set; }

    }
}
