namespace CookiesDemo.Models
{
    public class UserService : IUserService
    {
        //List<User> _users = new List<User>();
        public User? GetUserById(int id)
        {
            return GetUsers().FirstOrDefault(u => u.Id == id);
        }

        public List<User> GetUsers()
        {
            return new List<User>() { new User { UserName = "JohnDoe33", Id = 001, FullName = "John Doe", Password = "john3244@" },
            new User { UserName="AtharvicOne", Id=002, FullName="Atharva M", Password="pandalaKanda@2000" },
            new User {UserName="HarveySpecs", Id=003, FullName="Harvey Specter", Password="Harvey@sp"},
            new User {UserName="MikeRoss", Id=004, FullName="Mike Ross", Password="Mikey@45"},
            new User {UserName="DonnaDont", Id=005, FullName="Donna", Password="DontDont"}};
        }

        public User? ValidateUser(string uName, String pWord)
        {
            return GetUsers().FirstOrDefault(i => i.UserName.Equals(uName) && i.Password == pWord);
        }
    }
}
