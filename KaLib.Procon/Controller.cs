using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KaLib.IO.Hid;
using KaLib.Texts;
using KaLib.Utils;
using KaLib.Utils.Extensions;

namespace KaLib.Procon
{
    public static class ControllerCommand
    {
        public static readonly byte[] GetMAC = {0x80, 0x01};
        public static readonly byte[] Handshake = {0x80, 0x02};
        public static readonly byte[] SwitchBaudRate = {0x80, 0x03};
        public static readonly byte[] HidOnlyMode = {0x80, 0x04};
        public static readonly byte[] Enable = {0x01};

        public static readonly byte[] LedCalibration = {0xff};
        public static readonly byte[] LedCalibrated = {0x01};

        public static readonly byte GetInput = 0x1f;
        public static readonly byte[] Empty = Array.Empty<byte>();

        public const byte Rumble = 0x48;
        public const byte ImuData = 0x40;
        public const byte LedCommand = 0x30;
    }

    internal struct InputPacket
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] Header;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Unknown;

        public byte RightButtons;
        public byte MiddleButtons;
        public byte LeftButtons;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] Sticks;
    }

    public class Controller
    {
        public const short NintendoId = 0x057e;
        public const short ProconId = 0x2009;
        private const int TestBadDataCycles = 10;
        private bool _badDataDetected;

        private byte _rumbleCounter;

        public HidDevice Device { get; private set; }

        public ButtonState States { get; private set; } = new(new InputPacket
        {
            Header = new byte[1],
            LeftButtons = 0,
            MiddleButtons = 0,
            RightButtons = 0,
            Sticks = new byte[6],
            Unknown = new byte[2]
        });

        public static HidDeviceInfo GetProconHidDeviceInfo()
            => HidDeviceBrowse.Browse().Find(x => x.VendorId == NintendoId && x.ProductId == ProconId);

        public static HidDevice GetProconHidDevice()
        {
            var info = GetProconHidDeviceInfo();
            if (info == null) return null;

            var device = new HidDevice();
            if (device.Open(info)) return device;

            Logger.Error($"Failed to open the device path, path = {info.Path}");
            device.Close();
            return null;
        }

        private void LogRead(byte[] data)
        {
            Logger.Log(
                TranslateText.Of("%s [Device]: %s")
                    .SetColor(TextColor.Aqua)
                    .AddWith(LiteralText.Of("<-").SetColor(TextColor.Green))
                    .AddWith(LiteralText.Of(data.Hexdump()).SetColor(TextColor.Gold))
            );
        }
        
        private void LogWrite(byte[] data)
        {
            Logger.Log(
                TranslateText.Of("%s [Device]: %s")
                    .SetColor(TextColor.Aqua)
                    .AddWith(LiteralText.Of("->").SetColor(TextColor.Red))
                    .AddWith(LiteralText.Of(data.Hexdump()).SetColor(TextColor.Gold))
            );
        }

        private byte[] ExchangeData(byte[] data, bool timed = false)
        {
            if (Device == null) return null;

            // LogWrite(data);
            int ret = Device.Write(data);
            if (ret < 0)
            {
                return null;
            }

            var result = new byte[0x400];
            Array.Fill(result, (byte) 0);

            int length;
            if (!timed)
            {
                length = Device.Read(result);
            }
            else
            {
                length = Device.ReadTimeout(result, 15);
            }

            if (length < 0)
            {
                Logger.Warn("Failed to exchange data from device!");
                return Array.Empty<byte>();
            }
            
            var r = new byte[length];
            Array.Copy(result, r, length);
            // LogRead(r);
            return r;
        }

        private byte[] SendCommand(byte command, byte[] data)
        {
            var buf = new byte[data.Length + 0x9];
            Array.Fill(buf, (byte) 0);
            buf[0] = 0x80;
            buf[1] = 0x92;
            buf[3] = 0x31;
            buf[8] = command;

            if (data.Length > 0)
            {
                Array.Copy(data, 0, buf, 9, data.Length);
            }

            return ExchangeData(buf);
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

        private byte[] SendRumble(byte largeMotor, byte smallMotor)
        {
            var buf = new byte[]
            {
                (byte) (_rumbleCounter++ & 0xf),
                0x80, 0, 0x40, 0x40,
                0x80, 0, 0x40, 0x40
            };

            if (largeMotor != 0)
            {
                buf[1] = buf[5] = 8;
                buf[2] = buf[6] = largeMotor;
            }
            else if (smallMotor != 0)
            {
                buf[1] = buf[5] = 0x10;
                buf[2] = buf[6] = smallMotor;
            }

            return SendCommand(0x10, buf);
        }

        public void OpenFirstProcon()
        {
            var device = GetProconHidDevice();
            if (device == null)
            {
                Logger.Error("No Switch Pro controller found!");
                return;
            }

            Device = device;
            Logger.Info("Switching the Baud rate...");
            ExchangeData(ControllerCommand.SwitchBaudRate);
            Logger.Info("Handshaking...");
            ExchangeData(ControllerCommand.Handshake);
            Logger.Info("Set to HID-only mode...");
            ExchangeData(ControllerCommand.HidOnlyMode, true);

            Logger.Info("Detecting bad data stream...");
            for (int i = 0; i < TestBadDataCycles; i++)
            {
                if (!TryReadBadData()) continue;
                Logger.Warn("Detected bad data stream!");
                Device.Close();
                Thread.Sleep(1000);
                OpenFirstProcon();
                break;
            }
        }

        private bool TryReadBadData()
        {
            if (Device == null) return true;
            var data = SendCommand(ControllerCommand.GetInput, ControllerCommand.Empty);

            if (IsGarbageData(data)) return false;
            return IsBadData(data);
        }

        private bool IsGarbageData(byte[] data) => data[0] == 0 || data[0] == 0x30;

        private bool IsBadData(byte[] data) => data[0] == 0x81 && data[1] == 0x01 || _badDataDetected;

        public void PollInput()
        {
            if (Device == null) return;

            var data = SendCommand(ControllerCommand.GetInput, ControllerCommand.Empty);
            if (data == null || data.Length == 0) return;

            if (data[0] == 0x00)
            {
                // useless data
                return;
            }
            
            // Try to parse from the 0x30 one for now
            IntPtr ptr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, ptr, data.Length);
            var packet = Marshal.PtrToStructure<InputPacket>(ptr);
            
            var oldState = States;
            States = new ButtonState(packet);

            for (var i = 0; i < States.Buttons.Count; i++)
            {
                var a = oldState.Buttons[i];
                var b = States.Buttons[i];
                if (a.pressed != b.pressed)
                {
                    (b.pressed ? ButtonPressed : ButtonReleased)?.Invoke(b.button);
                }
            }

            // Logger.Info($"L = {States.LeftStick}, R = {States.RightStick}, Buttons = {States.GetPressedButtons().Select(x => x.ToString()).JoinToString(", ")}");
        }

        public delegate void ButtonDelegate(Button button);

        public event ButtonDelegate ButtonPressed;
        public event ButtonDelegate ButtonReleased;

        private DateTime _lastStatus = DateTime.Now;

        public void UpdateStatus()
        {
            if (DateTime.Now - _lastStatus < TimeSpan.FromMilliseconds(100)) return;
            var left = (byte) Math.Round(Math.Max(States.LeftStick.Y, 0) * 255);
            var right = (byte) Math.Round(Math.Max(States.RightStick.Y, 0) * 255);
            SendRumble(left, 0);
            SendRumble(0, right);
            _lastStatus = DateTime.Now;
        }
    }
}