using KaLib.IO.Hid;

namespace KaLib.IO.Controllers.Switch;

public class SwitchController // : IBasicController
{
    private readonly HidDevice _device;

    public SwitchController(HidDevice device)
    {
        _device = device;
    }

    public void Update()
    {
        throw new NotImplementedException();
    }
}