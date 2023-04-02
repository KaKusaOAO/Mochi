// See https://aka.ms/new-console-template for more information

using Mochi;
using Mochi.Concurrent;
using Mochi.Utils;
using Timeout = Mochi.Utils.Timeout;

Logger.Level = LogLevel.Verbose;
Logger.Logged += Logger.LogToEmulatedTerminalAsync;
Logger.RunThreaded();

var resolved = false;

await MochiLib.Platform.CreatePromise<int>((resolve, reject) =>
{
    Logger.Info("Wait for 3 seconds...");
    Timeout.CreateTimeout(() =>
    {
        var doResolve = Random.Shared.Next(2) == 0;
        if (doResolve)
        {
            Logger.Info("Resolving with a random number...");
            resolve(Random.Shared.Next(100));
        }
        else
        {
            Logger.Info("Rejecting with an exception...");
            reject(new Exception("Test exception!"));
        }
    }, 3000);
}).Then(i =>
{
    Logger.Info($"Resolved with number {i}.");
    return "Hello";
}).Then(s =>
{
    Logger.Info($"Resolved with a string {s}.");
}).Then(() =>
{
    Logger.Info($"Resolved with nothing.");
    resolved = true;
}).Catch(ex =>
{
    Logger.Info($"Caught an exception in Promise: {ex}");
}).Then(() =>
{
    Logger.Info("Run the final callback.");
}).AsTask();

Logger.Info("Awaited the task");