using APass.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace APass.Infrastructure
{
    public class PasswordManagerContext : DbContext
    {
        public PasswordManagerContext(DbContextOptions<PasswordManagerContext> options)
        : base(options)
        {
        }
        public DbSet<PasswordEntry> PasswordEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "passwordManager.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PasswordEntry>()
                .HasKey(pe => pe.ID);
            modelBuilder.Entity<PasswordEntry>()
                .Property(pe => pe.ID)
                .ValueGeneratedOnAdd(); // Configure ID to auto-generate
        }
    }
}
