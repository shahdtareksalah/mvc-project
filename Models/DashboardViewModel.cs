using System.Collections.Generic;

namespace mvc_pets.Models
{
    public class DashboardViewModel
    {
        public int TotalPets { get; set; }
        public int TotalAdoptionRequests { get; set; }
        public int TotalCaringRequests { get; set; }
        public decimal TotalDonations { get; set; }
        public List<Pet> AvailablePets { get; set; }
        public List<AdoptionRequest> RecentAdoptionRequests { get; set; }
        public List<Donation> RecentDonations { get; set; }
    }
} 