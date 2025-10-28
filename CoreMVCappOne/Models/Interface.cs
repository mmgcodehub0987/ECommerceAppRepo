namespace CoreMVCappOne.Models
{
    //this is called Service interface
    public interface IStudentRepository
    {
        Student GetStudentByID(int id);
        List<Student> GetAllStudents();
    }
}
