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
        set => this.AsMTLResource().Label = value;
    }

    public MTLDevice Device => this.AsMTLResource().Device;

    public uint Length
    {
        get 
        { 
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<GetLengthDelegate>()(Handle, _selLength); 
        }
    }

    public IntPtr Contents
    {
        get
        {
            this.EnsureInstanceNotNull();
            return (IntPtr)ObjCRuntime.GetSendMessageFunction<GetContentsDelegate>()(Handle, _selContents);
        }
    }

    public void DidModifyRange(NSRange range)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<DidModifyRangeDelegate>()(Handle, _selDidModifyRange, range);
    }

    public void AddDebugMarker(NSString marker, NSRange range)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<AddDebugMarkerDelegate>()(Handle, _selAddDebugMarker, marker, range);
    }

    public void RemoveAllDebugMarkers()
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<RemoveAllDebugMarkersDelegate>()(Handle, _selRemoveAllDebugMarkers);
    }

    static MTLBuffer INativeHandle<MTLBuffer>.CreateWithHandle(IntPtr handle) => new(handle);

    private delegate uint GetLengthDelegate(IntPtr handle, Selector sel);
    private delegate void* GetContentsDelegate(IntPtr handle, Selector sel);
    private delegate void DidModifyRangeDelegate(IntPtr handle, Selector sel, NSRange range);
    private delegate void AddDebugMarkerDelegate(IntPtr handle, Selector sel, NSString marker, NSRange range);
    private delegate void RemoveAllDebugMarkersDelegate(IntPtr handle, Selector sel);
    
    private static readonly Selector _selLength = "length";
    private static readonly Selector _selContents = "contents";
    private static readonly Selector _selDidModifyRange = "didModifyRange:";
    private static readonly Selector _selAddDebugMarker = "addDebugMarker:range:";
    private static readonly Selector _selRemoveAllDebugMarkers = "removeAllDebugMarkers";
}