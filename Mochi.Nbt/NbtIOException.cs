using System;
using System.IO;

namespace Mochi.Nbt;

// ReSharper disable once InconsistentNaming
public class NbtIOException : IOException
{
    public NbtIOException(string message) : base(message) { }
    public NbtIOException(string message, Exception inner) : base(message, inner) { }
}