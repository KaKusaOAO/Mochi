namespace Mochi.Nbt.Serializations;

public interface ITagVisitor
{
    public void VisitString(NbtString tag);
    public void VisitByte(NbtByte tag);
    public void VisitShort(NbtShort tag);
    public void VisitInt(NbtInt tag);
    public void VisitLong(NbtLong tag);
    public void VisitFloat(NbtFloat tag);
    public void VisitDouble(NbtDouble tag);
    public void VisitByteArray(NbtByteArray tag);
    public void VisitIntArray(NbtIntArray tag);
    public void VisitLongArray(NbtLongArray tag);
    public void VisitList(NbtList tag);
    public void VisitCompound(NbtCompound tag);
    public void VisitEnd(NbtEnd tag);
}