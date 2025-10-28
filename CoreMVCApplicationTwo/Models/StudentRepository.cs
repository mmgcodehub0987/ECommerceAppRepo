namespace CoreMVCApplicationTwo.Models
{
    public class StudentRepository : IStudentRepository
    {
        public Student getStudentByID(int id)
        {
            return this.myDataSource().FirstOrDefault(x => x.Id == id) ?? new Student();
        }
        public List<Student> getAllStudents()
        {
            return myDataSource();
        }

        public List<Student> myDataSource()
        {
            return new List<Student>()
            { new Student() { Id = 1, firstName="Bilbo", branch="A" , lastName="Baggins"},
              new Student() { Id = 2, firstName="Frodo", lastName="Baggins" , branch="A" }
            };

        }


    }
}
