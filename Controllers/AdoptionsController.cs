using Microsoft.AspNetCore.Mvc;
using mvc_pets.Models;
using System.Diagnostics;
namespace mvc_pets.Controllers
{
    public class AdoptionsController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }
    }
}
