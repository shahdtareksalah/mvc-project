using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvc_pets.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using mvc_pets.Data;

namespace mvc_pets.Controllers
{
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DonationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Donation/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Donation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Donation donation)
        {
            // Remove ApplicationUserId and User from model validation
            ModelState.Remove("ApplicationUserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                // تأكد من أنه المستخدم مسجل دخوله
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    // إذا كان المستخدم مش مسجل دخول، عرض رسالة خطأ
                    ModelState.AddModelError(string.Empty, "You must be logged in to make a donation.");
                    return View(donation);
                }

                // ربط التبرع بالمستخدم
                donation.ApplicationUserId = user.Id;
                donation.User = user;
                donation.DonationDate = DateTime.Now;

                try
                {
                    // إضافة التبرع إلى القاعدة
                    _context.Donations.Add(donation);
                    await _context.SaveChangesAsync();

                    // Add notification for the user
                    var notification = new Notification
                    {
                        UserId = user.Id,
                        Content = "Thank you! Your donation was received.",
                        SendDate = DateTime.Now,
                        IsRead = false
                    };
                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();

                    // تحويل المستخدم إلى صفحة النجاح بعد إضافة التبرع
                    return RedirectToAction("Success");
                }
                catch (Exception ex)
                {
                    // إذا حصل خطأ أثناء حفظ التبرع
                    ModelState.AddModelError(string.Empty, $"An error occurred while saving the donation: {ex.Message}");
                }
            }

            // إذا كانت البيانات غير صحيحة، عرض نفس الصفحة مع الأخطاء
            return View(donation);
        }

        // عرض صفحة النجاح
        public IActionResult Success()
        {
            return View();
        }

        // GET: Donation/Index
        public IActionResult Index()
        {
            // استرجاع جميع التبرعات من قاعدة البيانات
            var donations = _context.Donations.ToList();
            return View(donations);  // عرض التبرعات في الـ View
        }

        // GET: Donation/Manage
        [Authorize(Roles = "Admin")]
        public IActionResult Manage()
        {
            var donations = _context.Donations
                .OrderByDescending(d => d.DonationDate)
                .ToList();
            return View(donations);
        }

        // POST: Donation/Approve/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
            {
                return NotFound();
            }

            donation.Status = "Approved";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Manage));
        }

        // POST: Donation/Reject/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
            {
                return NotFound();
            }

            // Notify user before deleting
            if (!string.IsNullOrEmpty(donation.ApplicationUserId))
            {
                var notification = new Notification
                {
                    UserId = donation.ApplicationUserId,
                    Content = "Your donation was rejected and removed. Please contact support for more information.",
                    SendDate = DateTime.Now,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            _context.Donations.Remove(donation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Manage));
        }
    }
}