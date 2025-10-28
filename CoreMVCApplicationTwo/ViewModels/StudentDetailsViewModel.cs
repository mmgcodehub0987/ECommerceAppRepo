using CoreMVCApplicationTwo.Models;

namespace CoreMVCApplicationTwo.ViewModels
{
    public class StudentDetailsViewModel
    {
        public Student? student { get; set; }
        public Address? address { get; set; }
        public string? Title { get; set; }
        public string? Header { get; set; }
    }
}
