using System.Data.Entity;

namespace WebsitePerformance.Models
{
    public class WebsiteContext : DbContext
    {
        static WebsiteContext()
        {
            Database.SetInitializer<WebsiteContext>(new WebsiteContextInitializer());
        }
        public DbSet<Website> Websites { get; set; }
        public DbSet<Page> Pages { get; set; }
    }
}