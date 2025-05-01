using mvc_pets.Models;
using System.Collections.Generic;

namespace mvc_pets.ViewModel
{
    public class HomeViewModel
    {
        public SiteContent AboutUs { get; set; }
        public SiteContent AboutShelter { get; set; }
        public SiteContent CareGuide { get; set; }
        public List<HomeCard> HomeCards { get; set; }
    }
} 