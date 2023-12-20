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
                _selGetPixelFormat);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetPixelFormatDelegate>()(Handle,
                _selSetPixelFormat, value);
        }
    }

    public MTLColorWriteMask WriteMask
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetWriteMaskDelegate>()(Handle,
                _selGetWriteMask);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetWriteMaskDelegate>()(Handle,
                _selSetWriteMask, value);
        }
    }

    public bool IsBlendingEnabled
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle,
                _selGetBlendingEnabled);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBoolDelegate>()(Handle,
                _selSetBlendingEnabled, value);
        }
    }

    public MTLBlendOperation AlphaBlendOperation
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBlendOperationDelegate>()(Handle,
                _selGetAlphaBlendOperation);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBlendOperationDelegate>()(Handle,
                _selSetAlphaBlendOperation, value);
        }
    }

    public MTLBlendOperation RgbBlendOperation
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBlendOperationDelegate>()(Handle,
                _selGetRGBBlendOperation);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBlendOperationDelegate>()(Handle,
                _selSetRGBBlendOperation, value);
        }
    }

    public MTLBlendFactor DestinationAlphaBlendFactor
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBlendFactorDelegate>()(Handle,
                _selGetDestinationAlphaBlendFactor);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBlendFactorDelegate>()(Handle,
                _selSetDestinationAlphaBlendFactor, value);
        }
    }

    public MTLBlendFactor DestinationRgbBlendFactor
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBlendFactorDelegate>()(Handle,
                _selGetDestinationRGBBlendFactor);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBlendFactorDelegate>()(Handle,
                _selSetDestinationRGBBlendFactor, value);
        }
    }

    public MTLBlendFactor SourceAlphaBlendFactor
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBlendFactorDelegate>()(Handle,
                _selGetSourceAlphaBlendFactor);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBlendFactorDelegate>()(Handle,
                _selSetSourceAlphaBlendFactor, value);
        }
    }

    public MTLBlendFactor SourceRgbBlendFactor
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBlendFactorDelegate>()(Handle,
                _selGetSourceRGBBlendFactor);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBlendFactorDelegate>()(Handle,
                _selSetSourceRGBBlendFactor, value);
        }
    }

    public static ObjCClass RuntimeClass { get; } = nameof(MTLRenderPipelineColorAttachmentDescriptor);

    static MTLRenderPipelineColorAttachmentDescriptor INativeHandle<MTLRenderPipelineColorAttachmentDescriptor>.
        CreateWithHandle(IntPtr handle) => new(handle);

    private static readonly Selector _selGetPixelFormat = "pixelFormat";
    private static readonly Selector _selSetPixelFormat = "setPixelFormat:";
    private static readonly Selector _selGetWriteMask = "writeMask";
    private static readonly Selector _selSetWriteMask = "setWriteMask:";
    private static readonly Selector _selGetBlendingEnabled = "isBlendingEnabled";
    private static readonly Selector _selSetBlendingEnabled = "setBlendingEnabled:";
    private static readonly Selector _selGetAlphaBlendOperation = "alphaBlendOperation";
    private static readonly Selector _selSetAlphaBlendOperation = "setAlphaBlendOperation:";
    private static readonly Selector _selGetRGBBlendOperation = "rgbBlendOperation";
    private static readonly Selector _selSetRGBBlendOperation = "setRGBBlendOperation:";
    private static readonly Selector _selGetDestinationAlphaBlendFactor = "destinationAlphaBlendFactor";
    private static readonly Selector _selSetDestinationAlphaBlendFactor = "setDestinationAlphaBlendFactor:";
    private static readonly Selector _selGetDestinationRGBBlendFactor = "destinationRGBBlendFactor";
    private static readonly Selector _selSetDestinationRGBBlendFactor = "setDestinationRGBBlendFactor:";
    private static readonly Selector _selGetSourceAlphaBlendFactor = "sourceAlphaBlendFactor";
    private static readonly Selector _selSetSourceAlphaBlendFactor = "setSourceAlphaBlendFactor:";
    private static readonly Selector _selGetSourceRGBBlendFactor = "sourceRGBBlendFactor";
    private static readonly Selector _selSetSourceRGBBlendFactor = "setSourceRGBBlendFactor:";
}