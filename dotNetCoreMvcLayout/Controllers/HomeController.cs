using dotNetCoreMvcLayout.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace dotNetCoreMvcLayout.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }

        public ViewResult Details()
        {
            return View();
        }
    }
}
