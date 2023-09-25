using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public interface ITagTypeInfo
{
    public NbtTag Load(NbtReader reader);
}

public interface ITagTypeInfo<T> : ITagTypeInfo where T : NbtTag
{
    public new T Load(NbtReader reader);
    NbtTag ITagTypeInfo.Load(NbtReader reader) => Load(reader);
}