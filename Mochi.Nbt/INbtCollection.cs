using System.Collections;
using System.Collections.Generic;

namespace Mochi.Nbt;

public interface INbtCollection<T> : IList<T> where T : NbtTag
{
    public TagTypeInfo ElementTypeInfo { get; }
    
    public bool SetTag(int index, NbtTag tag);
    public bool AddTag(NbtTag tag);
    public bool InsertTag(int index, NbtTag tag);
}