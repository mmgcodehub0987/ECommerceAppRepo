using Microsoft.AspNetCore.Mvc;
using CookiesDemo.Models;

namespace CookiesDemo.Controllers
{
    public class AccountController : Controller
    {
        //private User _user= new User();
        private readonly UserService _userService;
        private const string CookieUserId = "UserID";
        private const string CookieUserName = "UserName";

        //constructor based dependency injection.
        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        //create A get method to requeest a login page
        [HttpGet]
        public IActionResult Login()
        {
            if (Request.Cookies.ContainsKey(CookieUserId))
            {
                return RedirectToAction("Dashboard");
            }
            return View("Login");
        }

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            if(!ModelState.IsValid) 
                return View(loginModel);
            var _user = _userService.ValidateUser(loginModel.Username, loginModel.Password);
            if (_user == null)
            {
                ModelState.AddModelError("", "User Not found ! Check your username and password again.");
                return View(loginModel);
            }

            //Give some cookie options
            var _UserCookieOptions = new CookieOptions { Path ="/", HttpOnly = true, IsEssential = true, Secure = true};
            //Add some gifts from server to client.
            Response.Cookies.Append(CookieUserId, _user.Id.ToString(), _UserCookieOptions);
            Response.Cookies.Append(CookieUserName, _user.UserName.ToString(), _UserCookieOptions);
            return RedirectToAction("Dashboard", "Account");
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            if (!Request.Cookies.ContainsKey(CookieUserId))
                return RedirectToAction("Login");
            if (!int.TryParse(Request.Cookies["UserID"], out int id))
                return RedirectToAction("Login");

            var _user = _userService.GetUserById(id);
            if (_user == null)
                return RedirectToAction("Login");
            return View("Dashboard", _user);
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete(CookieUserId);
            Response.Cookies.Delete(CookieUserName);
            return RedirectToAction("Login");
        }

    }
}
