using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc_pets.Models
{
    public class Adoptions
    {
        [Key]
        public int AdoptionRequestId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public int PetId { get; set; }

        [ForeignKey("PetId")]
        public Pet Pet { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        public string AdoptReqStatus { get; set; } = "Pending";

        public string? Notes { get; set; }
    }
}