using System.Runtime.Versioning;
using Mochi.Metal;
using Mochi.ObjC;

namespace Mochi.MetalFX;

public struct MTLFXTemporalScalerDescriptor : IObjCInterface<MTLFXTemporalScalerDescriptor>
{
    private readonly IntPtr Handle;
    IntPtr INativeHandle<MTLFXTemporalScalerDescriptor>.Handle => Handle;

    public MTLFXTemporalScalerDescriptor(IntPtr handle) => Handle = handle;

    public static MTLFXTemporalScalerDescriptor AllocInit()
    {
        if (!IsOnSupportedOS)
            throw new PlatformNotSupportedException();
        
        return RuntimeClass.AllocInit<MTLFXTemporalScalerDescriptor>();
    }

    public MTLPixelFormat ColorTextureFormat
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetPixelFormatDelegate>()(Handle,
                _selGetColorTextureFormat);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetPixelFormatDelegate>()(Handle,
                _selSetColorTextureFormat, value);
        }
    }

    public MTLPixelFormat DepthTextureFormat
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetPixelFormatDelegate>()(Handle,
                _selGetDepthTextureFormat);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetPixelFormatDelegate>()(Handle,
                _selSetDepthTextureFormat, value);
        }
    }

    public MTLPixelFormat MotionTextureFormat
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetPixelFormatDelegate>()(Handle,
                _selGetMotionTextureFormat);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetPixelFormatDelegate>()(Handle,
                _selSetMotionTextureFormat, value);
        }
    }

    public MTLPixelFormat OutputTextureFormat
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetPixelFormatDelegate>()(Handle,
                _selGetOutputTextureFormat);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetPixelFormatDelegate>()(Handle,
                _selSetOutputTextureFormat, value);
        }
    }

    public uint InputWidth
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetUInt32Delegate>()(Handle, _selGetInputWidth);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetUInt32Delegate>()(Handle, _selSetInputWidth, value);
        }
    }

    public uint InputHeight
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetUInt32Delegate>()(Handle,
                _selGetInputHeight);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetUInt32Delegate>()(Handle, _selSetInputHeight,
                value);
        }
    }

    public uint OutputWidth
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetUInt32Delegate>()(Handle,
                _selGetOutputWidth);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetUInt32Delegate>()(Handle, _selSetOutputWidth,
                value);
        }
    }

    public uint OutputHeight
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetUInt32Delegate>()(Handle,
                _selGetOutputHeight);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetUInt32Delegate>()(Handle, _selSetOutputHeight,
                value);
        }
    }

    public MTLFXTemporalScaler CreateTemporalScaler(MTLDevice device)
    {
        this.EnsureInstanceNotNull();
        
        if (OutputWidth == 0)
            throw new MetalException("The output width for the temporal scaler cannot be 0.");
        
        if (OutputHeight == 0)
            throw new MetalException("The output height for the temporal scaler cannot be 0.");
        
        // Input size with zeros cause the result value to be null,
        // but it is not validated by the Metal validation layer itself
        
        return ObjCRuntime.GetSendMessageFunction<NewTemporalScalerDelegate>()(Handle,
            _selNewTemporalScalerWithDevice, 
            device);
    }

    [SupportedOSPlatform("macos14.0")]
    [SupportedOSPlatform("ios17.0")]
    public static float GetSupportedInputContentMinScale(MTLDevice device)
    {
        if (!OperatingSystem.IsMacOSVersionAtLeast(14) && !OperatingSystem.IsIOSVersionAtLeast(17))
            throw new PlatformNotSupportedException();
        
        return ObjCRuntime.GetSendMessageFunction<SupportsInputContentScaleForDeviceDelegate>()(RuntimeClass.Handle,
            _selSupportedInputContentMinScaleForDevice, device);
    }

    [SupportedOSPlatform("macos14.0")]
    [SupportedOSPlatform("ios17.0")]
    public static float GetSupportedInputContentMaxScale(MTLDevice device)
    {
        if (!OperatingSystem.IsMacOSVersionAtLeast(14) && !OperatingSystem.IsIOSVersionAtLeast(17))
            throw new PlatformNotSupportedException();
        
        return ObjCRuntime.GetSendMessageFunction<SupportsInputContentScaleForDeviceDelegate>()(RuntimeClass.Handle,
            _selSupportedInputContentMaxScaleForDevice, device);
    }

    [SupportedOSPlatformGuard("macos13.0")]
    [SupportedOSPlatformGuard("ios16.0")]
    private static bool IsOnSupportedOS =>
        OperatingSystem.IsMacOSVersionAtLeast(13) || OperatingSystem.IsIOSVersionAtLeast(16);

    public static bool IsDeviceSupported(MTLDevice device)
    {
        if (!IsOnSupportedOS) return false;
        return ObjCRuntime.GetSendMessageFunction<SupportsDeviceDelegate>()(RuntimeClass.Handle, _selSupportsDevice,
            device);
    }

    private delegate MTLFXTemporalScaler NewTemporalScalerDelegate(IntPtr handle, Selector sel, MTLDevice device);
    private delegate NativeTypes.Bool8 SupportsDeviceDelegate(IntPtr handle, Selector sel, MTLDevice device);
    private delegate float SupportsInputContentScaleForDeviceDelegate(IntPtr handle, Selector sel, MTLDevice device);
    
    public static ObjCClass RuntimeClass { get; } = nameof(MTLFXTemporalScalerDescriptor);
    static MTLFXTemporalScalerDescriptor INativeHandle<MTLFXTemporalScalerDescriptor>.CreateWithHandle(IntPtr handle) =>
        new(handle);

    private static readonly Selector _selGetColorTextureFormat = "colorTextureFormat";
    private static readonly Selector _selSetColorTextureFormat = "setColorTextureFormat:";
    private static readonly Selector _selGetDepthTextureFormat = "depthTextureFormat";
    private static readonly Selector _selSetDepthTextureFormat = "setDepthTextureFormat:";
    private static readonly Selector _selGetMotionTextureFormat = "motionTextureFormat";
    private static readonly Selector _selSetMotionTextureFormat = "setMotionTextureFormat:";
    private static readonly Selector _selGetOutputTextureFormat = "outputTextureFormat";
    private static readonly Selector _selSetOutputTextureFormat = "setOutputTextureFormat:";

    private static readonly Selector _selGetInputWidth = "inputWidth";
    private static readonly Selector _selSetInputWidth = "setInputWidth:";
    private static readonly Selector _selGetInputHeight = "inputHeight";
    private static readonly Selector _selSetInputHeight = "setInputHeight:";
    private static readonly Selector _selGetOutputWidth = "outputWidth";
    private static readonly Selector _selSetOutputWidth = "setOutputWidth:";
    private static readonly Selector _selGetOutputHeight = "outputHeight";
    private static readonly Selector _selSetOutputHeight = "setOutputHeight:";
    
    private static readonly Selector _selNewTemporalScalerWithDevice = "newTemporalScalerWithDevice:";
    private static readonly Selector _selSupportedInputContentMinScaleForDevice = "supportedInputContentMinScaleForDevice:";
    private static readonly Selector _selSupportedInputContentMaxScaleForDevice = "supportedInputContentMaxScaleForDevice:";
    private static readonly Selector _selSupportsDevice = "supportsDevice:";
}