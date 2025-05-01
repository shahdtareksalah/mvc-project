using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using mvc_pets.Models;
using System.IO;
using System.Threading.Tasks;
using mvc_pets.ViewModels;
using Microsoft.AspNetCore.Authentication;
using System.Net;

public class ProfileController : Controller
{
    

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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

    [HttpGet]
    public async Task<IActionResult> Edit()
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
    public async Task<IActionResult> Edit(UserProfileViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;
        user.Address = model.Address;
        user.Gender = model.Gender;
        // ProfilePicture is updated separately

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user); // <--- Add this line!
            TempData["SuccessMessage"] = "Profile picture updated successfully";
            return RedirectToAction("index");
        }
        TempData["ErrorMessage"] = "Failed to update profile.";
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();
        if (!User.IsInRole("Admin")) return Forbid();

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        TempData["ErrorMessage"] = "Failed to delete profile.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfilePicture(IFormFile profilePicture)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        if (profilePicture != null && profilePicture.Length > 0)
        {
            var uploadsFolder = Path.Combine("wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePicture.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePicture.CopyToAsync(stream);
            }

            // Optionally: Delete old image file if you want to clean up
            // if (!string.IsNullOrEmpty(user.ProfilePicture))
            // {
            //     var oldPath = Path.Combine("wwwroot", user.ProfilePicture.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            //     if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            // }

            user.ProfilePicture = $"/uploads/{uniqueFileName}";
            await _userManager.UpdateAsync(user);
        }

        await RefreshSignInAsync(user);

        TempData["SuccessMessage"] = "Profile picture updated successfully!";
        return RedirectToAction("Index");
    }

    private async Task RefreshSignInAsync(ApplicationUser user)
    {
        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);

        // Sign out the current user
        await _signInManager.SignOutAsync();

        // Sign in again with the updated claims
        await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal);
    }
}
