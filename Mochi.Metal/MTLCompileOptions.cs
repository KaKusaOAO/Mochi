using System.Runtime.Versioning;
using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLCompileOptions : IObjCInterface<MTLCompileOptions>
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLCompileOptions>.Handle => Handle;

    public MTLCompileOptions(IntPtr handle) => Handle = handle;

    public static MTLCompileOptions AllocInit() => RuntimeClass.AllocInit<MTLCompileOptions>();

    public bool IsFastMathEnabled
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle,
                _propFastMathEnabled.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBoolDelegate>()(Handle, 
                _propFastMathEnabled.Setter, value);
        }
    }

    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("ios9.0")]
    public MTLLanguageVersion LanguageVersion
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetLanguageVersionDelegate>()(Handle,
                _propLanguageVersion.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetLanguageVersionDelegate>()(Handle,
                _propLanguageVersion.Setter, value);
        }
    }

    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("ios14.0")]
    public MTLLibraryType LibraryType
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetLibraryTypeDelegate>()(Handle,
                _propLibraryType.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetLibraryTypeDelegate>()(Handle, 
                _propLibraryType.Setter, value);
        }
    }

    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("ios14.0")]
    public NSString InstallName
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSStringDelegate>()(Handle,
                _propInstallName.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSStringDelegate>()(Handle, 
                _propInstallName.Setter, value);
        }
    }

    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("ios14.0")]
    public NSArray<MTLDynamicLibrary> Libraries
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSArrayDelegate>()(Handle,
                    _propLibraries.Getter)
                .AsTyped<MTLDynamicLibrary>();
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSArrayDelegate>()(Handle, 
                _propLibraries.Setter, value.AsUntyped());
        }
    }

    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios14.0")]
    public bool PreserveInvariance
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle,
                _propPreserveInvariance.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBoolDelegate>()(Handle, 
                _propPreserveInvariance.Setter, value);
        }
    }

    [SupportedOSPlatform("macos13.0")]
    [SupportedOSPlatform("ios16.0")]
    public MTLLibraryOptimizationLevel OptimizationLevel
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetOptimizationLevelDelegate>()(Handle,
                _propOptimizationLevel.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetOptimizationLevelDelegate>()(Handle,
                _propOptimizationLevel.Setter, value);
        }
    }

    [SupportedOSPlatform("macos13.3")]
    [SupportedOSPlatform("ios16.4")]
    public MTLCompileSymbolVisibility CompileSymbolVisibility
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetCompileSymbolVisibilityDelegate>()(Handle,
                _propCompileSymbolVisibility.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetCompileSymbolVisibilityDelegate>()(Handle,
                _propCompileSymbolVisibility.Setter, value);
        }
    }

    [SupportedOSPlatform("macos13.3")]
    [SupportedOSPlatform("ios16.4")]
    public bool IsReferencingUndefinedSymbolsAllowed
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetBoolDelegate>()(Handle,
                _propAllowReferencingUndefinedSymbols.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBoolDelegate>()(Handle,
                _propAllowReferencingUndefinedSymbols.Setter, value);
        }
    }

    [SupportedOSPlatform("macos13.3")]
    [SupportedOSPlatform("ios16.4")]
    public uint MaxTotalThreadsPerThreadGroup
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSUIntDelegate>()(Handle,
                _propMaxTotalThreadsPerThreadGroup.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSUIntDelegate>()(Handle,
                _propMaxTotalThreadsPerThreadGroup.Setter, value);
        }
    }

    public static ObjCClass RuntimeClass { get; } = nameof(MTLCompileOptions);
    static MTLCompileOptions INativeHandle<MTLCompileOptions>.CreateWithHandle(IntPtr handle) => new(handle);

    private static readonly Property _propFastMathEnabled = Property.Create("fastMathEnabled");
    private static readonly Property _propLanguageVersion = Property.Create("languageVersion");
    private static readonly Property _propLibraryType = Property.Create("libraryType");
    private static readonly Property _propInstallName = Property.Create("installName");
    private static readonly Property _propLibraries = Property.Create("libraries");
    private static readonly Property _propPreserveInvariance = Property.Create("preserveInvariance");
    private static readonly Property _propOptimizationLevel = Property.Create("optimizationLevel");
    private static readonly Property _propCompileSymbolVisibility = Property.Create("compileSymbolVisibility");
    private static readonly Property _propAllowReferencingUndefinedSymbols = Property.Create("allowReferencingUndefinedSymbols");
    private static readonly Property _propMaxTotalThreadsPerThreadGroup = Property.Create("maxTotalThreadsPerThreadgroup");
}