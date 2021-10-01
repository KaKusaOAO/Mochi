using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using KaLib.IO.Hid;

namespace KaLib.Procon
{
    public static class ControllerCommand
    {
        public static readonly byte[] GetMAC = { 0x80, 0x01 };
        public static readonly byte[] Handshake = { 0x80, 0x02 };
        public static readonly byte[] SwitchBaudRate = { 0x80, 0x03 };
        public static readonly byte[] HidOnlyMode = { 0x80, 0x04 };
        public static readonly byte[] Enable = { 0x01 };
        public static readonly byte[] Led = { 0x01 };

        public static readonly byte GetInput = 0x1f;
        public static readonly byte[] Empty = Array.Empty<byte>();

        public const byte Rumble = 0x48;
        public const byte ImuData = 0x40;
        public const byte LedCommand = 0x30;
    }

    internal struct InputPacket
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
    }

    public class Controller
    {
        public const short NintendoId = 0x057e;
        public const short ProconId = 0x2009;

        private byte _rumbleCounter;
        
        public HidDevice Device { get; private set; }
        
        public ButtonState States { get; private set; }

        public static HidDeviceInfo GetProconHidDeviceInfo()
            => HidDeviceBrowse.Browse().Find(x => x.VendorId == NintendoId && x.ProductId == ProconId);

        public static HidDevice GetProconHidDevice()
        {
            var info = GetProconHidDeviceInfo();
            if (info == null) return null;

            var device = new HidDevice();
            if (device.Open(info)) return device;
            
            Console.WriteLine($"Error: Failed to open the device path, path = {info.Path}");
            device.Close();
            return null;
        }

        private byte[] ExchangeData(byte[] data)
        {
            if (Device == null) return null;
            int ret = Device.Write(data);
            if (ret < 0)
            {
                return null;
            }

            var result = new byte[0x400];
            Array.Fill(result, (byte)0);
            Device.Read(result);
            return result;
        }

        private byte[] SendCommand(byte command, byte[] data)
        {
            var buf = new byte[data.Length + 0x9];
            Array.Fill(buf, (byte)0);
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
                (byte)(_rumbleCounter++ & 0xf),
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
            } else if (smallMotor != 0)
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
                Console.WriteLine("Error: No Switch Pro controller found");
                return;
            }
            
            Device = device;
            ExchangeData(ControllerCommand.Handshake);
            ExchangeData(ControllerCommand.GetMAC);
            ExchangeData(ControllerCommand.SwitchBaudRate);
            ExchangeData(ControllerCommand.Handshake);
            // ExchangeData(ControllerCommand.HidOnlyMode);
            
            SendSubCommand(0x1, ControllerCommand.Rumble, ControllerCommand.Enable);
            SendSubCommand(0x1, ControllerCommand.ImuData, ControllerCommand.Enable);
            SendSubCommand(0x1, ControllerCommand.LedCommand, ControllerCommand.Led);
        }

        public void PollInput()
        {
            if (Device == null) return;

            var data = SendCommand(ControllerCommand.GetInput, ControllerCommand.Empty);
            if (data == null)
            {
                throw new IOException("Error sending getInput command.");
            }

            if (data[0] == 0x30)
            {
                var ptr = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, ptr, data.Length);
                var packet = Marshal.PtrToStructure<InputPacket>(ptr);
                States = new ButtonState(packet);
                Marshal.FreeHGlobal(ptr);
            }
        }

        private DateTime _lastStatus = DateTime.Now;
        
        public void UpdateStatus()
        {
            if (DateTime.Now - _lastStatus < TimeSpan.FromMilliseconds(100)) return;
            var left = (byte)Math.Round(Math.Max(States.LeftStick.Y, 0) * 255);
            var right = (byte)Math.Round(Math.Max(States.RightStick.Y, 0) * 255);
            SendRumble(left, 0);
            SendRumble(0, right);
            SendSubCommand(0x1, ControllerCommand.LedCommand, new byte[] { 0x1 });
            _lastStatus = DateTime.Now;
        }
    }
}