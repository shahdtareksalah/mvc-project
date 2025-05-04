using System.Collections.Generic;
using mvc_pets.Models;

namespace mvc_pets.Models
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalPets { get; set; }
        public int TotalAdoptionRequests { get; set; }
        public int TotalCaringRequests { get; set; }
        public decimal TotalDonations { get; set; }
        public int PendingDonations { get; set; }
        public List<Pet> AvailablePets { get; set; }
        public List<Adoptions> RecentAdoptionRequests { get; set; }
        public List<Donation> RecentDonations { get; set; }
    }
} 