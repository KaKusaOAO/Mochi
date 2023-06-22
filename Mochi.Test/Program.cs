// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Mochi.Structs;
using Mochi.Utils;

Logger.Level = LogLevel.Verbose;
Logger.Logged += Logger.LogToEmulatedTerminalAsync;
Logger.RunThreaded();

var stopwatch = new Stopwatch();
stopwatch.Start();
var size = DataSize.MaxValue;
var elapsed = stopwatch.Elapsed;
var str = size.ToString();
var elapsed2 = stopwatch.Elapsed;
Logger.Info($"{str} ({size.TotalBytes:N0} bytes, {elapsed.TotalMilliseconds}ms, {elapsed2.TotalMilliseconds}ms)");
Logger.Flush();