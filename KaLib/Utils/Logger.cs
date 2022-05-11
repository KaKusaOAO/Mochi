using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using KaLib.Texts;

namespace KaLib.Utils
{
    public delegate Task AsyncLogDelegate(LogLevel level, Text text, TextColor color, Text name);

    public static class Logger
    {
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;
        public static event AsyncLogDelegate Logged;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("kernel32")]
        private static extern bool SetConsoleCP(uint cp);
        
        [DllImport("kernel32")]
        private static extern bool SetConsoleOutputCP(uint cp);
        
        public static LogLevel Level { get; set; }

        private const string DefaultName = null;

        public static  TranslateText PrefixFormat => TranslateText.Of("{2} - {0} {1}");

        private static SemaphoreSlim _logLock = new(1, 1);

        private static string GetCallSourceName()
        {
            string name;
            StackTrace stack = new StackTrace();
            MethodBase method = stack.GetFrame(2)?.GetMethod();
            if (method == null)
            {
                name = "?";
            }
            else
            {
                name = GetRootType(method.DeclaringType).FullName;
            }
            return name;
        }

        private static Type GetCallSourceType()
        {
            StackTrace stack = new StackTrace();
            MethodBase method = stack.GetFrame(2)?.GetMethod();
            return GetRootType(method.DeclaringType);
        }

        private static Type GetRootType(Type t)
        {
            if (t.Name.StartsWith("<"))
            {
                return GetRootType(t.DeclaringType);
            }
            return t;
        }

        static Logger()
        {
#if DEBUG
            Level = LogLevel.Verbose;
#else
            Level = LogLevel.Info;
#endif

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Force the console to use UTF-8
                SetConsoleCP(65001);
                SetConsoleOutputCP(65001);
                
                // Write ASCII colored text on Windows
                var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
                GetConsoleMode(iStdOut, out uint outConsoleMode);
                outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
                SetConsoleMode(iStdOut, outConsoleMode);
            }
        }
        
        private static void Log(LogLevel level, Text t, TextColor color, Text name)
        {
            var _name = name.CloneAsBase();
            Logged?.Invoke(level, t, color, _name).ConfigureAwait(false);
                
            if (Level > level) return;
            _logLock.Wait();

            var f = PrefixFormat;
            _name.Color = color;
            var tag = TranslateText.Of("[%s]").AddWith(_name).SetColor(color);
            var now = DateTime.Now.ToString();
            Console.WriteLine(f.AddWith(tag, t, LiteralText.Of(now)).ToAscii());
                
            _logLock.Release();
        }

        public static void Log(Text t, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Log, t, TextColor.DarkGray, tag);
        }

        public static void Log(string msg, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            foreach (string line in msg.Split('\n'))
            {
                Log(LogLevel.Log, LiteralText.Of(line), TextColor.DarkGray, tag);
            }
        }

        public static void Verbose(Text t, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Verbose, t, TextColor.DarkGray, tag);
        }

        public static void Verbose(string msg, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            foreach (string line in msg.Split('\n'))
            {
                Log(LogLevel.Verbose, LiteralText.Of(line), TextColor.DarkGray, tag);
            }
        }

        public static void Info(Text t, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Info, t, TextColor.Green, tag);
        }

        public static void Info(string msg, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            foreach (string line in msg.Split('\n'))
            {
                Log(LogLevel.Info, LiteralText.Of(line), TextColor.Green, tag);
            }
        }

        public static void Warn(Text t, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Warn, t, TextColor.Gold, tag);
        }

        public static void Warn(string msg, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            foreach (string line in msg.Split('\n'))
            {
                Log(LogLevel.Warn, LiteralText.Of(line), TextColor.Gold, tag);
            }
        }

        public static void Error(Text t, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Error, t, TextColor.Red, tag);
        }

        public static void Error(string msg, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            foreach (string line in msg.Split('\n'))
            {
                Log(LogLevel.Error, LiteralText.Of(line), TextColor.Red, tag);
            }
        }

        [Obsolete("This method will be removed.", true)]
        public static Task WaitForActiveLogAsync() => Task.CompletedTask;

        [Obsolete("This method will be removed.", true)]
        public static Task FlushAsync() => Task.CompletedTask;
    }
}
