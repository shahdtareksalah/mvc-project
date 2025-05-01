using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvc_pets.Models;
using System.Data;
using System.Linq;

namespace mvc_pets.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ApplicationDbContext _db;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ApplicationDbContext db)
        {
            _userManager = userManager;
            _context = context;
            _db = db;
        }

        public IActionResult Dashboard()
        {
            var stats = new
            {
                // If you add IsActive/IsDeleted, filter like: _context.Pets.Count(p => p.IsActive)
                TotalUsers = _userManager.Users.Count(),
                TotalPets = _context.Pets.Count(),
                TotalAdoptionRequests = _context.Adoptions.Count(),
                TotalDonations = _context.Donations.Count()
            };

            return View(stats);
        }

        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new List<(ApplicationUser User, IList<string> Roles)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add((user, roles));
            }

            return View(userRoles);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                user.IsAdmin = false;
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                user.IsAdmin = true;
            }

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Users));
        }

        public IActionResult HomeCards()
        {
            return View(_db.HomeCards.ToList());
        }

        public IActionResult AddHomeCard()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddHomeCard(HomeCard card)
        {
            if (ModelState.IsValid)
            {
                _db.HomeCards.Add(card);
                _db.SaveChanges();
                return RedirectToAction("HomeCards");
            }
            return View(card);
        }

        public IActionResult EditHomeCard(int id)
        {
            var card = _db.HomeCards.Find(id);
            if (card == null) return NotFound();
            return View(card);
        }

        [HttpPost]
        public IActionResult EditHomeCard(HomeCard card)
        {
            if (ModelState.IsValid)
            {
                _db.HomeCards.Update(card);
                _db.SaveChanges();
                return RedirectToAction("HomeCards");
            }
            return View(card);
        }

        public IActionResult DeleteHomeCard(int id)
        {
            var card = _db.HomeCards.Find(id);
            if (card != null)
            {
                _db.HomeCards.Remove(card);
                _db.SaveChanges();
            }
            return RedirectToAction("HomeCards");
        }
        
        public IActionResult ManageSiteContent(string key = "AboutUs")
        {
            var section = _db.SiteContents.FirstOrDefault(s => s.Key == key);
            if (section == null)
            {
                section = new SiteContent { Key = key, Title = GetTitleForKey(key), Content = "" };
                _db.SiteContents.Add(section);
                _db.SaveChanges();
            }
            ViewBag.SectionKeys = new List<SelectListItem>
            {
                new SelectListItem { Value = "AboutUs", Text = "About Us" },
                new SelectListItem { Value = "AboutShelter", Text = "About Pet Shelters" },
                new SelectListItem { Value = "CareGuide", Text = "Pet Care Guide" }
            };
            return View(section);
        }

        [HttpPost]
        public IActionResult ManageSiteContent(SiteContent model)
        {
            var section = _db.SiteContents.FirstOrDefault(s => s.Key == model.Key);
            if (section != null)
            {
                section.Title = model.Title;
                section.Content = model.Content;
                _db.SaveChanges();
            }
            return RedirectToAction("ManageSiteContent", new { key = model.Key });
        }

        private string GetTitleForKey(string key)
        {
            return key switch
            {
                "AboutUs" => "About Us",
                "AboutShelter" => "About Pet Shelters",
                "CareGuide" => "Pet Care Guide",
                _ => key
            };
        }
    }
}