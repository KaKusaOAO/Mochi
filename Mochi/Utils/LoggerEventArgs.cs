using System;
using System.Threading;
using Mochi.Texts;

namespace Mochi.Utils;

public class LoggerEventArgs : EventArgs
{
    public LogLevel Level { get; set; } = LogLevel.Verbose;
    public IComponent Content { get; set; } = Component.Literal("Log message not set");
    public IComponent Tag { get; set; } = Component.Literal("Unknown");
    public TextColor TagColor { get; set; } = TextColor.DarkGray;
    public Thread SourceThread { get; set; } = Thread.CurrentThread;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
}