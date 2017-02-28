using System.Data.Entity;

namespace WebsitePerformance.Models
{
    public class WebsiteContextInitializer : DropCreateDatabaseAlways<WebsiteContext>
    {
        protected override void Seed(WebsiteContext context)
        {
            base.Seed(context);
        }
    }
}