// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Mochi.Structs;
using Mochi.Texts;
using Mochi.Utils;

Logger.Level = LogLevel.Verbose;
Logger.Logged += Logger.LogToEmulatedTerminalAsync;
Logger.RunThreaded();

var json = JsonValue.Create("\u00a76\u00a7lTest \u00a7r\u00a7cComponent");
var component = Component.FromJson(json);
Logger.Info(component);