using Mochi.ObjC;

namespace Mochi.Metal;

public struct CAMetalDrawable : IObjCInterface<CAMetalDrawable>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<CAMetalDrawable>.Handle => Handle;

    public CAMetalDrawable(IntPtr handle)
    {
        Handle = handle;
    }

    static CAMetalDrawable INativeHandle<CAMetalDrawable>.CreateWithHandle(IntPtr handle) => new(handle);

    public MTLTexture Texture
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<GetTextureDelegate>()(Handle, _selTexture);
        }
    }

    public static ObjCClass RuntimeClass { get; } = nameof(CAMetalDrawable);

    private delegate MTLTexture GetTextureDelegate(IntPtr handle, Selector sel);
    private static readonly Selector _selTexture = "texture";
}