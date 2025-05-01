namespace mvc_pets.Models
{
    public class HomeCard
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLink { get; set; }
        public string ButtonClass { get; set; } // e.g., btn-success, btn-danger
    }
}