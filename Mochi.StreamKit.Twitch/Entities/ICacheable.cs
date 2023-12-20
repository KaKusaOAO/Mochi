namespace Mochi.StreamKit.Twitch.Entities;

public interface ICacheable
{
    public DateTimeOffset CacheExpireAt { get; }
}

public static class CacheableExtension
{
    public static bool IsCacheExpired(this ICacheable cacheable) => cacheable.CacheExpireAt <= DateTimeOffset.Now;
    public static bool IsValidCache(this ICacheable cacheable) => !cacheable.IsCacheExpired();
}