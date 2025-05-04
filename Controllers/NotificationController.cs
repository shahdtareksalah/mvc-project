using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_pets.Data;
using mvc_pets.Models;
using System.Security.Claims;
using System.Linq;

namespace mvc_pets.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // List notifications for the current user
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifications = _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SendDate)
                .ToList();

            // Mark all as read
            foreach (var n in notifications.Where(n => !n.IsRead))
            {
                n.IsRead = true;
            }
            _context.SaveChanges();

            return View(notifications);
        }

        // Mark a notification as read
        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            var notification = _context.Notifications.Find(id);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.SaveChanges();
            }
            return Ok();
        }
    }
}