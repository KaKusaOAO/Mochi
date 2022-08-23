using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using KaLib.Texts;

namespace KaLib.Utils
{
    public class LoggerEventArgs : EventArgs
    {
        public LogLevel Level { get; set; } = LogLevel.Verbose;
        public IText Content { get; set; } = LiteralText.Of("Log message not set");
        public IText Tag { get; set; } = LiteralText.Of("Unknown");
        public TextColor TagColor { get; set; } = TextColor.DarkGray;
        public Thread SourceThread { get; set; } = Thread.CurrentThread;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    }
    
    public delegate Task AsyncLogEventDelegate(LoggerEventArgs data);

    public static class Logger
    {
        public static event AsyncLogEventDelegate Logged;
        
        public static LogLevel Level { get; set; }

        private const string DefaultName = null;

        public static TranslateText PrefixFormat => TranslateText.Of("{2} - {0} {1}");

        private static SemaphoreSlim _logLock = new(1, 1);

        private static readonly ConcurrentQueue<Action> _recordCall = new();

        private static Thread _thread;
        private static bool _isThreaded;
        private static bool _bootstrapped;

        static Logger()
        {
#if DEBUG
            Level = LogLevel.Verbose;
#else
            Level = LogLevel.Info;
#endif
        }

        public static void RunThreaded()
        {
            if (_bootstrapped) return;
            _bootstrapped = true;
            _isThreaded = true;
            
            var thread = new Thread(RunEventLoop)
            {
                Name = "Logger Thread"
            };
            _thread = thread;
            thread.Start();
        }
        
        private static void RunEventLoop() 
        {
            while (_bootstrapped)
            {
                SpinWait.SpinUntil(() => !_recordCall.IsEmpty);
                PollEvents();
                Thread.Yield();
            }
        }
        
        public static void RunManualPoll()
        {
            if (_bootstrapped) return;
            _bootstrapped = true;
            _thread = Thread.CurrentThread;
        }
        
        public static void RunBlocking()
        {
            if (_bootstrapped) return;
            _bootstrapped = true;
            _thread = Thread.CurrentThread;
            RunEventLoop();
        }

        public static void PollEvents()
        {
            if (!_bootstrapped)
                throw new Exception("Logger is not bootstrapped");
            if (_thread != Thread.CurrentThread)
                throw new Exception("PollEvents() called from wrong thread");

            while (_recordCall.TryDequeue(out var action))
            {
                action();
                Thread.Yield();
            }
        }
        
        private static void InternalOnLogged(LoggerEventArgs data)
        {
            Logged?.Invoke(data).ConfigureAwait(false);
        }
        
        private static void CallOrQueue(Action action)
        {
            if (!_bootstrapped)
            {
                RunThreaded();
                SpinWait.SpinUntil(() => _thread != null);
                
                var d = new LoggerEventArgs
                {
                    Level = LogLevel.Warn,
                    Content = LiteralText.Of("*** Logger is not bootstrapped. ***"),
                    TagColor = TextColor.Gold,
                    SourceThread = _thread,
                    Tag = Text.RepresentType(typeof(Logger))
                };
                InternalOnLogged(d);

                d = new LoggerEventArgs
                {
                    Level = LogLevel.Warn,
                    Content = LiteralText.Of(
                        "Logger now requires either RunThreaded(), RunBlocking() or RunManualPoll() to poll log events."),
                    TagColor = TextColor.Gold,
                    SourceThread = _thread,
                    Tag = Text.RepresentType(typeof(Logger))
                };
                InternalOnLogged(d);
                
                d = new LoggerEventArgs
                {
                    Level = LogLevel.Warn,
                    Content = LiteralText.Of(
                        "The threaded approach will be used by default."),
                    TagColor = TextColor.Gold,
                    SourceThread = _thread,
                    Tag = Text.RepresentType(typeof(Logger))
                };
                InternalOnLogged(d);
            }

            if (!_bootstrapped) throw new Exception("Logger is not bootstrapped.");
            
            if (Thread.CurrentThread != _thread)
            {
                _recordCall.Enqueue(action);
            }
            else
            {
                action(); 
            }
        }

        private static string GetCallSourceName()
        {
            string name;
            var stack = new StackTrace();
            var method = stack.GetFrame(2)?.GetMethod();
            name = method == null ? "?" : GetRootType(method.DeclaringType).FullName;
            return name;
        }

        private static Type GetCallSourceType()
        {
            var stack = new StackTrace();
            var method = stack.GetFrame(2)?.GetMethod();
            return GetRootType(method!.DeclaringType);
        }

        private static Type GetRootType(Type t)
        {
            while (true)
            {
                if (!t!.Name.StartsWith("<")) return t;
                t = t.DeclaringType;
            }
        }

        private static void Log(LogLevel level, IText t, TextColor color, IText name)
        {
            var thread = Thread.CurrentThread;
            var tClone = t.Clone();
            var nameClone = name.Clone();
            var data = new LoggerEventArgs
            {
                Content = tClone,
                Tag = nameClone,
                TagColor = color,
                SourceThread = thread,
                Level = level
            };
            CallOrQueue(() => InternalOnLogged(data));
        }

        public static IText GetDefaultFormattedLine(DateTimeOffset time, IText t, TextColor color, IText name, Thread thread)
        {
            var _name = name.MutableCopy();
            var f = PrefixFormat;
            _name.Color = color;
            var tag = LiteralText.Of($"[{thread.Name}@{thread.ManagedThreadId}] ")
                .SetColor(TextColor.DarkGray)
                .AddExtra(TranslateText.Of("[%s]").AddWith(_name).SetColor(color));
            var now = time.ToLocalTime().DateTime.ToString(CultureInfo.InvariantCulture);
            return f.AddWith(tag, t.Clone(), LiteralText.Of(now));
        }

        public static async Task LogToConsoleAsync(LoggerEventArgs data)
        {
            var level = data.Level;
            if (Level > level) return;
            
            var t = data.Content;
            var color = data.TagColor;
            var name = data.Tag;
            
            // await _logLock.WaitAsync();
            Console.WriteLine(GetDefaultFormattedLine(data.Timestamp, t, color, name, data.SourceThread).ToAscii());
            // _logLock.Release();
        }
        
        public static async Task LogToEmulatedTerminalAsync(LoggerEventArgs data)
        {
            var level = data.Level;
            if (Level > level) return;
            
            var t = data.Content;
            var color = data.TagColor;
            var name = data.Tag;
            
            // await _logLock.WaitAsync();
            Terminal.WriteLineStdOut(GetDefaultFormattedLine(data.Timestamp, t, color, name, data.SourceThread).ToAscii());
            // _logLock.Release();
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
