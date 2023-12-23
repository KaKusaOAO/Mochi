using System.Runtime.Versioning;
using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLRenderPipelineColorAttachmentDescriptor : IObjCInterface<MTLRenderPipelineColorAttachmentDescriptor>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLRenderPipelineColorAttachmentDescriptor>.Handle => Handle;

    public MTLRenderPipelineColorAttachmentDescriptor(IntPtr handle)
    {
        Handle = handle;
    }

    public MTLPixelFormat PixelFormat
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetPixelFormatDelegate>()(Handle,
                _propPixelFormat.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetPixelFormatDelegate>()(Handle,
                _propPixelFormat.Setter, value);
        }
    }

    public MTLColorWriteMask WriteMask
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetWriteMaskDelegate>()(Handle,
                _propWriteMask.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetWriteMaskDelegate>()(Handle,
                _propWriteMask.Setter, value);
        }
    }

    public bool IsBlendingEnabled
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle,
                _propBlendingEnabled.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBoolDelegate>()(Handle,
                _propBlendingEnabled.Setter, value);
        }
    }

    public MTLBlendOperation AlphaBlendOperation
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBlendOperationDelegate>()(Handle,
                _propAlphaBlendOperation.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBlendOperationDelegate>()(Handle,
                _propAlphaBlendOperation.Setter, value);
        }
    }

    public MTLBlendOperation RgbBlendOperation
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBlendOperationDelegate>()(Handle,
                _propRgbBlendOperation.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBlendOperationDelegate>()(Handle,
                _propRgbBlendOperation.Setter, value);
        }
    }

    public MTLBlendFactor DestinationAlphaBlendFactor
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBlendFactorDelegate>()(Handle,
                _propDestinationAlphaBlendFactor.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBlendFactorDelegate>()(Handle,
                _propDestinationAlphaBlendFactor.Setter, value);
        }
    }
    
    public MTLBlendFactor DestinationRgbBlendFactor
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBlendFactorDelegate>()(Handle,
                _propDestinationRGBBlendFactor.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBlendFactorDelegate>()(Handle,
                _propDestinationRGBBlendFactor.Setter, value);
        }
    }

    public MTLBlendFactor SourceAlphaBlendFactor
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBlendFactorDelegate>()(Handle,
                _propSourceAlphaBlendFactor.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBlendFactorDelegate>()(Handle,
                _propSourceAlphaBlendFactor.Setter, value);
        }
    }

    public MTLBlendFactor SourceRgbBlendFactor
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBlendFactorDelegate>()(Handle,
                _propSourceRGBBlendFactor.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBlendFactorDelegate>()(Handle,
                _propSourceRGBBlendFactor.Setter, value);
        }
    }

    public static ObjCClass RuntimeClass { get; } = nameof(MTLRenderPipelineColorAttachmentDescriptor);

    static MTLRenderPipelineColorAttachmentDescriptor INativeHandle<MTLRenderPipelineColorAttachmentDescriptor>.
        CreateWithHandle(IntPtr handle) => new(handle);

    private static readonly Property _propPixelFormat = Property.Create("pixelFormat");
    private static readonly Property _propWriteMask = Property.Create("writeMask");
    private static readonly Property _propBlendingEnabled =
        Property.CreateWithGetter("blendingEnabled", "isBlendingEnabled");
    private static readonly Property _propAlphaBlendOperation = Property.Create("alphaBlendOperation");
    private static readonly Property _propRgbBlendOperation = Property.Create("rgbBlendOperation");
    private static readonly Property _propDestinationAlphaBlendFactor = Property.Create("destinationAlphaBlendFactor");
    private static readonly Property _propDestinationRGBBlendFactor = Property.Create("destinationRGBBlendFactor");
    private static readonly Property _propSourceAlphaBlendFactor = Property.Create("sourceAlphaBlendFactor");
    private static readonly Property _propSourceRGBBlendFactor = Property.Create("sourceRGBBlendFactor");
}