using mvc_pets.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net;

namespace mvc_pets.Controllers
{
    [Authorize] // Ensure only authenticated users can access this controller
    public class BlogPostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogPostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /BlogPost/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("Index action called.");

            try
            {
                // Retrieve all blog posts, ordered by creation date (newest first)
                var blogs = await _context.BlogPosts
                    .Include(b => b.User) // Include the related User data (optional, if needed)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                Console.WriteLine($"Loaded {blogs.Count} blog posts successfully.");
                return View(blogs); // Pass the list of blog posts to the view
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading blog posts: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to load blog posts. Please try again later.";
                return View(new List<BlogPost>()); // Return an empty list in case of an error
            }
        }

        // GET: /BlogPost/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "You must be logged in to create a blog post.";
                    return RedirectToAction("Login", "Account");
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error initializing form: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: /BlogPost/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Content")] BlogPost blogPost,
            IFormFile? ImageFile) // Allow null for ImageFile
        {
            Console.WriteLine("Processing Create POST request...");

            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(blogPost);
            }

            Console.WriteLine($"User details: FirstName={user.FirstName}, LastName={user.LastName}, Email={user.Email}");

            // Set required fields programmatically
            blogPost.UserId = user.Id; // Required field
            blogPost.CreatedAt = DateTime.UtcNow; // Required field

            // Handle image upload (optional)
            if (ImageFile != null && ImageFile.Length > 0)
            {
                Console.WriteLine($"Uploading file: {ImageFile.FileName}");

                var uploadsFolder = Path.Combine("wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Console.WriteLine($"Created directory: {uploadsFolder}");
                }

                var safeFileName = WebUtility.HtmlEncode(Path.GetFileName(ImageFile.FileName));
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + safeFileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                blogPost.Image = $"/uploads/{uniqueFileName}";
                Console.WriteLine($"Image saved to: {blogPost.Image}");
            }
            else
            {
                blogPost.Image = null; // Optional field
                Console.WriteLine("No image uploaded.");
            }

            // Manually update ModelState for fields set programmatically
            ModelState.Remove("UserId"); // Remove existing validation errors for UserId
            ModelState.Remove("Image");  // Remove existing validation errors for Image
            ModelState.Remove("User");   // Remove existing validation errors for User

            // Re-validate the model
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model validation failed.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return View(blogPost);
            }

            try
            {
                Console.WriteLine("Adding blog post to database...");

                _context.BlogPosts.Add(blogPost);
                await _context.SaveChangesAsync();

                Console.WriteLine("Blog post saved successfully.");

                TempData["SuccessMessage"] = "Blog created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving blog post: {ex.Message}");
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return View(blogPost);
            }
        }
    }
}