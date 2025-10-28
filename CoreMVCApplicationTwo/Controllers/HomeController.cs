using CoreMVCApplicationTwo.Models;
using CoreMVCApplicationTwo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CoreMVCApplicationTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public ViewResult Details()
        {
            StudentRepository studentRepository = new StudentRepository();
            ViewData["DataOne"] = "The Student Data";
            ViewData["DataTwo"] = "The Header : The begining";
            ViewData["DataThree"] = studentRepository.getStudentByID(1);
            //ViewData["DataFour"] = studentRepository.getAllStudents();


            return View();
        }

        public ViewResult StudentInfo()
        {
            StudentDetailsViewModel studentDetailsViewModel = new StudentDetailsViewModel();
            Student student = new Student();
            Address address = new Address();
            return View();
        }
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
    }
}
