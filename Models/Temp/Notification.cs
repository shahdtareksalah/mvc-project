using System;
using System.Collections.Generic;

namespace mvc_pets.Models.Temp;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string UserId { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime SentDate { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
