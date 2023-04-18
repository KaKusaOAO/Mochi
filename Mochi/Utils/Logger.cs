using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mochi.Texts;

namespace Mochi.Utils;

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

    private static Type GetCallSourceType()
    {
        var stack = new StackTrace();
        var method = stack.GetFrame(2)?.GetMethod();
            
        // There are some rare case where the method is null.
        // For example in an Unity WebGL build where the stack trace is disabled,
        // the method information might be unavailable.
        if (method == null) return typeof(Logger);
            
        return GetRootType(method.DeclaringType!);
    }

    private static Type GetRootType(Type t)
    {
        while (true)
        {
            if (!t!.Name.StartsWith("<")) return t;
            t = t.DeclaringType!;
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
        var nameClone = name.MutableCopy();
        var f = PrefixFormat;
        nameClone.Color = color;
        var tag = LiteralText.Of($"[{thread.Name}@{thread.ManagedThreadId}] ")
            .SetColor(TextColor.DarkGray)
            .AddExtra(TranslateText.Of("[%s]").AddWith(nameClone).SetColor(color));
        var now = time.ToLocalTime().DateTime.ToString(CultureInfo.InvariantCulture);

        var text = t.Clone();
        var prefix = f.AddWith(tag, LiteralText.Of(""), LiteralText.Of(now));
            
        var pPlain = prefix.ToPlainText();
        var pf = ascii ? prefix.ToAnsi() : prefix.ToPlainText(); 
        var content = ascii ? text.ToAnsi() : text.ToPlainText();
        var lines = content.Split('\n');

        var remainPrefixPlain = "+ ->> ";
        var remainPrefix = (ascii ? TextColor.DarkGray.ToAnsiCode() : "") + remainPrefixPlain +
                           (ascii ? LegacyAnsiColor.Reset.ToAnsiCode() : "");
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

    private static IText CreateTextFromGeneric(object? obj)
    {
        return obj switch
        {
            null => LiteralText.Of("<null>").SetColor(TextColor.Red),
            IText text => text,
            Type type => Text.RepresentType(type),
            _ => LiteralText.Of(obj.ToString())
        };
    }

    public static void Log(object? content, object? name = null)
    {
        var tag = name ?? GetCallSourceType();
        Log(LogLevel.Log, CreateTextFromGeneric(content), TextColor.DarkGray, CreateTextFromGeneric(tag));
    }

    public static void Verbose(object? content, object? name = null)
    {
        var tag = name ?? GetCallSourceType();
        Log(LogLevel.Verbose, CreateTextFromGeneric(content), TextColor.DarkGray, CreateTextFromGeneric(tag));
    }

    public static void Info(object? content, object? name = null)
    {
        var tag = name ?? GetCallSourceType();
        Log(LogLevel.Info, CreateTextFromGeneric(content), TextColor.Green, CreateTextFromGeneric(tag));
    }

    public static void Warn(object? content, object? name = null)
    {
        var tag = name ?? GetCallSourceType();
        Log(LogLevel.Warn, CreateTextFromGeneric(content), TextColor.Gold, CreateTextFromGeneric(tag));
    }

    public static void Error(object? content, object? name = null)
    {
        var tag = name ?? GetCallSourceType();
        Log(LogLevel.Error, CreateTextFromGeneric(content), TextColor.Red, CreateTextFromGeneric(tag));
    }
}