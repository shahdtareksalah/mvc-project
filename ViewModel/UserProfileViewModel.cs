using Microsoft.AspNetCore.Http;
using mvc_pets.Models;
using System.Collections.Generic;

namespace mvc_pets.ViewModels
{
    public class UserProfileViewModel
    {
        // User's first and last name
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // User's email and phone number
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // User's address and gender
        public string Address { get; set; }
        public string Gender { get; set; }

        // Full name computed from first and last name
        public string FullName => $"{FirstName} {LastName}";

        // The URL/path to the current profile picture
        public string ProfilePicture { get; set; }

        // For displaying the current profile picture
        public string ExistingProfilePicture { get; set; }

        // For uploading a new profile picture
        public IFormFile NewProfilePicture { get; set; }

        // List of adoption requests associated with the user
        public List<AdoptionRequest> AdoptionRequests { get; set; } = new();
    }
}
