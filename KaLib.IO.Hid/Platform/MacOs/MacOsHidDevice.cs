namespace KaLib.IO.Hid.Platform.MacOs;

public class MacOsHidDevice : HidDevice
{
    public override int Write(byte[] data)
    {
        throw new System.NotImplementedException();
    }

    public override int Read(byte[] output)
    {
        throw new System.NotImplementedException();
    }

    public override int Read(byte[] output, int millis)
    {
        throw new System.NotImplementedException();
    }

    public override int SetBlocking(bool blocking)
    {
        throw new System.NotImplementedException();
    }

    public override int SendFeatureReport(byte[] data)
    {
        throw new System.NotImplementedException();
    }

    public override int GetFeatureReport(byte[] output)
    {
        throw new System.NotImplementedException();
    }

    public override int GetInputReport(byte[] output)
    {
        throw new System.NotImplementedException();
    }

    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }

    protected override string InternalGetManufacturer()
    {
        throw new System.NotImplementedException();
    }

    protected override string InternalGetProduct()
    {
        throw new System.NotImplementedException();
    }

    protected override string InternalGetSerialNumber()
    {
        throw new System.NotImplementedException();
    }
}