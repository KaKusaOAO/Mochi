using Mochi.ObjC;

namespace Mochi.Metal;

public unsafe struct MTLBuffer : INativeHandle<MTLBuffer>, IMTLResource
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLBuffer>.Handle => Handle;

    public bool IsNull => Handle == 0;

    public MTLBuffer(IntPtr handle) => Handle = handle;

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

    public uint Length
    {
        get 
        { 
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<GetLengthDelegate>()(Handle, "length"); 
        }
    }

    public IntPtr Contents
    {
        get
        {
            this.EnsureInstanceNotNull();
            return (IntPtr)ObjCRuntime.GetSendMessageFunction<GetContentsDelegate>()(Handle, "contents");
        }
    }

    public void DidModifyRange(NSRange range)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<DidModifyRangeDelegate>()(Handle, "didModifyRange:", range);
    }

    public void AddDebugMarker(NSString marker, NSRange range)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<AddDebugMarkerDelegate>()(Handle, "addDebugMarker:range:", marker, range);
    }

    public void RemoveAllDebugMarkers()
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<RemoveAllDebugMarkersDelegate>()(Handle, "removeAllDebugMarkers");
    }

    static MTLBuffer INativeHandle<MTLBuffer>.CreateWithHandle(IntPtr handle) => new(handle);

    private delegate uint GetLengthDelegate(IntPtr handle, Selector sel);
    private delegate void* GetContentsDelegate(IntPtr handle, Selector sel);
    private delegate void DidModifyRangeDelegate(IntPtr handle, Selector sel, NSRange range);
    private delegate void AddDebugMarkerDelegate(IntPtr handle, Selector sel, NSString marker, NSRange range);
    private delegate void RemoveAllDebugMarkersDelegate(IntPtr handle, Selector sel);
}