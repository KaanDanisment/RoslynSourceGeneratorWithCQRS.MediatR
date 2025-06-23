using CqrsDemo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Api.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categorys { get; set; }
    }
}
