namespace Mochi.StreamKit.Twitch.Entities;

public interface IEntity<out T>
{
    public T Id { get; }
}