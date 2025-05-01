namespace mvc_pets.Models
{
    public class SiteContent
    {
        public int Id { get; set; }
        public string Key { get; set; } // e.g., AboutUs, AboutShelter, CareGuide
        public string Title { get; set; }
        public string Content { get; set; }
    }
}

