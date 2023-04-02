namespace KaLib.IO.Controllers.DualSense;

public class DualSenseRumble : IControllerExtendedRumble
{
    public float Amplitude { get; set; }
    public float Frequency { get; set; }
}