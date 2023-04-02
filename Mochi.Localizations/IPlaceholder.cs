using System.Collections.Generic;

namespace Mochi.Localizations;

public interface IPlaceholder
{
    public void WritePlaceholders(Dictionary<string, object> dictionary);
}