using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using mvc_pets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace mvc_pets.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PetsController> _logger;

        public PetsController(ApplicationDbContext context, ILogger<PetsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            var availablePets = _context.Pets.Where(p => p.AdoptionStatus == "Available").ToList();
            return View(availablePets);
        }

        [Route("ManagePets")]
        public IActionResult ManagePets()
        {
            try
            {
                _logger.LogInformation("Fetching all pets from database");
                var allPets = _context.Pets.ToList();
                _logger.LogInformation($"Found {allPets.Count} pets");
                return View("~/Views/Admin/ManagePets.cshtml", allPets);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching pets: {ex.Message}");
                throw;
            }
        }

        public IActionResult Details(int id)
        {
            var pet = _context.Pets.FirstOrDefault(p => p.PetId == id);
            if (pet == null)
                return NotFound();

            return View(pet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("AddPet")]
        public IActionResult AddPet([FromForm] Pet pet)
        {
            try
            {
                _logger.LogInformation($"Received pet data: Name={pet.PetName}, Species={pet.Species}, Age={pet.Age}");

                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Model state is valid, proceeding to add pet");
                    
                    // Initialize collections
                    pet.AdoptionRequests = new List<AdoptionRequest>();
                    pet.CaringRequests = new List<CaringRequest>();
                    
                    // Set default values if needed
                    if (string.IsNullOrEmpty(pet.AdoptionStatus))
                        pet.AdoptionStatus = "Available";
                    
                    if (string.IsNullOrEmpty(pet.EmergencyStatus))
                        pet.EmergencyStatus = "Normal";
                    
                    if (string.IsNullOrEmpty(pet.HealthStatus))
                        pet.HealthStatus = "Good";

                    _context.Pets.Add(pet);
                    _context.SaveChanges();
                    
                    _logger.LogInformation($"Successfully added pet with ID: {pet.PetId}");
                    TempData["SuccessMessage"] = "Pet added successfully!";
                    return RedirectToAction(nameof(ManagePets));
                }
                
                _logger.LogWarning("Invalid model state when adding pet");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Validation error: {error.ErrorMessage}");
                    TempData["ErrorMessage"] = error.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding pet: {ex.Message}");
                TempData["ErrorMessage"] = "Error adding pet: " + ex.Message;
            }
            
            return View("~/Views/Admin/ManagePets.cshtml", _context.Pets.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeletePet")]
        public IActionResult DeletePet(int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete pet with ID: {id}");
                var pet = _context.Pets.Find(id);
                if (pet == null)
                {
                    _logger.LogWarning($"Pet with ID {id} not found for deletion");
                    return NotFound();
                }

                _context.Pets.Remove(pet);
                _context.SaveChanges();
                _logger.LogInformation($"Successfully deleted pet with ID: {id}");
                TempData["SuccessMessage"] = "Pet deleted successfully!";
                return RedirectToAction(nameof(ManagePets));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting pet: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting pet: " + ex.Message;
                return RedirectToAction(nameof(ManagePets));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
