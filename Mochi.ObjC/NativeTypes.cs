namespace Mochi.ObjC;

public static class NativeTypes
{
    public struct Bool8
    {
        public readonly byte Value;

        public Bool8(byte val)
        {
            Value = val;
        }

        public Bool8(bool val)
        {
            Value = (byte) (val ? 1 : 0);
        }

        public static implicit operator bool(Bool8 val) => val.Value != 0;
        public static implicit operator byte(Bool8 val) => val.Value;
        public static implicit operator Bool8(bool val) => new(val);
        public static implicit operator Bool8(byte val) => new(val);
    }
}