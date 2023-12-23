using Mochi.ObjC;

namespace Mochi.Metal;

public interface IMTLCommandEncoder : IMTLResourceLike
{
    /// <summary>
    /// Declare that all command generation from this encoder is complete,
    /// and detach from the <see cref="MTLCommandBuffer" />.
    /// </summary>
    public void EndEncoding();
}

public static class MTLCommandEncoderExtension
{
    public static MTLCommandEncoder AsMTLCommandEncoder<T>(this T encoder)
        where T : struct, INativeHandle<T>, IMTLCommandEncoder =>
        encoder.UnsafeCast<T, MTLCommandEncoder>();
}

public struct MTLCommandEncoder : IObjCNativeHandle<MTLCommandEncoder>, IMTLCommandEncoder
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLCommandEncoder>.Handle => Handle;
    public MTLCommandEncoder(IntPtr handle) => Handle = handle;

    public NSString Label
    {
        get => this.AsMTLResource().Label;
        set
        {
            var r = this.AsMTLResource();
            r.Label = value;
        }
    }

    public MTLDevice Device => this.AsMTLResource().Device;

    public void EndEncoding()
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<VoidDelegate>()(Handle, _selEndEncoding);
    }
    
    /// <summary>
    /// Inserts a debug string into the command buffer.
    /// This does not change any API behavior, but can be useful when debugging.
    /// </summary>
    /// <param name="str"></param>
    public void InsertDebugSignpost(NSString str)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<VoidStrDelegate>()(Handle, _selInsertDebugSignpost, str);
    }

    /// <summary>
    /// Push a new named string onto a stack of string labels.
    /// </summary>
    /// <param name="str"></param>
    public void PushDebugGroup(NSString str)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<VoidStrDelegate>()(Handle, _selPushDebugGroup, str);
    }

    /// <summary>
    /// Pop the latest named string off of the stack.
    /// </summary>
    public void PopDebugGroup()
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<VoidDelegate>()(Handle, _selPopDebugGroup);
    }

    private delegate void VoidStrDelegate(IntPtr handle, Selector sel, NSString str);
    private delegate void VoidDelegate(IntPtr handle, Selector sel);

    static MTLCommandEncoder INativeHandle<MTLCommandEncoder>.CreateWithHandle(IntPtr handle) => new(handle);

    private static readonly Selector _selEndEncoding = "endEncoding";
    private static readonly Selector _selInsertDebugSignpost = "insertDebugSignpost:";
    private static readonly Selector _selPushDebugGroup = "pushDebugGroup:";
    private static readonly Selector _selPopDebugGroup = "popDebugGroup";
}