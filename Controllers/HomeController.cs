using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvc_pets.Models;
using System.Diagnostics;
using System.Linq;
using mvc_pets.ViewModel;
using mvc_pets.Data;

namespace mvc_pets.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext db) 
            : base(db)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var aboutUs = _context.SiteContents.FirstOrDefault(s => s.Key == "AboutUs");
            var aboutShelter = _context.SiteContents.FirstOrDefault(s => s.Key == "AboutShelter");
            var careGuide = _context.SiteContents.FirstOrDefault(s => s.Key == "CareGuide");
            var cards = _context.HomeCards.ToList();

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
            var careGuide = _context.SiteContents.FirstOrDefault(s => s.Key == "CareGuide");
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
