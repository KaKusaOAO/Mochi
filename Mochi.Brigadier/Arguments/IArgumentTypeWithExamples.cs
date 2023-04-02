using System.Collections.Generic;

namespace Mochi.Brigadier.Arguments;

public interface IArgumentTypeWithExamples : IArgumentType
{
    IEnumerable<string> GetExamples();
}