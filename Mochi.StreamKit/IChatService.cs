﻿namespace Mochi.StreamKit;

public interface IChatService
{
    public ChatServiceType Type { get; }
    public event Action<IGenericComment> Commented;
}