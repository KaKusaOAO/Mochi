using System.Numerics;
using System.Runtime.InteropServices;
using KaLib.IO.Hid;
using KaLib.Utils;
using KaLib.Utils.Extensions;

namespace KaLib.IO.Controllers.DualSense;

public class DualSenseController : IBasicController
{
    private readonly HidDevice _device;
    private IEnumerable<IControllerButton> _buttons;
    public GenericStick LeftStick { get; } = new("L3");
    public GenericStick RightStick { get; } = new("R3");
    public DualSenseTrigger LeftTrigger { get; } = new();
    public DualSenseTrigger RightTrigger { get; } = new();
    public GeneralControllerButton ButtonCross { get; } = new("Cross");
    public GeneralControllerButton ButtonCircle { get; } = new("Circle");
    public GeneralControllerButton ButtonSquare { get; } = new("Square");
    public GeneralControllerButton ButtonTriangle { get; } = new("Triangle");
    public GeneralControllerButton LeftShoulder { get; } = new("L1");
    public GeneralControllerButton RightShoulder { get; } = new("R1");
    public GenericDPad DPad { get; } = new();
    public GeneralControllerButton Share { get; } = new();
    public GeneralControllerButton Options { get; } = new();
    public DualSenseTouchPad TouchPad { get; } = new();
    public GeneralControllerButton PlayStationLogo { get; } = new();
    public DualSenseMicButton Mic { get; } = new();
    public Vector3 Accelerometer { get; private set; }
    public Vector3 Gyroscope { get; private set; }

    public event Action<IControllerButton> ButtonPressed;
    public event Action<IControllerButton> ButtonReleased;
    public event Action Disconnected;

    public DualSenseController(HidDevice device)
    {
        _device = device;
        _buttons = new IControllerButton[]
        {
            LeftStick, RightStick, LeftTrigger, RightTrigger,
            ButtonCircle, ButtonCross, ButtonSquare, ButtonTriangle,
            LeftShoulder, RightShoulder, DPad.Up, DPad.Down, DPad.Left, DPad.Right,
            Share, Options, TouchPad, PlayStationLogo, Mic
        };

        foreach (var button in ((IController)this).Buttons)
        {
            button.ButtonPressed += b => ButtonPressed?.Invoke(b);
            button.ButtonReleased += b => ButtonReleased?.Invoke(b);
        }
    }

    public IEnumerable<IControllerButton> Buttons => _buttons;

    public unsafe void PollEvents()
    {
        var buf = new byte[64];
        _device.Read(buf);
        var packet = Common.BufferToStructure<DualSenseInputState>(buf, 1);

        var x = packet.LeftStickX / 128f - 1;
        var y = packet.LeftStickY / 128f - 1;
        LeftStick.Vector = new Vector2(x, y);

        x = packet.RightStickX / 128f - 1;
        y = packet.RightStickY / 128f - 1;
        RightStick.Vector = new Vector2(x, y);

        LeftTrigger.Value = packet.LeftTrigger / 255f;
        RightTrigger.Value = packet.RightTrigger / 255f;

        var btnARaw = packet.ButtonsA;
        var btnDPad = btnARaw & 0xf;
        var btnA = (DualSenseButtonsA) (btnARaw & 0xf0);

        DPad.Up.Pressed = btnDPad is 7 or 0 or 1;
        DPad.Right.Pressed = btnDPad is 1 or 2 or 3;
        DPad.Down.Pressed = btnDPad is 3 or 4 or 5;
        DPad.Left.Pressed = btnDPad is 5 or 6 or 7;

        ButtonCross.Pressed = btnA.HasFlag(DualSenseButtonsA.Cross);
        ButtonCircle.Pressed = btnA.HasFlag(DualSenseButtonsA.Circle);
        ButtonSquare.Pressed = btnA.HasFlag(DualSenseButtonsA.Square);
        ButtonTriangle.Pressed = btnA.HasFlag(DualSenseButtonsA.Triangle);

        var btnB = packet.ButtonsB;
        var btnC = packet.ButtonsC;

        LeftShoulder.Pressed = btnB.HasFlag(DualSenseButtonsB.LeftShoulder);
        RightShoulder.Pressed = btnB.HasFlag(DualSenseButtonsB.RightShoulder);
        LeftStick.Pressed = btnB.HasFlag(DualSenseButtonsB.LeftStick);
        RightStick.Pressed = btnB.HasFlag(DualSenseButtonsB.RightStick);
        Share.Pressed = btnB.HasFlag(DualSenseButtonsB.Share);
        Options.Pressed = btnB.HasFlag(DualSenseButtonsB.Options);
        PlayStationLogo.Pressed = btnC.HasFlag(DualSenseButtonsC.Home);
        TouchPad.Pressed = btnC.HasFlag(DualSenseButtonsC.PadButton);
        Mic.Pressed = btnC.HasFlag(DualSenseButtonsC.MicButton);

        var ax = packet.Accelerometer[0] / (float) short.MaxValue;
        var ay = packet.Accelerometer[1] / (float) short.MaxValue;
        var az = packet.Accelerometer[2] / (float) short.MaxValue;
        Accelerometer = new Vector3(ax, ay, az);

        var gx = packet.Gyroscope[0] / (float) short.MaxValue;
        var gy = packet.Gyroscope[1] / (float) short.MaxValue;
        var gz = packet.Gyroscope[2] / (float) short.MaxValue;
        Gyroscope = new Vector3(gx, gy, gz);

        TouchPad.TouchStates[0] = TouchState.FromRaw(packet.Touch1);
        TouchPad.TouchStates[1] = TouchState.FromRaw(packet.Touch2);

        _ = Task.CompletedTask;
    }
    
    

