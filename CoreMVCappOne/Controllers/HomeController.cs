using CoreMVCappOne.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreMVCappOne.Controllers
{
    public class HomeController : Controller
    {
        public JsonResult Index()
        {
            StudentRepsitory studentRepsitory = new StudentRepsitory();
            List<Student> allStudentsDetails = studentRepsitory.GetAllStudents();

            //return Json(studentRepsitory.GetAllStudents() ?? new List<Student>());
            return Json(allStudentsDetails);
        }

        public JsonResult GetStudentDetails(int id)
        {
            StudentRepsitory studentRepsitory = new StudentRepsitory();
            return Json(studentRepsitory.GetStudentByID(id));
        }

        //public ViewResult Details(int id)
        //{
        //    StudentRepsitory studentRepsitory = new StudentRepsitory();
        //    Student student = studentRepsitory.GetStudentByID(id);
        //    ViewData["Student"] = student;
        //    return View();
        //}


    }
}
