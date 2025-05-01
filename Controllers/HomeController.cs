using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvc_pets.Models;
using System.Diagnostics;
using System.Linq;
using mvc_pets.ViewModel;

namespace mvc_pets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _logger = logger;
            _userManager = userManager;
            _db = db;
        }

        public IActionResult Index()
        {
            var aboutUs = _db.SiteContents.FirstOrDefault(s => s.Key == "AboutUs");
            var aboutShelter = _db.SiteContents.FirstOrDefault(s => s.Key == "AboutShelter");
            var careGuide = _db.SiteContents.FirstOrDefault(s => s.Key == "CareGuide");
            var cards = _db.HomeCards.ToList();

            var model = new HomeViewModel
            {
                AboutUs = aboutUs,
                AboutShelter = aboutShelter,
                CareGuide = careGuide,
                HomeCards = cards
            };
            return View(model);
        }
        public IActionResult CareGuide()
        {
            var careGuide = _db.SiteContents.FirstOrDefault(s => s.Key == "CareGuide");
            var model = new HomeViewModel
            {
                CareGuide = careGuide
            };
            return View(model);
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
