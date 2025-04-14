using mvc_pets.Models;

public class CaringRequest
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public Pet Pet { get; set; }
    public string Usual { get; set; }
    public DateTime EndDate { get; set; }
    public string PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
}
