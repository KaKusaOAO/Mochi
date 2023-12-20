using Mochi.Texts;

namespace Mochi.Brigadier.Bridge;

public class ComponentMessage : IBrigadierMessage
{
    private IComponent _component;

    public ComponentMessage(IComponent component)
    {
        _component = component;
    }

    public string GetString() => _component.ToPlainText();
}