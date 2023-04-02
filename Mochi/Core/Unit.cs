namespace Mochi.Core;

public struct Unit
{
    public static Unit Instance { get; } = new();

    public override bool Equals(object obj) => obj is Unit;
    public override int GetHashCode() => 0;
    public static bool operator ==(Unit left, Unit right) => left.Equals(right);
    public static bool operator !=(Unit left, Unit right) => !(left == right);
}