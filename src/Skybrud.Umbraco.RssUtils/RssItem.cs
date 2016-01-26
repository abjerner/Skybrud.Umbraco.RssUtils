using System;
using System.Xml.Linq;

namespace Skybrud.Umbraco.RssUtils {

    public class RssItem {
        
        /// <summary>
        /// Gets or sets the title of the feed item.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the link (URL) of the feed item.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets the publication date of the feed item.
        /// </summary>
        public DateTime PubDate { get; set; }

        /// <summary>
        /// Gets or sets the GUID (global unique identifier) of the feed item.
        /// A GUID could be URL of the feed item or some other unique value
        /// that identifies the item.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets the description of the feed item. If this property is
        /// specified, a &lt;description&gt; element will be inserted in the
        /// XML representing the feed item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the content of the feed item. If this property is
        /// specified, a &lt;content:encoded&gt; element will be inserted in
        /// the XML representing the feed item.
        /// </summary>
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

            XNamespace xContent = "http://purl.org/rss/1.0/modules/content/";

            // Add optinal attributes
            if (!String.IsNullOrWhiteSpace(Description)) xItem.Add(new XElement("description", new XCData(Description)));
            if (!String.IsNullOrWhiteSpace(Content)) xItem.Add(new XElement(xContent + "encoded", new XCData(Content)));

            return xItem;

        }

    }

}