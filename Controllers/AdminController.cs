using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_pets.Models;
using mvc_pets.Data;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.IO;  // Add this for File operations

namespace mvc_pets.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Data.ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(UserManager<ApplicationUser> userManager, Data.ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public IActionResult Dashboard()
        {
            try
            {
                var viewModel = new DashboardViewModel
                {
                    TotalPets = _context.Pets.Count(),
                    TotalAdoptionRequests = _context.AdoptionRequests.Count(),
                    TotalCaringRequests = _context.CaringRequests.Count(),
                    TotalDonations = _context.Donations.Sum(d => d.Amount),
                    AvailablePets = _context.Pets
                        .Where(p => p.AdoptionStatus == "Available")
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
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError($"Error in Dashboard: {ex.Message}");
                // Return a view with error message
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
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
            try
            {
                var pets = _context.Pets.ToList();
                return View(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ManagePets: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }

        // ????? Pet ????
        [HttpPost]
        public IActionResult AddPet(Pet pet, IFormFile image)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (image != null && image.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }
                        pet.Image = "/images/" + uniqueFileName;
                    }

                    _context.Pets.Add(pet);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Pet added successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Please fill all required fields correctly.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding pet: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while adding the pet.";
            }

            return RedirectToAction(nameof(ManagePets));
        }

        // ??? Pet
        [HttpPost]
        public IActionResult DeletePet(int id)
        {
            try
            {
                var pet = _context.Pets.Find(id);
                if (pet != null)
                {
                    // Delete the image file if it exists
                    if (!string.IsNullOrEmpty(pet.Image))
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pet.Image.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    _context.Pets.Remove(pet);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Pet deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Pet not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting pet: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the pet.";
            }

            return RedirectToAction(nameof(ManagePets));
        }
    }
}
