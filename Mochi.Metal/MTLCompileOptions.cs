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
                _selGetFastMathEnabled);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBoolDelegate>()(Handle, _selSetFastMathEnabled,
                value);
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
                _selGetLanguageVersion);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetLanguageVersionDelegate>()(Handle,
                _selSetLanguageVersion, value);
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
                _selGetLibraryType);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetLibraryTypeDelegate>()(Handle, _selSetLibraryType,
                value);
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
                _selGetInstallName);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSStringDelegate>()(Handle, _selSetInstallName,
                value);
        }
    }

    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("ios14.0")]
    public NSArray<MTLDynamicLibrary> Libraries
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSArrayDelegate>()(Handle, _selGetLibraries)
                .AsTyped<MTLDynamicLibrary>();
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSArrayDelegate>()(Handle, _selSetLibraries, value.AsUntyped());
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
                _selGetPreserveInvariance);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBoolDelegate>()(Handle, _selSetPreserveInvariance,
                value);
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
                _selGetOptimizationLevel);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetOptimizationLevelDelegate>()(Handle,
                _selSetOptimizationLevel, value);
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
                _selGetCompileSymbolVisibility);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetCompileSymbolVisibilityDelegate>()(Handle,
                _selSetCompileSymbolVisibility, value);
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
                _selGetAllowReferencingUndefinedSymbols);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetBoolDelegate>()(Handle,
                _selSetAllowReferencingUndefinedSymbols, value);
        }
    }

    [SupportedOSPlatform("macos13.3")]
    [SupportedOSPlatform("ios16.4")]
    public uint MaxTotalThreadsPerThreadGroup
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetUInt32Delegate>()(Handle,
                _selGetMaxTotalThreadsPerThreadGroup);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetUInt32Delegate>()(Handle,
                _selSetMaxTotalThreadsPerThreadGroup, value);
        }
    }

    public static ObjCClass RuntimeClass { get; } = nameof(MTLCompileOptions);
    static MTLCompileOptions INativeHandle<MTLCompileOptions>.CreateWithHandle(IntPtr handle) => new(handle);

    private static readonly Selector _selGetFastMathEnabled = "fastMathEnabled";
    private static readonly Selector _selSetFastMathEnabled = "setFastMathEnabled:";
    private static readonly Selector _selGetLanguageVersion = "languageVersion";
    private static readonly Selector _selSetLanguageVersion = "setLanguageVersion:";
    private static readonly Selector _selGetLibraryType = "libraryType";
    private static readonly Selector _selSetLibraryType = "setLibraryType:";
    private static readonly Selector _selGetInstallName = "installName";
    private static readonly Selector _selSetInstallName = "setInstallName:";
    private static readonly Selector _selGetLibraries = "libraries";
    private static readonly Selector _selSetLibraries = "setLibraries:";
    private static readonly Selector _selGetPreserveInvariance = "preserveInvariance";
    private static readonly Selector _selSetPreserveInvariance = "setPreserveInvariance:";
    private static readonly Selector _selGetOptimizationLevel = "optimizationLevel";
    private static readonly Selector _selSetOptimizationLevel = "setOptimizationLevel:";
    private static readonly Selector _selGetCompileSymbolVisibility = "compileSymbolVisibility";
    private static readonly Selector _selSetCompileSymbolVisibility = "setCompileSymbolVisibility:";
    private static readonly Selector _selGetAllowReferencingUndefinedSymbols = "allowReferencingUndefinedSymbols";
    private static readonly Selector _selSetAllowReferencingUndefinedSymbols = "setAllowReferencingUndefinedSymbols:";
    private static readonly Selector _selGetMaxTotalThreadsPerThreadGroup = "maxTotalThreadsPerThreadgroup";
    private static readonly Selector _selSetMaxTotalThreadsPerThreadGroup = "setMaxTotalThreadsPerThreadgroup:";
}