Skybrud.Umbraco.RssUtils
========================

## Example

This package is created for use in Umbraco, so this example shows how to use the package in an Umbraco MVC view. However the package itself doesn't contain any Umbraco specific code, so you're free to do something similar in other systems (or just native ASP.NET) as well.

```C#
@using System.Xml.Linq
@using Skybrud.Umbraco.RssUtils
@inherits Umbraco.Web.Mvc.UmbracoTemplatePage
@{
    Layout = null;

    // Obviously the news articles shouldn't be hardcoded
    var articles = new[] {
        new {
            Id = 1,
            Name = "This is the first news article",
            Url = "http://example.bjerner.dk/news/this-is-the-first-news-article/",
            PublishDate = new DateTime(2014, 8, 31, 0, 58, 55),
            Teaser = "This is the teaser text of the first news article"
        },
        new {
            Id = 2,
            Name = "This is the second news article",
            Url = "http://example.bjerner.dk/news/this-is-the-second-news-article/",
            PublishDate = new DateTime(2014, 8, 31, 1, 02, 34),
            Teaser = (string) null
        }
    };

    // Initialize a new feed and add the blog posts
    RssFeed feed = new RssFeed {
        Title = "News from Example",
        Link = Model.Content.UrlWithDomain() + "rss",
        Description = "You can now get news from Example as an RSS feed",
        Language = "en-US"
    };
    
    foreach (var article in articles.Take(30)) {
        feed.Add(new RssItem {
            Title = article.Name,
            Link = article.Url,
            PubDate = article.PublishDate,
            Description = article.Teaser,
            Content = article.Teaser,
            Guid = article.Url
        });
    }

    // Write the RSS feed to the response stream
    feed.Write(SaveOptions.None);
    
}
```

The above example would generate an output like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<rss xmlns:content="http://purl.org/rss/1.0/content">
  <channel>
    <title>News from Example</title>
    <link>http://localhost:54722/rss</link>
    <pubDate>Sat, 30 Aug 2014 22:58:55 GMT</pubDate>
    <description>You can now get news from Example as an RSS feed</description>
    <language>en-US</language>
    <item>
      <title>This is the second news article</title>
      <link>http://example.bjerner.dk/news/this-is-the-second-news-article/</link>
      <pubDate>Sat, 30 Aug 2014 23:02:34 GMT</pubDate>
      <guid>http://example.bjerner.dk/news/this-is-the-second-news-article/</guid>
    </item>
    <item>
      <title>This is the first news article</title>
      <link>http://example.bjerner.dk/news/this-is-the-first-news-article/</link>
      <pubDate>Sat, 30 Aug 2014 22:58:55 GMT</pubDate>
      <guid>http://example.bjerner.dk/news/this-is-the-first-news-article/</guid>
      <description><![CDATA[This is the teaser text of the first news article]]></description>
      <content:encoded><![CDATA[This is the teaser text of the first news article]]></content:encoded>
    </item>
  </channel>
</rss>
```

## Changelog

### Skybrud.Umbraco.RssUtils 1.0.3
_31st of August, 2014_

__Download__
- [Get on NuGet](https://www.nuget.org/packages/Skybrud.Umbraco.RssUtils/1.0.3)

__Changelog__
- Changed namespace from `Skybrud.Umbraco.Rss` to `Skybrud.Umbraco.RssUtils` to follow package name ([See commit](/abjerner/Skybrud.Umbraco.RssUtils/commit/2f9fed5f07e51235d19c6d1b1755ec94f74c09e9))
- Introduced the description property for RSS items ([See commit](/abjerner/Skybrud.Umbraco.RssUtils/commit/cf881f00c5cc058a6e31e2f25a2bf599d9204df4))
- Added some extra format options when writing the feed ([See commit](/abjerner/Skybrud.Umbraco.RssUtils/commit/5a5cd2438ff1092a131ee6b6e76bd74ba232050a))
- Other adjustments throughout the package
