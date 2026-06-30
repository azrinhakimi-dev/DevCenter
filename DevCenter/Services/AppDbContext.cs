using DevCenter.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DevCenter.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DevCommand> DevCommands { get; set; }
        public DbSet<Snippet> Snippets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (options.IsConfigured) return;

            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DevCenter");

            Directory.CreateDirectory(folder);

            options.UseSqlite($"Data Source={Path.Combine(folder, "devcenter.db")}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DevCommand>().Property(c => c.Description).HasDefaultValue("");
        }
    }
}