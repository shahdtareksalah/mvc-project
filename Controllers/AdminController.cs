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
    public class AdminController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<AdminController> logger)
            : base(context)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var viewModel = new DashboardViewModel
                {
                    TotalUsers = await _userManager.Users.CountAsync(),
                    TotalPets = await _context.Pets.CountAsync(),
                    TotalAdoptionRequests = await _context.Adoptions.CountAsync(),
                    TotalCaringRequests = await _context.CaringRequests.CountAsync(),
                    TotalDonations = await _context.Donations.SumAsync(d => d.Amount),
                    PendingDonations = await _context.Donations.CountAsync(d => d.Status == "Pending")
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
            var users = await _userManager.Users.ToListAsync();
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
            try
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
                TempData["SuccessMessage"] = $"User {(isAdmin ? "removed from" : "added to")} admin role successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error toggling admin role: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while updating user role.";
            }

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
        public async Task<IActionResult> AddHomeCard(HomeCard card)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.HomeCards.Add(card);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Home card added successfully!";
                    return RedirectToAction("HomeCards");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error adding home card: {ex.Message}");
                    TempData["ErrorMessage"] = "An error occurred while adding the home card.";
                }
            }
            return View(card);
        }

        public async Task<IActionResult> EditHomeCard(int id)
        {
            var card = await _context.HomeCards.FindAsync(id);
            if (card == null) return NotFound();
            return View(card);
        }

        [HttpPost]
        public async Task<IActionResult> EditHomeCard(HomeCard card)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.HomeCards.Update(card);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Home card updated successfully!";
                    return RedirectToAction("HomeCards");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error updating home card: {ex.Message}");
                    TempData["ErrorMessage"] = "An error occurred while updating the home card.";
                }
            }
            return View(card);
        }

        public async Task<IActionResult> DeleteHomeCard(int id)
        {
            try
            {
                var card = await _context.HomeCards.FindAsync(id);
                if (card != null)
                {
                    _context.HomeCards.Remove(card);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Home card deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting home card: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the home card.";
            }
            return RedirectToAction("HomeCards");
        }

        public async Task<IActionResult> ManageSiteContent(string key = "AboutUs")
        {
            try
            {
                var section = await _context.SiteContents.FirstOrDefaultAsync(s => s.Key == key);
                if (section == null)
                {
                    section = new SiteContent 
                    { 
                        Key = key, 
                        Title = GetTitleForKey(key), 
                        Content = GetDefaultContentForKey(key) 
                    };
                    _context.SiteContents.Add(section);
                    await _context.SaveChangesAsync();
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
        public async Task<IActionResult> ManageSiteContent(SiteContent model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Content))
                {
                    TempData["ErrorMessage"] = "Content cannot be empty.";
                    return RedirectToAction("ManageSiteContent", new { key = model.Key });
                }

                var section = await _context.SiteContents.FirstOrDefaultAsync(s => s.Key == model.Key);
                if (section == null)
                {
                    section = new SiteContent { Key = model.Key };
                    _context.SiteContents.Add(section);
                }

                section.Title = model.Title;
                section.Content = model.Content;
                await _context.SaveChangesAsync();

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

        public async Task<IActionResult> ManagePets()
        {
            try
            {
                var pets = await _context.Pets.ToListAsync();
                return View(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ManagePets: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading pets.";
                return View(new List<Pet>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPet(Pet pet, IFormFile image)
        {
            try
            {
                if (string.IsNullOrEmpty(pet.PetName) || string.IsNullOrEmpty(pet.Species) || pet.Age <= 0)
                {
                    TempData["ErrorMessage"] = "Please fill in all the required fields: Name, Species, and Age.";
                    return View("ManagePets", await _context.Pets.ToListAsync());
                }

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
                        await image.CopyToAsync(stream);
                    }

                    pet.Image = "/images/" + uniqueFileName;
                }
                else
                {
                    pet.Image = "/images/default-pet-image.jpg";
                }

                pet.CreatedAt = DateTime.Now;
                pet.AdoptionStatus ??= "Available";
                pet.EmergencyStatus ??= "Normal";
                pet.HealthStatus ??= "Good";

                _context.Pets.Add(pet);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Pet added successfully!";
                return RedirectToAction(nameof(ManagePets));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding pet: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while adding the pet.";
            }

            return View("ManagePets", await _context.Pets.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePet(int id)
        {
            try
            {
                var pet = await _context.Pets.FindAsync(id);
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
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Pet deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting pet: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the pet.";
            }

            return RedirectToAction(nameof(ManagePets));
        }

        public async Task<IActionResult> EditPet(int id)
        {
            try
            {
                var pet = await _context.Pets.FindAsync(id);
                if (pet == null)
                {
                    TempData["ErrorMessage"] = "Pet not found.";
                    return RedirectToAction(nameof(ManagePets));
                }
                return View(pet);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading pet for edit: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while loading the pet.";
                return RedirectToAction(nameof(ManagePets));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPet(Pet pet, IFormFile image)
        {
            try
            {
                if (string.IsNullOrEmpty(pet.PetName) || string.IsNullOrEmpty(pet.Species) || pet.Age <= 0)
                {
                    TempData["ErrorMessage"] = "Please fill in all the required fields: Name, Species, and Age.";
                    return View(pet);
                }

                var existingPet = await _context.Pets.FindAsync(pet.PetId);
                if (existingPet == null)
                {
                    TempData["ErrorMessage"] = "Pet not found.";
                    return RedirectToAction(nameof(ManagePets));
                }

                // Handle image upload
                if (image != null && image.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Delete old image if it exists and is not the default image
                    if (!string.IsNullOrEmpty(existingPet.Image) && existingPet.Image != "/images/default-pet-image.jpg")
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingPet.Image.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    existingPet.Image = "/images/" + uniqueFileName;
                }

                // Update other properties
                existingPet.PetName = pet.PetName;
                existingPet.Species = pet.Species;
                existingPet.Age = pet.Age;
                
                existingPet.Description = pet.Description;
                existingPet.AdoptionStatus = pet.AdoptionStatus ?? "Available";
                existingPet.EmergencyStatus = pet.EmergencyStatus ?? "Normal";
                existingPet.HealthStatus = pet.HealthStatus ?? "Good";

                _context.Pets.Update(existingPet);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Pet updated successfully!";
                return RedirectToAction(nameof(ManagePets));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating pet: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while updating the pet.";
                return View(pet);
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
    }
}