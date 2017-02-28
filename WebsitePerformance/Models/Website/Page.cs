namespace WebsitePerformance.Models
{
    public class Page
    {
        public int Id { get; set; }
        public string Uri { get; set; }
        public long ResponseTime { get; set; }
        public int? WebsiteId { get; set; }
        public Website Website { get; set; }
    }
}