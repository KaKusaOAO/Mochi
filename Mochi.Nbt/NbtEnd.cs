using System;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public class NbtEnd : NbtTag
{
    public static NbtEnd Instance { get; } = new();

    public override TagTypeInfo TypeInfo => TagTypeInfo.End;

    private NbtEnd() {}
    
    public override void WriteContentTo(NbtWriter writer)
    {
        throw new InvalidOperationException(
            $"{nameof(NbtEnd)}.{nameof(WriteContentTo)}() should never be called");
    }
    
    public override void Accept(ITagVisitor visitor) => visitor.VisitEnd(this);
    
    public sealed class EndTypeInfo : TagTypeInfo<NbtEnd>
    {
        // ReSharper disable once MemberHidesStaticFromOuterClass
        internal static EndTypeInfo Instance { get; } = new();
        
        public override TagType Type => TagType.End;

        public override string FriendlyName => "TAG_End";

        private EndTypeInfo() {}

        protected override NbtEnd LoadValue(NbtReader reader) => NbtEnd.Instance;
    }
}