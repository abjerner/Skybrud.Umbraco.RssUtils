using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Skybrud.Umbraco.RssUtils {

    public class RssFeed {

        private DateTime? _pubDate;
        private List<RssItem> _items = new List<RssItem>();

        #region Properties

        public string Title { get; set; }
        public string Link { get; set; }
        public string Generator { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the publication date of the feed. If no publication date has been explicitly
        /// set, the publication date of the most recent item will be used instead - or
        /// <var>DateTime.Now</var> if the feed is empty.
        /// </summary>
        public DateTime PubDate {
            get {
                if (_pubDate != null) return _pubDate.Value;
                return _items.Count == 0 ? DateTime.Now : _items[0].PubDate;
            }
            set { _pubDate = value; }
        }

        /// <summary>
        /// Gets or sets the list of RSS items of the feed.
        /// </summary>
        public List<RssItem> Items {
            get { return _items; }
            set { _items = value ?? new List<RssItem>(); }
        }

        #endregion

        #region Constructor

        public RssFeed() {
            // Default constructor
        }

        /// <summary>
        /// Adds the children of the specified <var>IPublishedContent</var> as RSS items.
        /// </summary>
        /// <param name="parent">The parent of the nodes to be added.</param>
        public RssFeed(IPublishedContent parent) {
            AddRange(parent.Children);
        }

        /// <summary>
        /// Adds the children of the specified <var>IPublishedContent</var> as RSS items.
        /// </summary>
        /// <param name="parent">The parent of the nodes to be added.</param>
        /// <param name="func">Function to convert an <var>IPublishedContent</var> to <var>RssItem</var>.</param>
        public RssFeed(IPublishedContent parent, Func<IPublishedContent, RssItem> func) {
            AddRange(parent.Children, func);
        }

        /// <summary>
        /// Adds the children of the specified <var>IPublishedContent</var> as RSS items.
        /// </summary>
        /// <param name="title">The title of the RSS feed.</param>
        /// <param name="link">A link to the RSS feed or relevant page.</param>
        /// <param name="parent">The parent of the nodes to be added.</param>
        public RssFeed(string title, string link, IPublishedContent parent) {
            Title = title;
            Link = link;
            AddRange(parent.Children);
        }

        /// <summary>
        /// Adds the children of the specified <var>IPublishedContent</var> as RSS items.
        /// </summary>
        /// <param name="title">The title of the RSS feed.</param>
        /// <param name="link">A link to the RSS feed or relevant page.</param>
        /// <param name="parent">The parent of the nodes to be added.</param>
        /// <param name="func">Function to convert an <var>IPublishedContent</var> to <var>RssItem</var>.</param>
        public RssFeed(string title, string link, IPublishedContent parent, Func<IPublishedContent, RssItem> func) {
            Title = title;
            Link = link;
            AddRange(parent.Children, func);
        }

        /// <summary>
        /// Adds the specified collection of <var>IPublishedContent</var> as RSS items.
        /// </summary>
        /// <param name="content">A collection of nodes to be added.</param>
        public RssFeed(IEnumerable<IPublishedContent> content) {
            AddRange(content);
        }

        /// <summary>
        /// Adds the specified collection of <var>IPublishedContent</var> as RSS items.
        /// </summary>
        /// <param name="content">A collection of nodes to be added.</param>
        public RssFeed(IEnumerable<IPublishedContent> content, Func<IPublishedContent, RssItem> func) {
            AddRange(content, func);
        }

        /// <summary>
        /// Adds the specified collection of <var>IPublishedContent</var> as RSS items.
        /// </summary>
        /// <param name="title">The title of the RSS feed.</param>
        /// <param name="link">A link to the RSS feed or relevant page.</param>
        /// <param name="content">A collection of nodes to be added.</param>
        public RssFeed(string title, string link, IEnumerable<IPublishedContent> content) {
            Title = title;
            Link = link;
            AddRange(content);
        }

        /// <summary>
        /// Adds the specified collection of <var>IPublishedContent</var> as RSS items.
        /// </summary>
        /// <param name="title">The title of the RSS feed.</param>
        /// <param name="link">A link to the RSS feed or relevant page.</param>
        /// <param name="content">A collection of nodes to be added.</param>
        /// <param name="func">Function to convert an <var>IPublishedContent</var> to <var>RssItem</var>.</param>
        public RssFeed(string title, string link, IEnumerable<IPublishedContent> content, Func<IPublishedContent, RssItem> func) {
            Title = title;
            Link = link;
            AddRange(content, func);
        }

        #endregion

        #region Member methods

        public void Add(RssItem item) {
            bool mustSort = (_items.Count > 1 && _items.Last().PubDate < item.PubDate);
            _items.Add(item);
            if (!mustSort) return;
            SortItems();
        }

        public void AddRange(IEnumerable<IPublishedContent> content) {
            AddRange(content, x => new RssItem {
                Title = x.Name,
                Guid = x.Id + "",
                PubDate = x.CreateDate,
                Link = x.UrlWithDomain()
            });
        }

        public void AddRange(IEnumerable<IPublishedContent> content, Func<IPublishedContent, RssItem> func) {
            _items.AddRange(from node in content let item = func(node) select item);
            SortItems();
        }

        private void SortItems() {
            _items = _items.OrderByDescending(x => x.PubDate).ToList();
        }

        public XDocument ToXDocument() {

            // Initialize the element for the channel
            XElement xChannel = new XElement(
                "channel",
                new XElement("title", Title ?? ""),
                new XElement("link", Link ?? ""),
                new XElement("pubDate", PubDate.ToUniversalTime().ToString("r")),
                from item in Items orderby item.PubDate descending select item.ToXElement()
            );

            // Add extra elements to the channel (if specified)
            if (!String.IsNullOrWhiteSpace(Generator)) xChannel.Add(new XElement("generator", Generator));
            if (!String.IsNullOrWhiteSpace(Description)) xChannel.Add(new XElement("description", Description));
            if (!String.IsNullOrWhiteSpace(Language)) xChannel.Add(new XElement("language", Language));

            // Initialize the root element of the feed
            XElement xRss = new XElement("rss", xChannel);

            // Add the content namespace only if necessary
            if (Items.Any(x => !String.IsNullOrWhiteSpace(x.Content))) {
                xRss.Add(new XAttribute(XNamespace.Xmlns + "content", "http://purl.org/rss/1.0/content"));
            }

            // Return the XDocument
            return new XDocument(
                new XDeclaration("1.0", "UTF-8", "true"),
                xRss
            );

        }

        public void Write() {
            HttpContext.Current.Response.ContentType = "application/rss+xml";
            ToXDocument().Save(HttpContext.Current.Response.OutputStream);
            HttpContext.Current.Response.End();
        }

        #endregion

        #region Static methods

        public static void Write(string title, string link, IPublishedContent parent) {
            new RssFeed(title, link, parent).Write();
        }

        public static void Write(string title, string link, IPublishedContent parent, Func<IPublishedContent, RssItem> func) {
            new RssFeed(title, link, parent, func).Write();
        }

        public static void Write(string title, string link, IEnumerable<IPublishedContent> content) {
            new RssFeed(title, link, content).Write();
        }

        public static void Write(string title, string link, IEnumerable<IPublishedContent> content, Func<IPublishedContent, RssItem> func) {
            new RssFeed(title, link, content, func).Write();
        }

        #endregion

    }

}