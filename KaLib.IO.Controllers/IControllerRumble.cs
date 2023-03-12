namespace KaLib.IO.Controllers;

public interface IControllerRumble
{
    public float Amplitude { get; set; }
}

public interface IControllerExtendedRumble : IControllerRumble
{
    public float Frequency { get; set; }
}