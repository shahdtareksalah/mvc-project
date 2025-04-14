namespace mvc_pets.Models
{
    public class Pet
    {
        public int PetId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Species { get; set; }
        public string HealthStatus { get; set; }
        public string Description { get; set; }
        public string GregoryStatus { get; set; } // Lost - Found
        public string ImageUrl { get; set; }
        public string AdoptionStatus { get; set; } // Available - Pending
    }

}
