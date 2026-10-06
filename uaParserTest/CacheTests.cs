using uaParserLibrary;

using Xunit;

namespace uaParserTest;

public class CacheTests
{
    private const string Chrome =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36";

    [Fact]
    public void Gives_the_same_result_as_the_parser()
    {
        var cache = new ClientInfoCache();

        Assert.Equal(UAParser.GetClientInfo(Chrome), cache.GetClientInfo(Chrome));
        Assert.Equal(UAParser.GetClientInfo(null), cache.GetClientInfo(null));
    }

    [Fact]
    public void Reuses_the_result_for_a_repeated_user_agent()
    {
        var cache = new ClientInfoCache();

        Assert.Same(cache.GetClientInfo(Chrome), cache.GetClientInfo(Chrome));
    }

    [Fact]
    public void Keys_on_the_first_500_characters_like_the_parser()
    {
        var cache = new ClientInfoCache();
        var longer = Chrome + new string('x', 600);

        Assert.Same(cache.GetClientInfo(longer + "a"), cache.GetClientInfo(longer + "b"));
    }

    [Fact]
    public void Stays_bounded_when_every_user_agent_is_different()
    {
        var cache = new ClientInfoCache(capacity: 100);

        for (var i = 0; i < 10_000; i++)
            cache.GetClientInfo($"{Chrome} {i}");

        Assert.InRange(cache.Count, 1, 200);
    }

    [Fact]
    public void Keeps_recently_used_user_agents_across_a_rotation()
    {
        var cache = new ClientInfoCache(capacity: 10);
        var first = cache.GetClientInfo(Chrome);

        for (var i = 0; i < 9; i++)
            cache.GetClientInfo($"other {i}");   // fills the generation: Chrome moves to "previous"
        Assert.Same(first, cache.GetClientInfo(Chrome));   // found there, moved back to "current"

        for (var i = 0; i < 9; i++)
            cache.GetClientInfo($"more {i}");
        Assert.Same(first, cache.GetClientInfo(Chrome));
    }

    [Fact]
    public void Clear_forgets_everything()
    {
        var cache = new ClientInfoCache();
        var first = cache.GetClientInfo(Chrome);

        cache.Clear();

        Assert.Equal(0, cache.Count);
        Assert.NotSame(first, cache.GetClientInfo(Chrome));
    }

    [Fact]
    public void Concurrent_use_gives_correct_results()
    {
        var cache = new ClientInfoCache(capacity: 50);
        var userAgents = TestData.Golden.Keys.Take(300).ToArray();
        var expected = userAgents.ToDictionary(ua => ua, ua => UAParser.GetClientInfo(ua));
        var wrong = 0;

        Parallel.For(0, 30_000, i =>
        {
            var ua = userAgents[i % userAgents.Length];
            if (cache.GetClientInfo(ua) != expected[ua])
                Interlocked.Increment(ref wrong);
        });

        Assert.Equal(0, wrong);
    }

    [Fact]
    public void Capacity_must_be_positive()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ClientInfoCache(0));
    }
}
