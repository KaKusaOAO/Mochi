using System.Numerics;

namespace KaLib.IO.Controllers.DualSense;

public struct TouchState
{
    public Vector2 Position { get; set; }
    public byte Id { get; set; }
    public bool IsActive { get; set; }

    public static TouchState FromRaw(uint value)
    {
        var x = (ushort) ((value & 0x000fff00) >> 8);
        var y = (ushort) ((value & 0xfff00000) >> 20);
        var down = (value & (1 << 7)) == 0;
        var id = (byte) (value & 0x7f);

        return new TouchState
        {
            Position = new Vector2(x / 1919f, y / 1079f),
            Id = id,
            IsActive = down
        };
    }

    public override string ToString()
    {
        return $"TouchState[#{Id},{Position},{IsActive}]";
    }
}