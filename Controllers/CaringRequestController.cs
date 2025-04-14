using Microsoft.AspNetCore.Mvc;

namespace mvc_pets.Controllers
{
    public class CaringRequestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
