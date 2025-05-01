using mvc_pets.Models;

public class AdoptionRequest
    {     
        public int AdoptionRequestId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int PetId { get; set; }
        public Pet Pet { get; set; }

        public string AdoptReqStatus { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public string Notes { get; set; }
    }