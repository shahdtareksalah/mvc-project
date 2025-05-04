using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvc_pets.Models;
using Microsoft.AspNetCore.Identity;



namespace mvc_pets.Data
{



    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Adoptions> Adoptions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CaringRequest> CaringRequests { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<HomeCard> HomeCards { get; set; }
        public DbSet<SiteContent> SiteContents { get; set; }
       
          protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ... your configuration and seeding ...

            modelBuilder.Entity<Adoptions>()
                .HasOne(a => a.Pet)
                .WithMany(p => p.AdoptionRequests)
                .HasForeignKey(a => a.PetId);

            modelBuilder.Entity<Adoptions>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId);
        

        // Example: Configure decimal precision
        modelBuilder.Entity<CaringRequest>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Donation>()
                .Property(d => d.Amount)
                .HasColumnType("decimal(18,2)");

            // Seed SiteContent
            modelBuilder.Entity<SiteContent>().HasData(
                new SiteContent
                {
                    Id = 1,
                    Key = "AboutUs",
                    Title = "Welcome to Pet Care 🐾",
                    Content = "Your trusted partner in pet adoption and care\n\nAbout Us\nWe are dedicated to connecting pets with loving homes and providing comprehensive care guides to ensure their well-being."
                },
                new SiteContent
                {
                    Id = 2,
                    Key = "AboutShelter",
                    Title = "About Pet Shelters",
                    Content = "Pet shelters provide a safe haven for animals in need, offering food, medical care, and a chance for adoption into loving homes. ..."
                },
                new SiteContent
                {
                    Id = 3,
                    Key = "CareGuide",
                    Title = "Pet Care Guide",
                    Content = "Welcome to the Pet Care Guide! Here you'll find essential tips and advice on how to care for your pets and ensure their well-being. ..."
                }
            );

            // Seed HomeCard
            modelBuilder.Entity<HomeCard>().HasData(
                new HomeCard
                {
                    Id = 4561,
                    Title = "Care Guides",
                    Description = "Access expert advice on how to care for your pets.",
                    ButtonText = "Learn More",
                    ButtonLink = "/Home/CareGuide",
                    ButtonClass = "btn-success"
                },
                new HomeCard
                {
                    Id = 4562,
                    Title = "Emergency Help",
                    Description = "Get immediate assistance for lost or injured pets.",
                    ButtonText = "Get Help",
                    ButtonLink = "/BlogPosts/Index",
                    ButtonClass = "btn-danger"
                },
                new HomeCard
                {
                    Id = 4563,
                    Title = "Donation",
                    Description = "Your contribution helps us provide better care for pets in need.",
                    ButtonText = "Donate Now",
                    ButtonLink = "/Donation/Create",
                    ButtonClass = "btn-warning"
                }
            );


        }

    }
}
