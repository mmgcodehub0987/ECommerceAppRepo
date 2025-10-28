namespace CookiesDemo.Models
{
    public interface IUserService
    {
        List<User> GetUsers();
        User GetUserById(int id);
    }
}
