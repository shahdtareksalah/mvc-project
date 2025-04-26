using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using mvc_pets.Models;

namespace mvc_pets.Controllers
{
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var availablePets = _context.Pets.Where(p => p.AdoptionStatus == "Available").ToList();
            return View(availablePets);
        }

        public IActionResult Details(int id)
        {
            var pet = _context.Pets.FirstOrDefault(p => p.PetId == id);
            if (pet == null)
                return NotFound();

            return View(pet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
