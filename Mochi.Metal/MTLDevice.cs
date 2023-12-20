using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Mochi.ObjC;

namespace Mochi.Metal;

[StructLayout(LayoutKind.Sequential)]
public struct MTLDevice : INativeHandle<MTLDevice>
{
    private const string MetalFramework = "/System/Library/Frameworks/Metal.framework/Metal";
    
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLDevice>.Handle => Handle;

    public MTLDevice(IntPtr handle) => Handle = handle;

    public NSString Name
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSStringDelegate>()(Handle,
                CommonSelectors.Name);
        }
    }

    public MTLArchitecture Architecture
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetArchitectureDelegate>()(Handle,
                _selGetArchitecture);
        }
    }

    /// <summary>
    /// On systems that support automatic graphics switching, this will return <c>true</c> for the the low power device.
    /// </summary>
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [UnsupportedOSPlatform("ios")]
    public bool IsLowPower
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle, _selGetIsLowPower);
        }
    }

    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [UnsupportedOSPlatform("ios")]
    public bool IsHeadless
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle, _selGetIsHeadless);
        }
    }

    [SupportedOSPlatform("macos10.13")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [UnsupportedOSPlatform("ios")]
    public bool IsRemovable
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle, _selGetIsRemovable);
        }
    }

    [SupportedOSPlatform("macos10.15")]
    [SupportedOSPlatform("ios13.0")]
    public bool HasUnifiedMemory
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle,
                _selGetHasUnifiedMemory);
        }
    }

    [SupportedOSPlatform("macos10.15")]
    [UnsupportedOSPlatform("ios")]
    public MTLDeviceLocation Location
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetDeviceLocationDelegate>()(Handle,
                _selGetLocation);
        }
    }

    public MTLCommandQueue CreateCommandQueue()
    {
        this.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<NewCommandQueueDelegate>()(Handle, _selNewCommandQueue);
    }

    public MTLBuffer CreateBuffer(uint length, MTLResourceOptions options)
    {
        // Creating a zero-length buffer results in a null MTLBuffer.
        this.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<NewBufferWithLengthDelegate>()(Handle, _selNewBufferWithLength,
            length, options);
    }
    
    public MTLBuffer CreateBuffer(IntPtr buffer, uint length, MTLResourceOptions options)
    {
        // Creating a zero-length buffer results in a null MTLBuffer.
        this.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<NewBufferWithBytesDelegate>()(Handle, _selNewBufferWithLength,
            buffer, length, options);
    }

    public MTLTexture CreateTexture(MTLTextureDescriptor descriptor)
    {
        this.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<NewTextureWithDescriptorDelegate>()(Handle,
            _selNewTextureWithDescriptor, descriptor);
    }

    private delegate MTLCommandQueue NewCommandQueueDelegate(IntPtr handle, Selector sel);
    private delegate MTLBuffer NewBufferWithLengthDelegate(IntPtr handle, Selector sel, uint length, MTLResourceOptions options);
    private delegate MTLBuffer NewBufferWithBytesDelegate(IntPtr handle, Selector sel, IntPtr buffer, uint length, MTLResourceOptions options);
    private delegate MTLTexture NewTextureWithDescriptorDelegate(IntPtr handle, Selector sel, MTLTextureDescriptor descriptor);
    
    // public static ObjCClass RuntimeClass { get; } = nameof(MTLDevice);
    static MTLDevice INativeHandle<MTLDevice>.CreateWithHandle(IntPtr handle) => new(handle);

    private static readonly Selector _selGetArchitecture = "architecture";
    private static readonly Selector _selGetIsLowPower = "isLowPower";
    private static readonly Selector _selGetIsHeadless = "isHeadless";
    private static readonly Selector _selGetIsRemovable = "isRemovable";
    private static readonly Selector _selGetHasUnifiedMemory = "hasUnifiedMemory";
    private static readonly Selector _selGetLocation = "location";
    private static readonly Selector _selNewCommandQueue = "newCommandQueue";
    private static readonly Selector _selNewCommandQueueWithMaxCBCount = "newCommandQueueWithMaxCommandBufferCount:";
    private static readonly Selector _selNewBufferWithLength = "newBufferWithLength:options:";
    private static readonly Selector _selNewBufferWithBytes = "newBufferWithBytes:length:options:";
    private static readonly Selector _selNewBufferWithBytesNoCopy = "newBufferWithBytesNoCopy:length:options:deallocator:";
    private static readonly Selector _selNewTextureWithDescriptor = "newTextureWithDescriptor:";

    [DllImport(MetalFramework, EntryPoint = "MTLCreateSystemDefaultDevice")]
    public static extern MTLDevice CreateSystemDefaultDevice();

    [DllImport(MetalFramework, EntryPoint = "MTLCopyAllDevices")]
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [UnsupportedOSPlatform("ios")]
    public static extern NSArray<MTLDevice> CopyAllDevices();

    private static unsafe void EnsureLoadedFramework(string name)
    {
        var arr = Encoding.UTF8.GetBytes($"/System/Library/Frameworks/{name}.framework/{name}");
        fixed (byte* namePtr = arr)
        {
            var handle = LibDl.dlopen(namePtr, LibDl.OpenFlags.Now);
            if (handle != 0)
            {
                _ = LibDl.dlclose(handle);
            }
        }
    }
    
    static MTLDevice()
    {
        // Ensure CoreGraphics has been loaded at least once, so MTLCreateSystemDefaultDevice() works
        EnsureLoadedFramework("CoreGraphics");
            
        // Ensure MetalFX has been loaded at least once, so the created devices supports MetalFX
        EnsureLoadedFramework("MetalFX");
    }
}