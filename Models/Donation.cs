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
        public string? AdminNote { get; set; } // ملاحظات المشرف
    }
}