using mvc_pets.Models;

public class Adoption
{
    public int KeyId { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int PetId { get; set; }
    public Pet Pet { get; set; }
    public string Status { get; set; } // Accepted / Rejected / Pending
    public string Abstract { get; set; }
    public string Type { get; set; } // e.g., temporary, permanent
    public DateTime Time { get; set; }
}
