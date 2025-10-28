namespace EFcoreOne.Models
{
    public class Student
    {
        public int? StudentID { get; set; }
        public string? StudentName { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public Branch BranchID { get; set; }
        public Branch BranchName { get; set; }
    }
}
