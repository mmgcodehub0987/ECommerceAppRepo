using Microsoft.AspNetCore.Mvc;
using ModelBindingMVC.Models;
using System.Diagnostics;

namespace ModelBindingMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult Create()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]
        public IActionResult Create([FromForm] User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            return RedirectToAction("Success", user);
        }

        [HttpGet]
        public IActionResult Success(User user)
        {
            return View(user);
        }
    }
}
