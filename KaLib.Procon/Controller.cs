using System;
using System.Collections.Generic;
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

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public byte[] Reserved;

        public Vector3s Gyroscope;
        public Vector3s Accelerometer;
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
                Logger.Warn(Device.GetLastError());
                Device.Close();
                return null;
            }

            var result = new byte[0x400];
            Array.Clear(result, 0, result.Length);

            int length = !timed ? Device.Read(result) : Device.ReadTimeout(result, 15);
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
            Array.Clear(buf, 0, buf.Length);
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
        
        private struct Rumble {
            public Queue<float[]> queue;

            public void set_vals(float low_freq, float high_freq, float amplitude) {
                float[] rumbleQueue = new float[] { low_freq, high_freq, amplitude };
                // Keep a queue of 15 items, discard oldest item if queue is full.
                if (queue.Count > 15) {
                    queue.Dequeue();
                }
                queue.Enqueue(rumbleQueue);
            }
            public Rumble(float[] rumble_info) {
                queue = new Queue<float[]>();
                queue.Enqueue(rumble_info);
            }
            private float clamp(float x, float min, float max) {
                if (x < min) return min;
                if (x > max) return max;
                return x;
            }

            private byte EncodeAmp(float amp) {
                byte en_amp;

                if (amp == 0)
                    en_amp = 0;
                else if (amp < 0.117)
                    en_amp = (byte)(((Math.Log(amp * 1000, 2) * 32) - 0x60) / (5 - Math.Pow(amp, 2)) - 1);
                else if (amp < 0.23)
                    en_amp = (byte)(((Math.Log(amp * 1000, 2) * 32) - 0x60) - 0x5c);
                else
                    en_amp = (byte)((((Math.Log(amp * 1000, 2) * 32) - 0x60) * 2) - 0xf6);

                return en_amp;
            }

            public byte[] GetData() {
                byte[] rumble_data = new byte[8];
                float[] queued_data = queue.Dequeue();

                if (queued_data[2] == 0.0f) {
                    rumble_data[0] = 0x0;
                    rumble_data[1] = 0x1;
                    rumble_data[2] = 0x40;
                    rumble_data[3] = 0x40;
                } else {
                    queued_data[0] = clamp(queued_data[0], 40.875885f, 626.286133f);
                    queued_data[1] = clamp(queued_data[1], 81.75177f, 1252.572266f);

                    queued_data[2] = clamp(queued_data[2], 0.0f, 1.0f);

                    UInt16 hf = (UInt16)((Math.Round(32f * Math.Log(queued_data[1] * 0.1f, 2)) - 0x60) * 4);
                    byte lf = (byte)(Math.Round(32f * Math.Log(queued_data[0] * 0.1f, 2)) - 0x40);
                    byte hf_amp = EncodeAmp(queued_data[2]);

                    UInt16 lf_amp = (UInt16)(Math.Round((double)hf_amp) * .5);
                    byte parity = (byte)(lf_amp % 2);
                    if (parity > 0) {
                        --lf_amp;
                    }

                    lf_amp = (UInt16)(lf_amp >> 1);
                    lf_amp += 0x40;
                    if (parity > 0) lf_amp |= 0x8000;

                    hf_amp = (byte)(hf_amp - (hf_amp % 2)); // make even at all times to prevent weird hum
                    rumble_data[0] = (byte)(hf & 0xff);
                    rumble_data[1] = (byte)(((hf >> 8) & 0xff) + hf_amp);
                    rumble_data[2] = (byte)(((lf_amp >> 8) & 0xff) + lf);
                    rumble_data[3] = (byte)(lf_amp & 0xff);
                }

                for (int i = 0; i < 4; ++i) {
                    rumble_data[4 + i] = rumble_data[i];
                }

                return rumble_data;
            }
        }

        private byte[] SendRumble(float lowFreq, float highFreq, float amptitude)
        {
            var rumble = new Rumble(new[] { lowFreq, highFreq, amptitude });
            var data = rumble.GetData();
            var buf = new byte[11];
            Array.Clear(buf, 0, 11);
            Array.Copy(data, 0, buf, 1, 8);
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
            ExchangeData(ControllerCommand.Handshake);
            ExchangeData(ControllerCommand.SwitchBaudRate);
            Logger.Info("Handshaking...");
            ExchangeData(ControllerCommand.Handshake);
            Logger.Info("Set to HID-only mode...");
            ExchangeData(ControllerCommand.HidOnlyMode, true);

            SendSubCommand(1, ControllerCommand.Rumble, ControllerCommand.Enable);
            SendSubCommand(1, ControllerCommand.ImuData, ControllerCommand.Enable);
            SendSubCommand(1, ControllerCommand.LedCommand, ControllerCommand.LedCalibrated);

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
            if (data == null) return false;
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

            if (IsGarbageData(data))
            {
                // useless data
                return;
            }
            
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

        public void UpdateStatus()
        {
            var left = (float)Math.Max(States.LeftStick.Y, 0) * 150;
            var right = (float)Math.Max(States.RightStick.Y, 0) * 150;
            var amp = Math.Max(left, right) / 150;
            if (amp < 0.1f) amp = 0;
            SendRumble(left, right, amp);
        }
    }
}