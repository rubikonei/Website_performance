using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebsitePerformance.Models;

namespace WebsitePerformance.Controllers
{
    public class HomeController : Controller
    {
        private WebsiteContext db = new WebsiteContext();

        public ActionResult Index()
        {
            List<SelectListItem> websitesList = new List<SelectListItem>();
            foreach (Website website in db.Websites)
            {
                websitesList.Add(new SelectListItem { Text = website.Url, Value = website.Id.ToString() });
            }
            ViewBag.WebsitesList = websitesList;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult WebsitePerformance(Uri uri)
        {
            string url = uri.GetLeftPart(UriPartial.Authority);
            foreach (Website w in db.Websites)
            {
                if (w.Url == url)
                {
                    return RedirectToAction("History", new { websiteId = w.Id });
                }
            }
            Website website = new Website
            {
                Url = url
            };
            db.Websites.Add(website);
            db.SaveChanges();

            List<string> listUri = new SitemapGetter(uri).UriList;
            ResponseTimeMeter rtm = new ResponseTimeMeter();
            List<Page> pagesList = new List<Page>();
            foreach (string str in listUri)
            {
                try
                {
                    Page page = new Page
                    {
                        Uri = str,
                        ResponseTime = rtm.MeasureResponseTime(str),
                        Website = website
                    };
                    pagesList.Add(page);
                }
                catch (WebException ex) { } 
            }
            db.Pages.AddRange(pagesList);
            db.SaveChanges();

            Highcharts chart = HighchartDraw(website.Id);

            WebsiteViewer websiteViewer = new WebsiteViewer
            {
                Website = website,
                Highchart = chart
            };
            return View(websiteViewer);
        }

        public ActionResult History(int websiteId)
        {
            Website website = new Website
            {
                Id = websiteId,
                Pages = (from p in db.Pages where p.WebsiteId == websiteId select p).ToList(),
                Url = db.Websites.First(w => w.Id == websiteId).Url
            };

            Highcharts chart = HighchartDraw(websiteId);

            WebsiteViewer websiteViewer = new WebsiteViewer
            {
                Website = website,
                Highchart = chart
            };
            return View("~/Views/Home/WebsitePerformance.cshtml", websiteViewer);
        }

        private Highcharts HighchartDraw(int websiteId)
        {
            var uris = (from p in db.Pages where p.WebsiteId == websiteId select p.Uri).ToArray();
            var times = (from p in db.Pages where p.WebsiteId == websiteId select p.ResponseTime).ToArray().Cast<object>().ToArray();

            Website website = new Website
            {
                Id = websiteId,
                Pages = (from p in db.Pages where p.WebsiteId == websiteId select p).ToList(),
                Url = db.Websites.First(w => w.Id == websiteId).Url
            };

            Highcharts chart = new Highcharts("chart").
                InitChart(new Chart
                {
                    DefaultSeriesType = ChartTypes.Line
                }).
                SetTitle(new Title
                {
                    Text = string.Format("Website performance for: {0}", website.Url)
                }).
                SetXAxis(new XAxis
                {
                    Categories = uris,
                    Title = new XAxisTitle { Text = "Pages " },
                    Labels = new XAxisLabels { Enabled = false }
                }).
                SetYAxis(new YAxis
                {
                    Title = new YAxisTitle { Text = "Response Time, ms" }
                }).
                SetSeries(new Series
                {
                    Name = website.Url,
                    Data = new Data(times)
                }).
                SetTooltip(new Tooltip
                {
                    Formatter = @"function() {
                                return '<b>'+ this.series.name +'</b><br/>'+
                                this.x +'<br/>'+ this.y +' ms';
                                }",

                });

            return chart;
        }
    }
}