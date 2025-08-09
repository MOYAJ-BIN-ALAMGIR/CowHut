using CowHut.Models;
using Microsoft.EntityFrameworkCore;

namespace CowHut
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        public DbSet<User> users { get; set; }
    }
}
