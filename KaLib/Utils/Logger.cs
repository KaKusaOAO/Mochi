using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using KaLib.Texts;

namespace KaLib.Utils
{
    public delegate Task AsyncLogDelegate(LogLevel level, IText text, TextColor color, IText name);

    public static class Logger
    {
        public static event AsyncLogDelegate Logged;
        
        public static LogLevel Level { get; set; }

        private const string DefaultName = null;

        public static TranslateText PrefixFormat => TranslateText.Of("{2} - {0} {1}");

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
        }
        
        private static void Log(LogLevel level, IText t, TextColor color, IText name)
        {
            Logged?.Invoke(level, t.Clone(), color, name.Clone()).ConfigureAwait(false);
        }

        public static IText GetDefaultFormattedLine(IText t, TextColor color, IText name)
        {
            var _name = name.MutableCopy();
            var f = PrefixFormat;
            _name.Color = color;
            var tag = TranslateText.Of("[%s]").AddWith(_name).SetColor(color);
            var now = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            return f.AddWith(tag, t.Clone(), LiteralText.Of(now));
        }

        public static async Task LogToConsoleAsync(LogLevel level, IText t, TextColor color, IText name)
        {
            if (Level > level) return;
            await _logLock.WaitAsync();
            Console.WriteLine(GetDefaultFormattedLine(t, color, name).ToAscii());
            _logLock.Release();
        }
        
        public static async Task LogToEmulatedTerminalAsync(LogLevel level, IText t, TextColor color, IText name)
        {
            if (Level > level) return;
            await _logLock.WaitAsync();
            Terminal.WriteLineStdOut(GetDefaultFormattedLine(t, color, name).ToAscii());
            _logLock.Release();
        }

        public static void Log(IText t, string name = DefaultName)
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

        public static void Verbose(IText t, string name = DefaultName)
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

        public static void Info(IText t, string name = DefaultName)
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

        public static void Warn(IText t, string name = DefaultName)
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

        public static void Error(IText t, string name = DefaultName)
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
