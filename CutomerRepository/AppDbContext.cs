using Microsoft.EntityFrameworkCore;
namespace CutomerRepository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Login> Logins { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Server=DESKTOP-S533LLH\\SQLEXPRESS;Database=CustomerDb;Integrated Security=True;");
        //    }
        //}
    }

}