using ClassicPRGPattern.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClassicPRGPattern.Controllers
{
    public class FeedBackController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpGet]
        public IActionResult FeedBackForm()
        {
            ViewData["FeedBack"] = "";
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
