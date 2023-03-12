using System.Numerics;

namespace KaLib.IO.Controllers.Switch;

public struct ProconInputSnapshot : IBasicControllerSnapshot
{
    public IControllerStickButton.StickButtonState LeftStick { get; init; }
    public IControllerStickButton.StickButtonState RightStick { get; init; }
    public bool LeftTrigger { get; init; }
    public bool RightTrigger { get; init; }
    public bool ButtonA { get; init; }
    public bool ButtonB { get; init; }
    public bool ButtonX { get; init; }
    public bool ButtonY { get; init; }
    public bool LeftShoulder { get; init; }
    public bool RightShoulder { get; init; }
    public IControllerDPad.DPadState DPad { get; init; }
    public bool Share { get; init; }
    public bool Options { get; init; }
    public bool View { get; init; }
    public bool Home { get; init; }

    float IBasicControllerSnapshot.LeftTrigger => LeftTrigger ? 1 : 0;

    float IBasicControllerSnapshot.RightTrigger => RightTrigger ? 1 : 0;
}

public enum Button
{
    None,
    DPadUp, DPadDown, DPadRight, DPadLeft,
    A, B, X, Y, Plus, Minus,
    L, ZL, R, ZR, LStick, RStick, Home, Share
}

public enum ButtonSource
{
    Left, Middle, Right
}

public struct ButtonState
{
    public Vector2 LeftStick { get; }
    public Vector2 RightStick { get; }

    public List<(Button button, bool pressed)> Buttons { get; }
    internal ProconInputPacket Source { get; }
    
    #if NET6_0_OR_GREATER
    public Vector3 Gyroscope { get; }
    
    public Vector3 Accelerometer { get; }
    #endif

    private static readonly Button[] ButtonLeftBitmap = {
        Button.DPadDown,
        Button.DPadUp,
        Button.DPadRight,
        Button.DPadLeft,
        Button.None,
        Button.None,
        Button.L,
        Button.ZL
    };
    
    private static readonly Button[] ButtonRightBitmap = {
        Button.Y,
        Button.X,
        Button.B,
        Button.A,
        Button.None,
        Button.None,
        Button.R,
        Button.ZR
    };
    
    private static readonly Button[] ButtonMiddleBitmap = {
        Button.Minus,
        Button.Plus,
        Button.RStick,
        Button.LStick,
        Button.Home,
        Button.Share,
        Button.None,
        Button.None
    };

    private Button[] GetButtonMap(ButtonSource source)
    {
        return source switch
        {
            ButtonSource.Left => ButtonLeftBitmap,
            ButtonSource.Middle => ButtonMiddleBitmap,
            ButtonSource.Right => ButtonRightBitmap,
            _ => throw new InvalidDataException()
        };
    }

    private List<(Button button, bool pressed)> PullButtonsFromByte(byte c, ButtonSource src)
    {
        var result = new List<(Button, bool)>();
        var map = GetButtonMap(src);
        for (var i = 0; i < map.Length; i++)
        {
            if (map[i] == Button.None) continue;
            result.Add((map[i], (c & (1 << i)) != 0));
        }
        return result;
    }

    internal ButtonState(ProconInputPacket packet)
    {
        Source = packet;
        
        var lx = StickByteToDouble((byte) ((packet.Sticks[1] & 0xf) << 4 | (packet.Sticks[0] & 0xf0) >> 4));
        var ly = StickByteToDouble(packet.Sticks[2]);
        var rx = StickByteToDouble((byte) ((packet.Sticks[4] & 0xf) << 4 | (packet.Sticks[3] & 0xf0) >> 4));
        var ry = StickByteToDouble(packet.Sticks[5]);
        LeftStick = new Vector2(lx, ly);
        RightStick = new Vector2(rx, ry);

        #if NET6_0_OR_GREATER
        Gyroscope = new Vector3(packet.Gyroscope.X, packet.Gyroscope.Y, packet.Gyroscope.Z);
        Accelerometer = new Vector3(packet.Accelerometer.X, packet.Accelerometer.Y, packet.Accelerometer.Z);
        #endif

        Buttons = new();
        Buttons.AddRange(PullButtonsFromByte(packet.LeftButtons, ButtonSource.Left));
        Buttons.AddRange(PullButtonsFromByte(packet.MiddleButtons, ButtonSource.Middle));
        Buttons.AddRange(PullButtonsFromByte(packet.RightButtons, ButtonSource.Right));
    }

    private static float StickByteToDouble(byte b) => b / 255f * 2 - 1;

    public IEnumerable<Button> GetPressedButtons()
        => Buttons.FindAll(x => x.pressed).Select(x => x.button);
}