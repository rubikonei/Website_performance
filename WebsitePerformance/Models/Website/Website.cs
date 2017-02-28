using System.Collections.Generic;

namespace WebsitePerformance.Models
{
    public class Website
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public List<Page> Pages { get; set; }
    }
}