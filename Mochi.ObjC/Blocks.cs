using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mochi.ObjC;

[Flags]
public enum BlockFlags //: uint
{
    IsNoEscape = 1 << 23,
    HasCopyDispose = 1 << 25,
    HasCtor = 1 << 26,
    IsGlobal = 1 << 28,
    HasStRet = 1 << 29,
    HasSignature = 1 << 30
}

public unsafe struct BlockLiteral : IDisposable
{
    /// <summary>
    /// Initialized to <see cref="LibSystem.NSConcreteStackBlock" /> or <see cref="LibSystem.NSConcreteGlobalBlock" />.
    /// </summary>
    public IntPtr Isa;
    public BlockFlags Flags;
    public int Reserved;
    public IntPtr Invoke;
    public BlockDescriptor* Descriptor;

    public void Dispose()
    {
        var ptr = (IntPtr)Descriptor;
        if (ptr != 0)
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}

public unsafe struct BlockDescriptor
{
    /// <summary>
    /// A reserved field. Should be set to <c>0</c>.
    /// </summary>
    public ulong Reserved;
    /// <summary>
    /// Size of the <see cref="BlockLiteral"/>. Use <see cref="Unsafe.SizeOf"/> to get the size.
    /// </summary>
    public ulong Size;
    public IntPtr CopyHelper;
    public IntPtr DisposeHelper;
    public byte* Signature;
}

public unsafe struct Block : IDisposable
{
    public readonly BlockLiteral* BlockLiteral;

    // public static Block CreateStackBlock() => CreateBlock(LibSystem.NSConcreteStackBlock);
    public static Block CreateGlobalBlock() => CreateBlock(LibSystem.NSConcreteGlobalBlock);

    private static Block CreateBlock(IntPtr isa)
    {
        var blockDescriptor = (BlockDescriptor*)Marshal.AllocHGlobal(Unsafe.SizeOf<BlockDescriptor>());
        blockDescriptor->Reserved = 0;
        blockDescriptor->Size = (ulong) Unsafe.SizeOf<BlockLiteral>();

        var blockLiteral = (BlockLiteral*)Marshal.AllocHGlobal(Unsafe.SizeOf<BlockLiteral>());
        blockLiteral->Isa = isa;
        blockLiteral->Flags = BlockFlags.IsGlobal | BlockFlags.HasStRet;
        blockLiteral->Descriptor = blockDescriptor;

        return new Block(blockLiteral);
    }

    public Block(BlockLiteral* blockLiteral)
    {
        BlockLiteral = blockLiteral;
    }

    public void SetBlockCallback<T>(T func) where T : notnull
    {
        BlockLiteral->Invoke = Marshal.GetFunctionPointerForDelegate(func);
    }

    public void Dispose()
    {
        var ptr = (IntPtr)BlockLiteral;
        if (ptr != 0)
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}