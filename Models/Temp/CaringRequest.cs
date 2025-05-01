using System;
using System.Collections.Generic;

namespace mvc_pets.Models.Temp;

public partial class CaringRequest
{
    public int CareReqId { get; set; }

    public string UserId { get; set; } = null!;

    public int PetId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Notes { get; set; } = null!;

    public decimal Price { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public virtual Pet Pet { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
