using System.Runtime.InteropServices;
using KaLib.IO.Hid;
using KaLib.Utils;

namespace KaLib.IO.Controllers.Switch;

public static class ControllerCommand
{
    public static readonly byte[] GetMAC = {0x80, 0x01};
    public static readonly byte[] Handshake = {0x80, 0x02};
    public static readonly byte[] SwitchBaudRate = {0x80, 0x03};
    public static readonly byte[] HidOnlyMode = {0x80, 0x04};
    public static readonly byte[] Enable = {0x01};

    public static readonly byte[] LedCalibration = { 0b1111 };
    public static readonly byte[] LedCalibrated = { 0b1 };

    public static readonly byte GetInput = 0x1f;
    public static readonly byte[] Empty = Array.Empty<byte>();

    public const byte Rumble = 0x48;
    public const byte ImuData = 0x40;
    public const byte LedCommand = 0x30;
}

internal struct Vector3s
{
    public short X;
    public short Y;
    public short Z;
}

internal struct ProconInputPacket
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] Header;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public byte[] Unknown;

    public byte RightButtons;
    public byte MiddleButtons;
    public byte LeftButtons;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public byte[] Sticks;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
    public byte[] Reserved;

    public Vector3s Gyroscope;
    public Vector3s Accelerometer;
}

public class SwitchProController : IController<ProconInputSnapshot>
{
    private readonly HidDevice _device;

    public event Action<IControllerButton>? ButtonPressed;
    public event Action<IControllerButton>? ButtonReleased;
    public event Action<ProconInputSnapshot, ProconInputSnapshot>? SnapshotUpdated;
    public event Action? Disconnected;

    public SwitchProController(HidDevice device)
    {
        _device = device;
    }
    
    public const short NintendoId = 0x057e;
    public const short ProconId = 0x2009;
    private const int TestBadDataCycles = 10;
    private bool _badDataDetected;
    private int _rumbleCounter;

    public bool Disposed { get; private set; }

    public ButtonState States { get; private set; } = new(new ProconInputPacket
    {
        Header = new byte[1],
        LeftButtons = 0,
        MiddleButtons = 0,
        RightButtons = 0,
        Sticks = new byte[6],
        Unknown = new byte[2]
    });
    
    private bool IsGarbageData(byte[] data) => data[0] == 0 || data[0] == 0x30;

    private bool IsBadData(byte[] data) => data[0] == 0x81 && data[1] == 0x01 || _badDataDetected;

    private ProconInputSnapshot _lastSnapshot;

    public IOptional<ProconInputSnapshot> LastSnapshot => Optional.Of(_lastSnapshot);

    public ProconInputSnapshot PollInput()
    {
        var data = SendCommand(ControllerCommand.GetInput, ControllerCommand.Empty);
        if (data == null || data.Length == 0) return _lastSnapshot;

        if (IsGarbageData(data))
        {
            // useless data
            return _lastSnapshot;
        }
            
        var ptr = Marshal.AllocHGlobal(data.Length);
        Marshal.Copy(data, 0, ptr, data.Length);
        var packet = Marshal.PtrToStructure<ProconInputPacket>(ptr);
            
        var oldState = States;
        States = new ButtonState(packet);

        for (var i = 0; i < States.Buttons.Count; i++)
        {
            var a = oldState.Buttons[i];
            var b = States.Buttons[i];
            if (a.pressed != b.pressed)
            {
                // (b.pressed ? ButtonPressed : ButtonReleased)?.Invoke(b.button);
            }
        }

        var buttonMap = States.Buttons.ToDictionary(k => k.button, k => k.pressed);
        var snapshot = new ProconInputSnapshot
        {
            LeftStick = new IControllerStickButton.StickButtonState
            {
                Vector = States.LeftStick,
                Pressed = buttonMap[Button.LStick]
            },
            RightStick = new IControllerStickButton.StickButtonState
            {
                Vector = States.RightStick,
                Pressed = buttonMap[Button.RStick]
            },
            LeftTrigger = buttonMap[Button.ZL],
            RightTrigger = buttonMap[Button.ZR]
        };
        _lastSnapshot = snapshot;
        return snapshot;
    }
    
    private byte[] SendCommand(byte command, byte[] data)
    {
        var buf = new byte[data.Length + 0x9];
        Array.Clear(buf, 0, buf.Length);
        buf[0] = 0x80;
        buf[1] = 0x92;
        buf[3] = 0x31;
        buf[8] = command;

        if (data.Length > 0)
        {
            Array.Copy(data, 0, buf, 9, data.Length);
        }

        return ExchangeData(buf, true);
    }
    
    private byte[] ExchangeData(byte[] data, bool timed = false)
    {
        try
        {
            _device.Write(data);
        }
        catch (HidException)
        {
            Disconnected?.Invoke();
            return Array.Empty<byte>();
        }

        var result = new byte[0x400];
        Array.Clear(result, 0, result.Length);

        try
        {
            int length = !timed ? _device.Read(result) : _device.ReadTimeout(result, 150);
            if (length < 0)
            {
                Logger.Warn("Failed to exchange data from device!");
                return Array.Empty<byte>();
            }

            var r = new byte[length];
            Array.Copy(result, r, length);
            return r;
        }
        catch (HidException)
        {
            Disconnected?.Invoke();
            return Array.Empty<byte>();
        }
    }

    private byte[] SendSubCommand(byte command, byte subCommand, byte[] data)
    {
        var buf = new byte[data.Length + 10];
        var header = new byte[]
        {
            (byte) (_rumbleCounter++ & 0xf),
            0, 1, 0x40, 0x40,
            0, 1, 0x40, 0x40,
            subCommand
        };
        Array.Copy(header, buf, 10);

        if (data.Length > 0)
        {
            Array.Copy(data, 0, buf, 10, data.Length);
        }

        return SendCommand(command, buf);
    }
    
    public static IEnumerable<HidDeviceInfo> GetProconHidDeviceInfo()
        => HidDeviceBrowse.Browse().Where(x => x.VendorId == NintendoId && x.ProductId == ProconId);

    public static IEnumerable<HidDevice> GetProconHidDevice()
    {
        return GetProconHidDeviceInfo()
            .Select(info =>
            {
                var device = new HidDevice();
                if (device.Open(info)) return device;

                Logger.Error($"Failed to open the device path, path = {info.Path}");
                device.Close();
                return null;
            })
            .Where(n => n != null)
            .Select(n => n!);
    }
    
    public static SwitchProController? OpenFirstProcon()
    {
        var device = GetProconHidDevice().FirstOrDefault();
        if (device == null)
        {
            return null;
        }

        return new SwitchProController(device);
    }

    public override int GetHashCode()
    {
        return _device.Info.Path.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not SwitchProController c) return false;
        return c.GetHashCode() == GetHashCode();
    }

    public void Initialize()
    {
        Logger.Verbose("Switching the Baud rate...");
        ExchangeData(ControllerCommand.Handshake);
        ExchangeData(ControllerCommand.SwitchBaudRate);
        Logger.Verbose("Handshaking...");
        ExchangeData(ControllerCommand.Handshake);
        Logger.Verbose("Set to HID-only mode...");
        ExchangeData(ControllerCommand.HidOnlyMode, true);

        SendSubCommand(1, ControllerCommand.Rumble, ControllerCommand.Enable);
        SendSubCommand(1, ControllerCommand.ImuData, ControllerCommand.Enable);
        SendSubCommand(1, ControllerCommand.LedCommand, ControllerCommand.LedCalibrated);
    }

    public void UpdateStates()
    {
    }

    public void Dispose()
    {
        Disposed = true;
        _device.Close();
    }
}