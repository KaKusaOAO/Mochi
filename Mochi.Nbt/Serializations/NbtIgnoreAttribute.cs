using System;

namespace Mochi.Nbt.Serializations;

[AttributeUsage(AttributeTargets.Property)]
public class NbtIgnoreAttribute : Attribute
{
}