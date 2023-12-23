using Mochi.ObjC;

namespace Mochi.Metal;

internal static class CommonSelectors
{
    public static Selector Label => _propLabel.Getter;
    public static Selector SetLabel => _propLabel.Setter;
    public static Selector Name => "name";
    
    private static readonly Property _propLabel = Property.Create("label");
}