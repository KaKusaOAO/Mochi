// See https://aka.ms/new-console-template for more information

using Mochi;
using Mochi.Concurrent;
using Mochi.IO;
using Mochi.Utils;
using Timeout = Mochi.Utils.Timeout;

Logger.Level = LogLevel.Verbose;
Logger.Logged += Logger.LogToEmulatedTerminalAsync;
Logger.RunThreaded();

const int value = -0x9876;
var writeStream = new MemoryStream();
var writer = new BufferWriter(writeStream);
writer.WriteVarInt(value);

var readStream = new MemoryStream(writeStream.ToArray());
var reader = new BufferReader(readStream);
var val = reader.ReadVarInt();
if (val == value)
{
#if NET7_0_OR_GREATER
    Logger.Info("Test passed with .NET 7.0 codebase");
#else
    Logger.Info("Test passed");
#endif
}
else
{
    Logger.Error($"Expected read int to be {value}, found {val}");
}