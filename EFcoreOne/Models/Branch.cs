namespace EFcoreOne.Models
{
    public class Branch
    {
        public int? BranchID { get; set; }
        public string? BranchName { get; set; }
        public ICollection<Student>? Students { get; set; }
    }
}
