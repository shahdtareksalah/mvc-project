using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace mvc_pets.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<AdoptionRequest> Adoptions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CaringRequest> CaringRequests { get; set; }

    }
}
