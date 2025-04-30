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
        public DbSet<BlogPost> BlogPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تحديد نوع العمود للـ decimal
            modelBuilder.Entity<CaringRequest>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)"); // 18 أرقام، و2 للأرقام بعد الفاصلة العشرية

            modelBuilder.Entity<Donation>()
                .Property(d => d.Amount)
                .HasColumnType("decimal(18,2)"); // 18 أرقام، و2 للأرقام بعد الفاصلة العشرية
        }
    }
}
