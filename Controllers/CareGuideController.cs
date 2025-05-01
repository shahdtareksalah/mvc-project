using Microsoft.AspNetCore.Mvc;

namespace mvc_pets.Controllers
{
    public class CareGuideController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
} 