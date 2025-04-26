using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace mvc_pets.ViewModels
{
    public class RegisterViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string FirstName { get; set; }  // New field

        [Required]
        public string LastName { get; set; }   // New field

        [Required]
        public string Gender { get; set; }     // New field

        [Required]
        public string Address { get; set; }    // New field

        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; } // This will hold the uploaded file
    }
}
