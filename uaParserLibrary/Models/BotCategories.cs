namespace uaParserLibrary.Models;

/// <summary>The values <see cref="Bot.Category"/> can have (the tags of the crawler-user-agents list).</summary>
public static class BotCategories
{
    /// <summary>Collects pages for academic research.</summary>
    public const string Academic = "academic";
    /// <summary>Checks pages for an advertising network, for example AdsBot-Google.</summary>
    public const string Advertising = "advertising";
    /// <summary>Collects pages for an AI model or answers questions with them, for example GPTBot or ClaudeBot.</summary>
    public const string AiCrawler = "ai-crawler";
    /// <summary>Archives pages, for example the Internet Archive.</summary>
    public const string Archiver = "archiver";
    /// <summary>A browser controlled by a program, for example headless Chrome or PhantomJS.</summary>
    public const string BrowserAutomation = "browser-automation";
    /// <summary>Reads RSS and Atom feeds.</summary>
    public const string FeedReader = "feed-reader";
    /// <summary>An HTTP library or command-line tool, for example curl, python-requests or okhttp.</summary>
    public const string HttpLibrary = "http-library";
    /// <summary>Checks that a site is up and fast, for example UptimeRobot or Pingdom.</summary>
    public const string Monitoring = "monitoring";
    /// <summary>Looks for security problems, for example a vulnerability scanner.</summary>
    public const string Scanner = "scanner";
    /// <summary>Indexes pages for a search engine, for example Googlebot or bingbot.</summary>
    public const string SearchEngine = "search-engine";
    /// <summary>Analyses sites for search engine optimisation, for example AhrefsBot or SemrushBot.</summary>
    public const string Seo = "seo";
    /// <summary>Reads a page to show a preview of a shared link, for example facebookexternalhit or Slackbot.</summary>
    public const string SocialPreview = "social-preview";
}
