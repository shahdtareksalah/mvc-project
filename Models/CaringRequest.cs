using mvc_pets.Models;
using System.ComponentModel.DataAnnotations;

public class CaringRequest
{
    [Key]
    public int CareReqId { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public int PetId { get; set; }
    public Pet Pet { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Notes { get; set; }
    public decimal Price { get; set; }
    public string PaymentMethod { get; set; }
}