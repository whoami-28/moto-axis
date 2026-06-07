using Microsoft.EntityFrameworkCore;

namespace FarshGenerator.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MotorcycleSpec> MotorcycleSpecs { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<ListingImage> ListingImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Listing>()
                .HasOne(l => l.Spec)
                .WithOne(s => s.Listing)
                .HasForeignKey<Listing>(l => l.SpecId);

            modelBuilder.Entity<Listing>()
                .Property(l => l.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<MotorcycleSpec>()
                .Property(m => m.EnginePower)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<User>()
                .Property(u => u.Rating)
                .HasColumnType("decimal(18,2)");
        }
    }
}