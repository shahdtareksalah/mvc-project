namespace mvc_pets.Models
{
    public class Pet
    {
        public int PetId { get; set; }
        public string PetName { get; set; }
        public string Species { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string HealthStatus { get; set; }
        public string EmergencyStatus { get; set; }
        public string AdoptionStatus { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<AdoptionRequest> AdoptionRequests { get; set; }
        public ICollection<CaringRequest> CaringRequests { get; set; }
    }
}