    public unsafe void SendStates()
    {
        var buf = new byte[0x40];
        buf[0] = 2;
        buf[1] = 0xff;
        buf[2] = 0xf7;

        var force = (byte) Math.Round(TouchPad.TouchStates[0].Position.X * 255);

        var output = new DualSenseOutputState
        {
            LeftRumble = 0,
            RightRumble = 0,
            MicLed = (byte) (Mic.IsLedEnabled ? 0x1 : 0x0),
            MicFlag = (byte) (Mic.IsLedEnabled ? 0x10 : 0x00),
            RightAdaptiveTriggerState = new AdaptiveTriggerState
            {
                Mode = AdaptiveTriggerMode.Section,
                Forces = new AdaptiveTriggerForces
                {
                    Force1 = force
                }
            },
            PlayerLed = TouchPad.PlayerLed,
            TouchpadColorR = TouchPad.LedColor.R,
            TouchpadColorG = TouchPad.LedColor.G,
            TouchpadColorB = TouchPad.LedColor.B
        };
        
        var ptr = Marshal.AllocHGlobal(61);
        for (var i = 0; i < 61; i++)
        {
            Marshal.WriteByte(ptr, i, 0);
        }
        
        Marshal.StructureToPtr(output, ptr, false);
        Marshal.Copy(ptr, buf, 3, 61);
        Marshal.FreeHGlobal(ptr);
        
        _device.Write(buf);
    }

    public void Update()
    {
        try
        {
            PollEvents();
            SendStates();
        }
        catch (HidException)
        {
            Disconnected?.Invoke();
            Dispose();
        }
    }

    public static bool IsDualSense(HidDeviceInfo info)
    {
        return info.VendorId == 0x054c && info.ProductId == 0x0ce6;
    }

    public static HidDevice? FindFirstDualSense()
    {
        var info = HidDeviceBrowse.Browse().Find(IsDualSense);
        if (info == null) return null;

        var device = new HidDevice();
        if (device.Open(info)) return device;

        Logger.Error($"Failed to open the device path, path = {info.Path}");
        device.Close();
        return null;
    }

    public void Dispose()
    {
        _device.Dispose();
    }

    IControllerStickButton IBasicController.LeftStick => LeftStick;
    IControllerStickButton IBasicController.RightStick => RightStick;
    IControllerTrigger IBasicController.LeftTrigger => LeftTrigger;
    IControllerTrigger IBasicController.RightTrigger => RightTrigger;
    IControllerButton IBasicController.ButtonA => ButtonCross;
    IControllerButton IBasicController.ButtonB => ButtonCircle;
    IControllerButton IBasicController.ButtonX => ButtonSquare;
    IControllerButton IBasicController.ButtonY => ButtonTriangle;
    IControllerButton IBasicController.LeftShoulder => LeftShoulder;
    IControllerButton IBasicController.RightShoulder => RightShoulder;
    IControllerDPad IBasicController.DPad => DPad;
    IControllerButton IBasicController.Share => Share;
    IControllerButton IBasicController.Options => Options;
    IControllerButton IBasicController.Function => TouchPad;
    IControllerButton IBasicController.Home => PlayStationLogo;
}