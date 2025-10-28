namespace CoreMVCApplicationTwo.Models
{
    public interface IStudentRepository
    {
        public Student getStudentByID(int id);
        public List<Student> getAllStudents();
    }
}
