using KaLib.IO.Controllers.DualSense;
using KaLib.IO.Controllers.Switch;
using KaLib.IO.Controllers.XInput;
using KaLib.IO.Hid;

namespace KaLib.IO.Controllers;

public static class Controller
{
    public static IEnumerable<IController> FindAllControllers()
    {
        foreach (var n in DualSenseController.FindAllFreeDualSense())
        {
            yield return new DualSenseController(n);
        }

        foreach (var n in XInputController.FindAll())
        {
            yield return n;
        }
        
        var c = SwitchProController.OpenFirstProcon();
        if (c != null) yield return c;
    }

    public static void StartEventLoopThread(this IController controller)
    {
        var thread = new Thread(() =>
        {
            while (!controller.Disposed)
            {
                controller.PollInput();
                controller.UpdateStates();
                Thread.Sleep(2);
            }
        })
        {
            Name = $"{controller.GetType().Name}-IOThread",
            Priority = ThreadPriority.Highest
        };
        thread.Start();
    }
}