using be_magang.Models;
using Microsoft.EntityFrameworkCore;

namespace be_magang.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<FileRecord> FileRecord { get; set; }
        public DbSet<Profile> Profiles { get; set; }
    }
}
