using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using mvc_pets.Models;
using System.IO;
using System.Threading.Tasks;
using mvc_pets.ViewModels;

public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var model = new UserProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            Gender = user.Gender,
            ProfilePicture = user.ProfilePicture
        };

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> UpdateProfilePicture(IFormFile NewProfilePicture)
    {
        if (NewProfilePicture != null && NewProfilePicture.Length > 0)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction("Profile");
            }

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
            Directory.CreateDirectory(uploadsPath);

            // Generate unique filename to prevent conflicts
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(NewProfilePicture.FileName);
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await NewProfilePicture.CopyToAsync(stream);
            }

            user.ProfilePicture = "/images/profiles/" + uniqueFileName;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile picture updated successfully";
                return RedirectToAction("index");
            }
        }

        TempData["ErrorMessage"] = "Upload failed";
        return RedirectToAction("Profile");
    }
}
