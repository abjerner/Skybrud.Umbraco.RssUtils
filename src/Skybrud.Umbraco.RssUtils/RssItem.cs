using System;
using System.Xml.Linq;

namespace Skybrud.Umbraco.Rss {

    public class RssItem {

        public string Title { get; set; }
        public string Link { get; set; }
        public DateTime PubDate { get; set; }
        public string Guid { get; set; }
        public string Content { get; set; }

        public XElement ToXElement() {

            // Generate the XML node with mandatory attributes
            XElement xItem = new XElement(
                "item",
                new XElement("title", Title ?? ""),
                new XElement("link", Link ?? ""),
                new XElement("pubDate", PubDate.ToUniversalTime().ToString("r")),
                new XElement("guid", Guid ?? "")
            );

            XNamespace xContent = "http://purl.org/rss/1.0/content";

            // Add optinal attributes
            if (!String.IsNullOrWhiteSpace(Content)) xItem.Add(new XElement(xContent + "encoded", new XCData(Content)));

            return xItem;

        }

    }

}