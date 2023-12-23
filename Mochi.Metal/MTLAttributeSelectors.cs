using Mochi.ObjC;

namespace Mochi.Metal;

internal static class MTLAttributeSelectors
{
    public static Selector Name => CommonSelectors.Name;
    public static Selector AttributeIndex => "attributeIndex";
    public static Selector AttributeType => "attributeType";
    public static Selector IsActive => "isActive";
    public static Selector IsPatchData => "isPatchData";
    public static Selector IsPatchControlPointData => "isPatchControlPointData";
}