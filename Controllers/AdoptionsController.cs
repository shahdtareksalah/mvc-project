using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using mvc_pets.Models;
using mvc_pets.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace mvc_pets.Controllers
{
    public class AdoptionsController : BaseController
    {
        private readonly ILogger<AdoptionsController> _logger;

        public AdoptionsController(ApplicationDbContext context, ILogger<AdoptionsController> logger)
            : base(context)
        {
            _logger = logger;
        }

        // Show available pets for adoption
        public async Task<IActionResult> Index()
        {
            var availablePets = await _context.Pets
                .Where(p => p.IsAvailable)
                .ToListAsync();
            return View(availablePets);
        }

        [Authorize]
        public async Task<IActionResult> Request(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null || !pet.IsAvailable)
            {
                TempData["ErrorMessage"] = "Pet not available for adoption.";
                return RedirectToAction("Index", "Pets");
            }
            return View(pet);
        }

        // POST: Adoption/Request/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Request(int id, string Notes)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            var pet = await _context.Pets.FindAsync(id);

            if (pet == null || !pet.IsAvailable)
            {
                TempData["ErrorMessage"] = "Pet not available for adoption.";
                return RedirectToAction("Index", "Pets");
            }

            // Check for existing pending request
            var existingRequest = await _context.Adoptions
                .FirstOrDefaultAsync(r => r.UserId == userId && r.PetId == id && r.AdoptReqStatus == "Pending");

            if (existingRequest != null)
            {
                TempData["ErrorMessage"] = "You already have a pending request for this pet.";
                return RedirectToAction("MyRequests");
            }

            var adoptionRequest = new Adoptions
            {
                UserId = userId,
                PetId = id,
                RequestDate = DateTime.Now,
                AdoptReqStatus = "Pending",
                Notes = Notes
            };

            _context.Adoptions.Add(adoptionRequest);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your adoption request has been submitted successfully.";
            return RedirectToAction("MyRequests");
        }

        // GET: Adoption/MyRequests
        [Authorize]
        public async Task<IActionResult> MyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requests = await _context.Adoptions
                .Include(r => r.Pet)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        // GET: Adoption/AdminRequests
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminRequests()
        {
            var requests = await _context.Adoptions
                .Include(r => r.Pet)
                .Include(r => r.User)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        // POST: Adoption/UpdateRequestStatus
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRequestStatus(int requestId, string status, string adminNotes)
        {
            var request = await _context.Adoptions
                .Include(r => r.Pet)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.AdoptionRequestId == requestId);

            if (request == null)
            {
                return NotFound();
            }

            request.AdoptReqStatus = status;
            request.Notes = adminNotes;

            // Notification logic
            string notificationMessage = "";
            if (status == "Approved")
            {
                request.Pet.IsAvailable = false;
                request.Pet.AdoptionStatus = "Adopted";
                var pickupDate = DateTime.Now.AddDays(3).ToString("dddd, MMMM dd");
                notificationMessage = $"🎉 Congratulations! Your adoption request for <b>{request.Pet.PetName}</b> has been approved! We can't wait to see you and your new friend together. Please visit our shelter after 3 days (on <b>{pickupDate}</b>) to complete the adoption process.";
                if (!string.IsNullOrWhiteSpace(adminNotes))
                    notificationMessage += $"<br/><b>Note from our team:</b> {adminNotes}";
            }
            else if (status == "Rejected")
            {
                notificationMessage = $"😿 We're so sorry, but your adoption request for <b>{request.Pet.PetName}</b> was not approved this time. While this pet is no longer available, we encourage you to explore our other wonderful pets waiting for their forever homes!";
                if (!string.IsNullOrWhiteSpace(adminNotes))
                    notificationMessage += $"<br/><b>Note from our team:</b> {adminNotes}";
            }

            // Send notification to the user
            if (!string.IsNullOrEmpty(request.UserId))
            {
                var notification = new Notification
                {
                    UserId = request.UserId,
                    Content = notificationMessage,
                    SendDate = DateTime.Now,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Request has been {status.ToLower()}.";
            return RedirectToAction(nameof(AdminRequests));
        }
    }
}