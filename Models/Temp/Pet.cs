using System;
using System.Collections.Generic;

namespace mvc_pets.Models.Temp;

public partial class Pet
{
    public int PetId { get; set; }

    public string PetName { get; set; } = null!;

    public string Species { get; set; } = null!;

    public int Age { get; set; }

    public string Gender { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Image { get; set; } = null!;

    public string HealthStatus { get; set; } = null!;

    public string EmergencyStatus { get; set; } = null!;

    public string AdoptionStatus { get; set; } = null!;

    public virtual ICollection<Adoption> Adoptions { get; } = new List<Adoption>();

    public virtual ICollection<CaringRequest> CaringRequests { get; } = new List<CaringRequest>();
}
