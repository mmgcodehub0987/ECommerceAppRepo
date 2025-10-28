using System.ComponentModel.DataAnnotations;

namespace ModelBindingMVC.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required (ErrorMessage ="Please enter your full name")]
        public string? Name { get; set; }
        [Required (ErrorMessage ="You have a gender right?")]
        public string? Gender { get; set; }
        public string? Email {  get; set; }
        public State State { get; set; }
        [Required(ErrorMessage ="Enter 10 digit mobile number")] 
        public string? Mobile { get; set; }
    }
}
