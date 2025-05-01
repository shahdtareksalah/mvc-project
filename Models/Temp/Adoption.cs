using System;
using System.Collections.Generic;

namespace mvc_pets.Models.Temp;

public partial class Adoption
{
    public int AdoptionRequestId { get; set; }

    public string UserId { get; set; } = null!;

    public int PetId { get; set; }

    public string AdoptReqStatus { get; set; } = null!;

    public virtual Pet Pet { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
