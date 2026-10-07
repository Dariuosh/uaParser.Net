using uaParserLibrary;
using uaParserLibrary.Models;

using Xunit;

namespace uaParserTest;

public class BotTests
{
    [Theory]
    [InlineData("Mozilla/5.0 AppleWebKit/537.36 (KHTML, like Gecko; compatible; GPTBot/1.2; +https://openai.com/gptbot)", "GPTBot", BotCategories.AiCrawler)]
    [InlineData("Mozilla/5.0 AppleWebKit/537.36 (KHTML, like Gecko; compatible; ClaudeBot/1.0; +claudebot@anthropic.com)", "ClaudeBot", BotCategories.AiCrawler)]
    [InlineData("Mozilla/5.0 AppleWebKit/537.36 (KHTML, like Gecko; compatible; PerplexityBot/1.0; +https://perplexity.ai/perplexitybot)", "PerplexityBot", BotCategories.AiCrawler)]
    [InlineData("Mozilla/5.0 (Linux; Android 6.0.1; Nexus 5X Build/MMB29P) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.7339.127 Mobile Safari/537.36 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)", "Googlebot", BotCategories.SearchEngine)]
    [InlineData("Mozilla/5.0 AppleWebKit/537.36 (KHTML, like Gecko; compatible; bingbot/2.0; +http://www.bing.com/bingbot.htm) Chrome/116.0.1938.76 Safari/537.36", "bingbot", BotCategories.SearchEngine)]
    [InlineData("facebookexternalhit/1.1 (+http://www.facebook.com/externalhit_uatext.php)", "facebookexternalhit", BotCategories.SocialPreview)]
    [InlineData("Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) HeadlessChrome/141.0.0.0 Safari/537.36", "HeadlessChrome", BotCategories.BrowserAutomation)]
    [InlineData("Mozilla/5.0 (compatible; UptimeRobot/2.0; http://www.uptimerobot.com/)", "UptimeRobot", BotCategories.Monitoring)]
    [InlineData("curl/8.9.1", "curl", BotCategories.HttpLibrary)]
    [InlineData("python-requests/2.32.3", "python-requests", BotCategories.HttpLibrary)]
    [InlineData("Java/17.0.12", "Java", BotCategories.HttpLibrary)]
    [InlineData("PostmanRuntime/7.39.0", "PostmanRuntime", BotCategories.HttpLibrary)]
    // Regex patterns: case variants and "(^| )".
    [InlineData("WGETbot/1.0 (+http://wget.alanreed.org)", "Wget", BotCategories.HttpLibrary)]
    [InlineData("sentry/8.22.0 (https://sentry.io)", "Sentry", BotCategories.Monitoring)]
    public void Finds_known_bots(string userAgent, string name, string category)
    {
        var bot = UAParser.GetBot(userAgent);

        Assert.True(bot.IsBot);
        Assert.Equal(name, bot.Name);
        Assert.Equal(category, bot.Category);
    }

    [Theory]
    [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36")]
    [InlineData("Mozilla/5.0 (iPhone; CPU iPhone OS 18_7 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/26.0 Mobile/15E148 Safari/604.1")]
    // Phones whose names the list mistook for bots, fixed in rules/bot-rules.js.
    [InlineData("Mozilla/5.0 (Linux; Android 4.4.2; HTC Butterfly Build/KOT49H) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.136 Mobile Safari/537.36")]
    [InlineData("Mozilla/5.0 (Linux; Android 6.0; Lucky Ultra Sonic Build/MRA58K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.133 Mobile Safari/537.36")]
    [InlineData("Mozilla/5.0 (Linux; Android 15; CPH2557 Build/AP3A.240617.008; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/131.0.6778.135 Mobile Safari/537.36")]
    [InlineData("sprd-L008/1.0 Linux/2.6.35.7 Android/2.3.5 Release/10.01.2012 Browser/AppleWebKit533.1 Profile/MIDP-2.0 Configuration/CLDC-1.1")]
    // Cubot makes phones.
    [InlineData("Mozilla/5.0 (Linux; Android 13; CUBOT KINGKONG 9 Build/TP1A.220624.014) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.6099.230 Mobile Safari/537.36")]
    [InlineData(null)]
    [InlineData("")]
    public void Browsers_are_not_bots(string? userAgent)
    {
        var bot = UAParser.GetBot(userAgent);

        Assert.False(bot.IsBot);
        Assert.Same(Bot.None, bot);
        Assert.Same(Bot.None, UAParser.GetClientInfo(userAgent).Bot);
    }

    [Theory]
    [InlineData("Mozilla/5.0 (compatible; ExampleBot/1.0; +https://example.com/bot)", "ExampleBot")]
    [InlineData("my-site-crawler/2.1", "my-site-crawler")]
    [InlineData("Mozilla/5.0 (compatible; Some Spider)", "Spider")]
    public void Names_unknown_bots(string userAgent, string name)
    {
        var bot = UAParser.GetBot(userAgent);

        Assert.Equal(new Bot(name, null, null), bot);
    }

    [Fact]
    public void The_match_that_starts_first_wins()
    {
        // Both "W3C-checklink" and "libwww-perl" are in the list; libwww-perl comes first in it.
        Assert.Equal("W3C-checklink", UAParser.GetBot("W3C-checklink/4.5 [4.160] libwww-perl/5.823").Name);
    }

    [Fact]
    public void Describes_a_bot_in_one_line()
    {
        Assert.Equal("Bot    : curl http-library", UAParser.GetBot("curl/8.9.1").ToString());
        Assert.Equal("Bot    : ", Bot.None.ToString());
    }

    [Fact]
    public void Names_the_source_of_the_list()
    {
        Assert.StartsWith("crawler-user-agents ", UAParser.BotListSource);
    }
}
