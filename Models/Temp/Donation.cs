using System;
using System.Collections.Generic;

namespace mvc_pets.Models.Temp;

public partial class Donation
{
    public int DonationId { get; set; }

    public string UserId { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Type { get; set; } = null!;

    public DateTime Date { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
