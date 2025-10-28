using ClassicPRGPattern.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClassicPRGPattern.Controllers
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
            ViewData["Title"] = "Home Page";
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
        public IActionResult FeedBackForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult FeedBackFormPOST(Form form)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessAcknowledgeMessage"] = "Yayy !! you have successfully submitted the form";
                return RedirectToAction("Confirmation");
            }
            return View(form);
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
