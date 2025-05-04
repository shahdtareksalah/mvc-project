using System.ComponentModel.DataAnnotations.Schema;

namespace mvc_pets.Models
{
    public class Donation
    {
        public int DonationId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime DonationDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; } = "Pending";
        public string? AdminNote { get; set; }

        // Use the correct property name:
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser User { get; set; }
        
    }
}
