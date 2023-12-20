using System.Collections.Generic;
using System.Linq;

namespace Mochi.Texts;

public interface IMutableComponent : IComponent
{
    public new IStyle Style { get; set; }
    IStyle IComponent.Style => Style;
}

public interface IMutableComponent<T> : IComponent<T>, IMutableComponent where T : IStyle<T>
{
    public new T Style { get; set; }
    IStyle IMutableComponent.Style
    {
        get => Style;
        set => Style = (T) value;
    }

    T IComponent<T>.Style => Style;
    IStyle IComponent.Style => Style;
}