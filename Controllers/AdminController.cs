using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc_pets.Models;
using mvc_pets.Data;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

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


        public async Task<IActionResult> Dashboard()
        {
            try
            {
                ViewBag.TotalUsers = await _userManager.Users.CountAsync();

                var viewModel = new DashboardViewModel
                {
                    TotalPets = await _context.Pets.CountAsync(),
                    TotalAdoptionRequests = await _context.Adoptions.CountAsync(),
                    TotalCaringRequests = await _context.CaringRequests.CountAsync(),
                    TotalDonations = await _context.Donations.SumAsync(d => d.Amount),
                    PendingDonations = await _context.Donations.CountAsync(d => d.Status == "Pending"),
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Dashboard: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading the dashboard.";
                return View(new DashboardViewModel());
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

        public IActionResult HomeCards()
        {
            return View(_context.HomeCards.ToList());
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
                _context.HomeCards.Add(card);
                _context.SaveChanges();
                return RedirectToAction("HomeCards");
            }
            return View(card);
        }

        public IActionResult EditHomeCard(int id)
        {
            var card = _context.HomeCards.Find(id);
            if (card == null) return NotFound();
            return View(card);
        }

        [HttpPost]
        public IActionResult EditHomeCard(HomeCard card)
        {
            if (ModelState.IsValid)
            {
                _context.HomeCards.Update(card);
                _context.SaveChanges();
                return RedirectToAction("HomeCards");
            }
            return View(card);
        }

        public IActionResult DeleteHomeCard(int id)
        {
            var card = _context.HomeCards.Find(id);
            if (card != null)
            {
                _context.HomeCards.Remove(card);
                _context.SaveChanges();
            }
            return RedirectToAction("HomeCards");
        }

        public IActionResult ManageSiteContent(string key = "AboutUs")
        {
            try
            {
                var section = _context.SiteContents.FirstOrDefault(s => s.Key == key);
                if (section == null)
                {
                    section = new SiteContent 
                    { 
                        Key = key, 
                        Title = GetTitleForKey(key), 
                        Content = GetDefaultContentForKey(key) 
                    };
                    _context.SiteContents.Add(section);
                    _context.SaveChanges();
                }

                ViewBag.SectionKeys = new List<SelectListItem>
                {
                    new SelectListItem { Value = "AboutUs", Text = "About Us" },
                    new SelectListItem { Value = "AboutShelter", Text = "About Pet Shelters" },
                    new SelectListItem { Value = "CareGuide", Text = "Pet Care Guide" }
                };

                return View(section);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ManageSiteContent: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading the content.";
                return View(new SiteContent { Key = key, Title = GetTitleForKey(key) });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ManageSiteContent(SiteContent model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Content))
                {
                    TempData["ErrorMessage"] = "Content cannot be empty.";
                    return RedirectToAction("ManageSiteContent", new { key = model.Key });
                }

                var section = _context.SiteContents.FirstOrDefault(s => s.Key == model.Key);
                if (section == null)
                {
                    section = new SiteContent { Key = model.Key };
                    _context.SiteContents.Add(section);
                }

                section.Title = model.Title;
                section.Content = model.Content;
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Content saved successfully!";
                return RedirectToAction("ManageSiteContent", new { key = model.Key });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving site content: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while saving the content.";
                return RedirectToAction("ManageSiteContent", new { key = model.Key });
            }
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

        private string GetDefaultContentForKey(string key)
        {
            return key switch
            {
                "AboutUs" => "Welcome to our pet adoption platform! We are dedicated to helping pets find their forever homes.",
                "AboutShelter" => "Our pet shelters provide a safe and caring environment for animals in need. Learn more about our facilities and services.",
                "CareGuide" => "Taking care of a pet is a big responsibility. Here are some essential tips for pet care:\n\n1. Regular veterinary check-ups\n2. Proper nutrition\n3. Exercise and playtime\n4. Grooming and hygiene\n5. Training and socialization",
                _ => ""
            };
        }

        // Manage Pets Page
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPet(Pet pet, IFormFile image)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(pet.PetName) || string.IsNullOrEmpty(pet.Species) || pet.Age <= 0)
                {
                    TempData["ErrorMessage"] = "Please fill in all the required fields: Name, Species, and Age.";
                    return View("ManagePets", _context.Pets.ToList());
                }

                // Handle image upload
                if (image != null && image.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }

                    pet.Image = "/images/" + uniqueFileName;
                }
                else
                {
                    // Assign default image if none uploaded
                    pet.Image = "/images/default-pet-image.jpg";
                }

                // Set default values
                pet.CreatedAt = DateTime.Now;
                pet.AdoptionStatus ??= "Available";
                pet.EmergencyStatus ??= "Normal";
                pet.HealthStatus ??= "Good";

                // Save to database
                _context.Pets.Add(pet);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Pet added successfully!";
                return RedirectToAction(nameof(ManagePets));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding pet: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while adding the pet.";
            }

            return View("ManagePets", _context.Pets.ToList());
        }




        // Delete Pet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePet(int id)
        {
            try
            {
                var pet = _context.Pets.Find(id);
                if (pet == null)
                {
                    TempData["ErrorMessage"] = "Pet not found.";
                    return RedirectToAction(nameof(ManagePets));
                }

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
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting pet: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the pet.";
            }

            return RedirectToAction(nameof(ManagePets));
        }

        // Users Management Page
       

    }
}