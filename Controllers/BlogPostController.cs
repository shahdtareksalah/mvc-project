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

        // ADMIN: List all blog posts with Edit/Delete
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AdminIndex()
        {
            var blogs = await _context.BlogPosts.Include(b => b.User).OrderByDescending(b => b.CreatedAt).ToListAsync();
            return View(blogs);
        }

        // ADMIN: Edit blog post
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var blog = await _context.BlogPosts.FindAsync(id);
            if (blog == null) return NotFound();
            return View(blog);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogPost blogPost, IFormFile? ImageFile)
        {
            Console.WriteLine("Edit POST called for BlogPost Id: " + id);

            var existingBlog = await _context.BlogPosts.FindAsync(id);
            if (existingBlog == null)
            {
                Console.WriteLine("Blog not found for Id: " + id);
                return NotFound();
            }

            // Remove unnecessary fields from ModelState
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("CreatedAt");

            // Check if an image is required
            if (string.IsNullOrEmpty(existingBlog.Image) && (ImageFile == null || ImageFile.Length == 0))
            {
                ModelState.AddModelError("Image", "The Image field is required.");
            }
            else if (!string.IsNullOrEmpty(existingBlog.Image))
            {
                ModelState.Remove("Image");
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid:");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }
                return View(blogPost);
            }

            // Update blog post properties
            existingBlog.Title = blogPost.Title;
            existingBlog.Content = blogPost.Content;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var safeFileName = WebUtility.HtmlEncode(Path.GetFileName(ImageFile.FileName));
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + safeFileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                existingBlog.Image = $"/uploads/{uniqueFileName}";
                Console.WriteLine("Image updated: " + existingBlog.Image);
            }
            else
            {
                existingBlog.Image = existingBlog.Image; // Retain existing image
            }

            Console.WriteLine("Saving changes to database...");
            _context.BlogPosts.Update(existingBlog);
            await _context.SaveChangesAsync();
            Console.WriteLine("Blog updated successfully!");

            TempData["SuccessMessage"] = "Blog updated successfully!";
            return RedirectToAction("AdminIndex");
        }
        // ADMIN: Delete blog post
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.BlogPosts.FindAsync(id);
            if (blog == null) return NotFound();
            return View(blog);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.BlogPosts.FindAsync(id);
            if (blog == null) return NotFound();
            _context.BlogPosts.Remove(blog);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Blog deleted successfully!";
            return RedirectToAction("AdminIndex");
        }

        // Restrict Create to Admins only
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to create a blog post.";
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content")] BlogPost blogPost, IFormFile? ImageFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(blogPost);
            }
            blogPost.UserId = user.Id;
            blogPost.CreatedAt = DateTime.UtcNow;
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                var safeFileName = WebUtility.HtmlEncode(Path.GetFileName(ImageFile.FileName));
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + safeFileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                blogPost.Image = $"/uploads/{uniqueFileName}";
            }
            else
            {
                blogPost.Image = null;
            }
            ModelState.Remove("UserId");
            ModelState.Remove("Image");
            ModelState.Remove("User");
            if (!ModelState.IsValid) return View(blogPost);
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Blog created successfully!";
            return RedirectToAction("AdminIndex");
        }
    }
}