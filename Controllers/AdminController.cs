using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_pets.Models;
using mvc_pets.Data;
using System.IO;
using System.Linq;

namespace mvc_pets.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Data.ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, Data.ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var viewModel = new DashboardViewModel
            {
                TotalPets = _context.Pets.Count(),
                TotalAdoptionRequests = _context.AdoptionRequests.Count(),
                TotalCaringRequests = _context.CaringRequests.Count(),
                TotalDonations = _context.Donations.Sum(d => d.Amount),
                AvailablePets = _context.Pets
                    .Where(p => p.IsAvailable)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .ToList(),
                RecentAdoptionRequests = _context.AdoptionRequests
                    .Include(r => r.Pet)
                    .Include(r => r.User)
                    .OrderByDescending(r => r.RequestDate)
                    .Take(5)
                    .ToList(),
                RecentDonations = _context.Donations
                    .OrderByDescending(d => d.DonationDate)
                    .Take(5)
                    .ToList()
            };

            return View(viewModel);
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

        // ??? ???? ??? Pets
        public IActionResult ManagePets()
        {
            var pets = _context.Pets.ToList();
            return View(pets);
        }

        // ????? Pet ????
        [HttpPost]
        public IActionResult AddPet(Pet pet, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                // ??? ?????? ?? ??????
                if (image != null && image.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", image.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }
                    pet.Image = "/images/" + image.FileName;
                }

                // ????? ??????? ?????? ??? ????? ????????
                _context.Pets.Add(pet);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(ManagePets));
        }

        // ??? Pet
        [HttpPost]
        public IActionResult DeletePet(int id)
        {
            var pet = _context.Pets.Find(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(ManagePets));
        }
    }
}
