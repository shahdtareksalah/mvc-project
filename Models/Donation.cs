namespace mvc_pets.Models
{
    public class Donation
    {
        public int DomainId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public DateTime Date { get; set; }
    }

}
