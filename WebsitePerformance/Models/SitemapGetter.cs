using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace WebsitePerformance.Models
{
    public class SitemapGetter
    {
        private string baseSiteUrl;
        private string sitemapUri;
        private string robotsTxtUri;
        private XDocument sitemap;

        public List<string> UriList { get; private set; }

        public SitemapGetter(Uri baseSiteUrl)
        {
            this.baseSiteUrl = baseSiteUrl.GetLeftPart(UriPartial.Authority);
            UriList = new List<string>();
            Initialize();
        }

        private void Initialize()
        {
            sitemapUri = baseSiteUrl + "/sitemap.xml";
            robotsTxtUri = baseSiteUrl + "/robots.txt";
            try
            {
                sitemap = XDocument.Load(sitemapUri);
                GetListOfUri(sitemap);
            }
            catch (WebException sitemapException)
            {
                try
                {
                    GetSitemapUri(robotsTxtUri);
                    sitemap = XDocument.Load(sitemapUri);
                    GetListOfUri(sitemap);
                }
                catch (Exception robotsTxtException)
                {
                    GetListOfUri(baseSiteUrl);
                }
            }
        }

        public List<string> GetListOfUri(string url)
        {
            List<string> allUri = new List<string>();
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument htmlDocument = hw.Load(url);
            foreach (HtmlNode htmlNode in htmlDocument.DocumentNode.SelectNodes("//a[@href]"))
            {
                string hrefValue = htmlNode.GetAttributeValue("href", string.Empty);
                allUri.Add(hrefValue);
            }
            UriList = (from u in allUri
                       where ((u.StartsWith("/") && !u.StartsWith("/#")) || u.StartsWith(url))
                       select u).
                       Select(u => u.StartsWith("/") ? url + u : u).
                       Distinct().ToList();
            return UriList;
        }

        public List<string> GetListOfUri(XDocument sitemap)
        {
            XNamespace sitemapNameSpace = sitemap.Root.Name.Namespace;
            foreach (XElement sitemapElement in sitemap.Root.Elements())
            {
                XElement uriPage = sitemapElement.Element(sitemapNameSpace + "loc");
                if (uriPage != null)
                {
                    if (uriPage.Value.EndsWith(".xml"))
                    {
                        XDocument insertedSitemap = XDocument.Load(uriPage.Value);
                        GetListOfUri(insertedSitemap);
                    }
                    else
                    {
                        UriList.Add(uriPage.Value);
                    }
                }
            }
            return UriList;
        }

        public string GetSitemapUri(string robotsTxtUri)
        {
            WebClient webclient = new WebClient();
            string robotsTxt = webclient.DownloadString(robotsTxtUri);
            string[] robotsTxtArray = robotsTxt.Split('\n');
            sitemapUri = "";
            foreach (string s in robotsTxtArray)
            {
                if (s.Contains("Sitemap"))
                {
                    int index = s.IndexOf("http");
                    sitemapUri = s.Substring(index);
                }
            }
            return sitemapUri;
        }
    }
}