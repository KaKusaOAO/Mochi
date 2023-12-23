namespace Mochi.ObjC;

public readonly struct Property
{
    public string Name { get; }
    private string GetterName { get; }
    private string SetterName { get; }

    public Selector Getter => GetterName;
    public Selector Setter => SetterName;

    private Property(string name, string getter, string setter)
    {
        Name = name;
        GetterName = getter;
        SetterName = setter;
    }

    public static Property Create(string name) => new(name, name, GenerateSetterName(name));

    public static Property CreateWithGetter(string name, string getter) => new(name, getter, GenerateSetterName(name));
    
    public static Property CreateWithSetter(string name, string setter) => new(name, name, setter + ":");

    public static Property Create(string name, string getter, string setter) => new(name, getter, setter + ":");

    private static string GenerateSetterName(string name)
    {
        return new string(
            "set".Concat(
                name.Take(1)
                    .Select(char.ToUpper)
                    .Concat(name.Skip(1))
                    .Append(':')
            ).ToArray());
    }
    
    static Property()
    {
        if (GenerateSetterName("someProperty") != "setSomeProperty:")
            throw new Exception("The implementation is incorrect!");
    }
}