using Microsoft.AspNetCore.Identity;
using mvc_pets.Models;
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsAdmin { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LastLogin { get; set; }
    public string Address { get; set; }
    public string Gender { get; set; }
    public string ProfilePicture { get; set; }  // This is the image path
    public override string PhoneNumber { get; set; }  // Already part of IdentityUser


    public ICollection<AdoptionRequest> AdoptionRequests { get; set; }
    public ICollection<CaringRequest> CaringRequests { get; set; }
    public ICollection<Donation> Donations { get; set; }
    public ICollection<Notification> Notifications { get; set; }
}


