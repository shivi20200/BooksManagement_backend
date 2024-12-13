using BookAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; } // DbSet for Books
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring the Book entity
            modelBuilder.Entity<Book>(entity =>
            {
                // Configure the primary key
                entity.HasKey(b => b.Id);

                // Configure properties
                entity.Property(b => b.Title)
                      .IsRequired() // Make the Title property required
                      .HasMaxLength(255); // Set maximum length

                entity.Property(b => b.Author)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(b => b.ISBN)
                      .IsRequired()
                      .HasMaxLength(13); // ISBN max length (13 digits for standard format)

                entity.Property(b => b.PublicationYear)
                      .IsRequired();

                // Ensure ISBN is unique
                entity.HasIndex(b => b.ISBN).IsUnique();
            });
        }
    }      
}
