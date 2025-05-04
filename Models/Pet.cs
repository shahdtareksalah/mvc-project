namespace mvc_pets.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Pet
    {
        public int PetId { get; set; }

        [Required]
        [StringLength(100)]
        public string PetName { get; set; }

        [Required]
        [StringLength(50)]
        public string Species { get; set; }

        [Range(0, 100)]
        public int Age { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public string Image { get; set; }
        public string HealthStatus { get; set; }
        public string EmergencyStatus { get; set; }
        public string AdoptionStatus { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Adoptions> AdoptionRequests { get; set; } = new List<Adoptions>();
        public ICollection<CaringRequest> CaringRequests { get; set; } = new List<CaringRequest>();
    }
}
