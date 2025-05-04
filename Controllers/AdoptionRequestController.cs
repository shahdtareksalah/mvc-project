using Microsoft.AspNetCore.Mvc;
using mvc_pets.Models;
using System.Diagnostics;
namespace mvc_pets.Controllers
{
    public class AdoptionRequestController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }
    }
}
