namespace CoreMVCappOne.Models
{
    public class StudentRepsitory : IStudentRepository
    {
        public List<Student> GetAllStudents()
        {
            return DatabaseData();
        }

        public Student GetStudentByID(int StudID)
        {
            return DatabaseData().FirstOrDefault(e => e.Id == StudID) ?? new Student();
        }

        //public List<Student> DatabaseData()
        //{
        //    List<Student> students = new List<Student>();
        ////    students.Add(new Student() { Id = 101, Name = "John", Gender = "male", Branch = "Cse", Section = "a" });
        ////    students.Add(new Student() { Id = 102, Name = "Cercy", Branch = "Cse", Section = "a", Gender = "female" });
        ////    students.Add(new Student() { Id = 103, Name = "Jamie", Branch = "Cse", Section = "a", Gender = "male" });
        ////    students.Add(new Student() { Id = 104, Name = "Peter", Branch = "ISE", Section = "c", Gender = "male" });
        //    return students;

        //}

        public List<Student> DatabaseData()
        {
            return new List<Student>() { new Student() { Id= 201, Name="Richard", Branch="eee", Gender="Male", Section="B" },
                                         new Student() { Id=202, Name="Anthony", Branch="mech", Gender="Male", Section="A"},
                                         new Student() { Id=203, Name="Shivai", Branch="civil", Gender="Female", Section="B" },
                                         new Student() { Id = 101, Name = "John", Gender = "male", Branch = "Cse", Section = "a" },
                                         new Student() { Id = 102, Name = "Cercy", Branch = "Cse", Section = "a", Gender = "female" }
            };
        }
    }
}
