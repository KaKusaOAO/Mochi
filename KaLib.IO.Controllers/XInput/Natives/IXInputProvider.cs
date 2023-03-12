namespace KaLib.IO.Controllers.XInput.Natives;

public interface IXInputProvider
{
    public XInputState GetState(PlayerIndex index);
    public void SetState(PlayerIndex index, float leftMotor, float rightMotor);
}