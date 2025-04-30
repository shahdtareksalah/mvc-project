namespace mvc_pets.Models
{
    public class Donation
    {
        public int DonationId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime DonationDate { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }
    }

}
