using System;
using System.Collections.Generic;

namespace mvc_pets.Models.Temp;

public partial class AspNetUser
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public string Address { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string ProfilePicture { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public virtual ICollection<Adoption> Adoptions { get; } = new List<Adoption>();

    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; } = new List<AspNetUserClaim>();

    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; } = new List<AspNetUserLogin>();

    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; } = new List<AspNetUserToken>();

    public virtual ICollection<BlogPost> BlogPosts { get; } = new List<BlogPost>();

    public virtual ICollection<CaringRequest> CaringRequests { get; } = new List<CaringRequest>();

    public virtual ICollection<Donation> Donations { get; } = new List<Donation>();

    public virtual ICollection<Notification> Notifications { get; } = new List<Notification>();

    public virtual ICollection<AspNetRole> Roles { get; } = new List<AspNetRole>();
}
