using System;
using System.Runtime.InteropServices.JavaScript;

namespace Mochi.JS;

public static partial class Interop
{
    [JSImport("getGlobal", "utils.js")]
    [return: JSMarshalAs<JSType.Any>]
    public static partial object GetWindow();

    [JSImport("get", "utils.js")]
    [return: JSMarshalAs<JSType.Any>]
    public static partial object Get(
        [JSMarshalAs<JSType.Any>] object obj,
        string name);

    [JSImport("set", "utils.js")]
    [return: JSMarshalAs<JSType.Any>]
    public static partial object Set(
        [JSMarshalAs<JSType.Any>] object obj, 
        string name, 
        [JSMarshalAs<JSType.Any>] object val);

    [JSImport("invoke", "utils.js")]
    [return: JSMarshalAs<JSType.Any>]
    public static partial object Invoke(
        [JSMarshalAs<JSType.Any>] object func, 
        [JSMarshalAs<JSType.Array<JSType.Any>>] object[] args);
        
    [JSImport("getInvoke", "utils.js")]
    [return: JSMarshalAs<JSType.Any>]
    public static partial object GetInvoke(
        [JSMarshalAs<JSType.Any>] object obj, 
        string name, 
        [JSMarshalAs<JSType.Array<JSType.Any>>] object[] val);
    
    [JSImport("getNew", "utils.js")]
    [return: JSMarshalAs<JSType.Any>]
    public static partial object GetNew(
        [JSMarshalAs<JSType.Any>] object obj, 
        string name, 
        [JSMarshalAs<JSType.Array<JSType.Any>>] object[] val);

    [JSImport("convertToAny", "utils.js")]
    [return: JSMarshalAs<JSType.Any>]
    // We need a trick here to pass functions as objects to prevent checks by the runtime.
    public static partial object ConvertToAny(
        [JSMarshalAs<JSType.Function>] Action action);
        
    [JSImport("pass", "utils.js")]
    [return: JSMarshalAs<JSType.Number>]
    // The pass() function returns the same value as it receives.
    // It is used to convert values to the desired type.
    public static partial int AsInt32(
        [JSMarshalAs<JSType.Any>] object obj);
    
    [JSImport("pass", "utils.js")]
    [return: JSMarshalAs<JSType.Number>]
    public static partial double AsDouble(
        [JSMarshalAs<JSType.Any>] object obj);
    
    [JSImport("pass", "utils.js")]
    [return: JSMarshalAs<JSType.Number>]
    public static partial float AsSingle(
        [JSMarshalAs<JSType.Any>] object obj);

    [JSImport("pass", "utils.js")]
    [return: JSMarshalAs<JSType.String>]
    public static partial string AsString(
        [JSMarshalAs<JSType.Any>] object obj);
    
    [JSImport("pass", "utils.js")]
    [return: JSMarshalAs<JSType.Boolean>]
    public static partial bool AsBool(
        [JSMarshalAs<JSType.Any>] object obj);
}