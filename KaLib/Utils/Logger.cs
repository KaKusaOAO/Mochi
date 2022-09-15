using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
        private static readonly AsyncEventHandler<AsyncLogEventDelegate> _loggedHandler = new();
        public static event AsyncLogEventDelegate Logged
        {
            add => _loggedHandler.AddHandler(value);
            remove => _loggedHandler.RemoveHandler(value);
        }
        
        public static LogLevel Level { get; set; }

        private const string DefaultName = null;

        public static TranslateText PrefixFormat => TranslateText.Of("{2} - {0} {1}");

        private static readonly SemaphoreSlim _logLock = new(1, 1);
        private static readonly ConcurrentQueue<Action> _recordCall = new();
        private static Thread _thread;
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

            var thread = new Thread(RunEventLoop)
            {
                Name = "Logger Thread",
                IsBackground = true
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
                try
                {
                    action();
                }
                catch (Exception)
                {
                    // Ignored
                }
                Thread.Yield();
            }
        }
        
        private static void InternalOnLogged(LoggerEventArgs data)
        {
            foreach (var handler in _loggedHandler.Handlers)
            {
                void HandleException(Exception ex)
                {
                    _ = LogToEmulatedTerminalAsync(new LoggerEventArgs
                    {
                        Content = TranslateText.Of("Unhandled exception in handler %s!")
                            .AddWith(LiteralText.Of($"{handler.Method}").SetColor(TextColor.Gold)),
                        Level = LogLevel.Error,
                        SourceThread = _thread,
                        Tag = LiteralText.Of("Logger"),
                        TagColor = TextColor.Red,
                        Timestamp = DateTimeOffset.Now
                    });
                    
                    _ = LogToEmulatedTerminalAsync(new LoggerEventArgs
                    {
                        Content = LiteralText.Of($"{ex}"),
                        Level = LogLevel.Error,
                        SourceThread = _thread,
                        Tag = LiteralText.Of("Logger"),
                        TagColor = TextColor.Red,
                        Timestamp = DateTimeOffset.Now
                    });
                }

                try
                {
                    var task = handler(data);
                    Common.DiscardAndCatch(task, HandleException);
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
                
            }
        }

        public static void Flush()
        {
            var called = false;
            CallOrQueue(() => called = true);
            SpinWait.SpinUntil(() => called);
        }
        
        public static Task FlushAsync() => Task.Run(Flush);

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

            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                Flush();
            };
            
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

        public static List<string> GetDefaultFormattedLines(DateTimeOffset time, IText t, TextColor color, IText name, Thread thread, bool ascii = true)
        {
            var _name = name.MutableCopy();
            var f = PrefixFormat;
            _name.Color = color;
            var tag = LiteralText.Of($"[{thread.Name}@{thread.ManagedThreadId}] ")
                .SetColor(TextColor.DarkGray)
                .AddExtra(TranslateText.Of("[%s]").AddWith(_name).SetColor(color));
            var now = time.ToLocalTime().DateTime.ToString(CultureInfo.InvariantCulture);

            var text = t.Clone();
            var prefix = f.AddWith(tag, LiteralText.Of(""), LiteralText.Of(now));
            
            var pPlain = prefix.ToPlainText();
            var pf = ascii ? prefix.ToAscii() : prefix.ToPlainText(); 
            var content = ascii ? text.ToAscii() : text.ToPlainText();
            var lines = content.Split('\n');

            var remainPrefixPlain = "+ ->> ";
            var remainPrefix = (ascii ? AsciiColor.DarkGray.ToAsciiCode() : "") + remainPrefixPlain +
                               (ascii ? AsciiColor.Reset.ToAsciiCode() : "");
            return lines.Take(1).Select(c => pf + c)
                .Concat(lines.Skip(1).Select(c => (remainPrefix + c).PadLeft(c.Length + pPlain.Length + remainPrefix.Length - remainPrefixPlain.Length))).ToList();
        }

        public static async Task LogToConsoleAsync(LoggerEventArgs data)
        {
            var level = data.Level;
            if (Level > level) return;
            
            var t = data.Content;
            var color = data.TagColor;
            var name = data.Tag;

            await Common.AcquireSemaphoreAsync(_logLock, () =>
            {
                foreach (var line in GetDefaultFormattedLines(data.Timestamp, t, color, name, data.SourceThread))
                {
                    Console.WriteLine(line);
                }
            });
        }
        
        public static async Task LogToEmulatedTerminalAsync(LoggerEventArgs data)
        {
            var level = data.Level;
            if (Level > level) return;
            
            var t = data.Content;
            var color = data.TagColor;
            var name = data.Tag;

            await Common.AcquireSemaphoreAsync(_logLock, () =>
            {
                foreach (var line in GetDefaultFormattedLines(data.Timestamp, t, color, name, data.SourceThread))
                {
                    Terminal.WriteLineStdOut(line);
                }
            });
        }

        public static void Log(IText t, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Log, t, TextColor.DarkGray, tag);
        }

        public static void Log(string msg, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Log, LiteralText.Of(msg), TextColor.DarkGray, tag);
        }

        public static void Verbose(IText t, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Verbose, t, TextColor.DarkGray, tag);
        }

        public static void Verbose(string msg, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Verbose, LiteralText.Of(msg), TextColor.DarkGray, tag);
        }

        public static void Info(IText t, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Info, t, TextColor.Green, tag);
        }

        public static void Info(string msg, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Info, LiteralText.Of(msg), TextColor.Green, tag);
        }

        public static void Warn(IText t, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Warn, t, TextColor.Gold, tag);
        }

        public static void Warn(string msg, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Warn, LiteralText.Of(msg), TextColor.Gold, tag);
        }

        public static void Error(IText t, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Error, t, TextColor.Red, tag);
        }

        public static void Error(string msg, string name = DefaultName)
        {
            var tag = name == null ? Text.RepresentType(GetCallSourceType()) : LiteralText.Of(name);
            Log(LogLevel.Error, LiteralText.Of(msg), TextColor.Red, tag);
        }
    }
}
