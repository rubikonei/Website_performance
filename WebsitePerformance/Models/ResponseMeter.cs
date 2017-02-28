using System.Diagnostics;
using System.Net;

namespace WebsitePerformance.Models
{
    public class ResponseTimeMeter
    {
        public long MeasureResponseTime(string uri)
        {
            WebRequest request = WebRequest.Create(uri);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            WebResponse response = request.GetResponse();
            timer.Stop();
            long timeTaken = timer.ElapsedMilliseconds;
            return timeTaken;
        }
    }
}