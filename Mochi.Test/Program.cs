// See https://aka.ms/new-console-template for more information

using System.IO.Compression;
using Mochi.Nbt;
using Mochi.Utils;

Logger.Level = LogLevel.Verbose;
Logger.Logged += Logger.LogToEmulatedTerminalAsync;
Logger.RunThreaded();

using var stream = File.OpenRead("bigtest.nbt");
using var compressed = new GZipStream(stream, CompressionMode.Decompress);
var tag = NbtTag.Parse(compressed, true).AsCompound();

Logger.Info($"Read NBT: {tag}");