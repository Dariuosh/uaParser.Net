using System.Collections.Concurrent;

using uaParserLibrary.Models;
using uaParserLibrary.Parsing;

namespace uaParserLibrary;

/// <summary>
/// Remembers recent results, so a user agent that was parsed before is returned without parsing
/// it again. Results are immutable, so they are safe to share. Thread-safe.
/// </summary>
/// <remarks>
/// The cache keeps two generations of at most <see cref="Capacity"/> user agents each. When the
/// current generation is full it becomes the previous one and the oldest is dropped, so memory
/// stays bounded even when every request has a different user agent. A user agent found in the
/// previous generation moves back to the current one.
/// </remarks>
public sealed class ClientInfoCache
{
    private readonly Lock _rotation = new();
    private ConcurrentDictionary<string, ClientInfo> _current = new(StringComparer.Ordinal);
    private ConcurrentDictionary<string, ClientInfo> _previous = new(StringComparer.Ordinal);
    private int _currentCount;   // entries added to _current (ConcurrentDictionary.Count takes every lock)

    /// <param name="capacity">How many user agents each generation holds (at least 1).</param>
    public ClientInfoCache(int capacity = 1024)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        Capacity = capacity;
    }

    /// <summary>How many user agents each generation holds.</summary>
    public int Capacity { get; }

    /// <summary>The number of user agents currently remembered (in both generations).</summary>
    public int Count => _current.Count + _previous.Count;

    /// <summary>Like <see cref="UAParser.GetClientInfo(string?)"/>, but reuses earlier results.</summary>
    public ClientInfo GetClientInfo(string? userAgent)
    {
        // Same key as the parser sees: at most 500 characters, so entries stay small.
        var key = JsString.NormalizeUserAgent(userAgent);
        return Get(key, key, static userAgent => UAParser.GetClientInfo(userAgent));
    }

    /// <summary>
    /// Like <see cref="UAParser.GetClientInfo(string?, ClientHints?, string?)"/> without a
    /// renderer, but reuses earlier results for the same user agent and hints.
    /// </summary>
    public ClientInfo GetClientInfo(string? userAgent, ClientHints? hints)
    {
        if (hints is null || hints.IsEmpty)
            return GetClientInfo(userAgent);

        var normalized = JsString.NormalizeUserAgent(userAgent);
        return Get(normalized + "\n" + hints.CacheKey(), (normalized, hints),
            static state => UAParser.GetClientInfo(state.normalized, state.hints));
    }

    private ClientInfo Get<TState>(string key, TState state, Func<TState, ClientInfo> parse)
    {
        var current = _current;
        if (current.TryGetValue(key, out var info))
            return info;

        // Found in the previous generation: move it back to the current one.
        if (!_previous.TryRemove(key, out info))
            info = parse(state);

        if (current.TryAdd(key, info) && Interlocked.Increment(ref _currentCount) >= Capacity)
            Rotate(current);
        return info;
    }

    /// <summary>Forgets every result.</summary>
    public void Clear()
    {
        lock (_rotation)
        {
            _previous = new(StringComparer.Ordinal);
            _current = new(StringComparer.Ordinal);
            _currentCount = 0;
        }
    }

    private void Rotate(ConcurrentDictionary<string, ClientInfo> full)
    {
        lock (_rotation)
        {
            if (_current != full)
                return;   // another thread rotated already

            _previous = full;
            _current = new(StringComparer.Ordinal);
            _currentCount = 0;
        }
    }
}
