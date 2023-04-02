namespace KaLib.IO.Controllers.DualSense;

public enum PlayerLedMode : byte
{
    None,
    One   = 0b00100,
    Two   = 0b01010,
    Three = 0b10101,
    Four  = 0b11011,
    All   = 0b11111
}