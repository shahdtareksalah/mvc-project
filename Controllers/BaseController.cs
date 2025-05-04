using Microsoft.AspNetCore.Mvc;
using mvc_pets.Data;
using System.Security.Claims;

namespace mvc_pets.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        protected void SetNotifications()
        {
            var userId = User.Identity.IsAuthenticated ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
            if (userId != null)
            {
                var notifications = _context.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.SendDate)
                    .Take(5)
                    .ToList();
                var unreadCount = notifications.Count(n => !n.IsRead);
                ViewBag.Notifications = notifications;
                ViewBag.UnreadCount = unreadCount;
            }
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            SetNotifications();
        }
    }
} 