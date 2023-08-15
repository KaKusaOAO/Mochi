namespace Mochi.StreamKit.Twitch.Entities;

public abstract class Entity<T> : IEntity<T>
{
    public TwitchClient Client { get; }
    public T Id { get; }

    protected Entity(TwitchClient client, T id)
    {
        Client = client;
        Id = id;
    }
}