namespace KaLib.IO.Controllers;

public interface IControllerExtendedRumble : IControllerRumble
{
    public float Frequency { get; set; }
}

public interface IControllerRumble
{
    public float Amplitude { get; set; }
}