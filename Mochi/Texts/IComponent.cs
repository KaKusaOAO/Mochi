using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Mochi.Texts;

public interface IComponent
{
    public IContent Content { get; }
    public IStyle Style { get; }
    public IList<IComponent> Siblings { get; }
    public IMutableComponent Clone();
    public void Visit(IContentVisitor visitor, IStyle style);
    public void VisitLiteral(IContentVisitor visitor, IStyle style);
}

internal class DowncastList<T, TSuper> : IList<TSuper> where T : TSuper
{
    private readonly IList<T> _list;

    public DowncastList(IList<T> list)
    {
        _list = list;
    }

    public IEnumerator<TSuper> GetEnumerator() => _list.Cast<TSuper>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(TSuper item) => _list.Add((T) item!);

    public void Clear() => _list.Clear();

    public bool Contains(TSuper item) => _list.Contains((T) item!);

    public void CopyTo(TSuper[] array, int arrayIndex) => _list.Cast<TSuper>().ToList().CopyTo(array, arrayIndex);

    public bool Remove(TSuper item) => _list.Remove((T) item!);

    public int Count => _list.Count;
    public bool IsReadOnly => _list.IsReadOnly;
    public int IndexOf(TSuper item) => _list.IndexOf((T) item!);

    public void Insert(int index, TSuper item) => _list.Insert(index, (T) item!);

    public void RemoveAt(int index) => _list.RemoveAt(index);

    public TSuper this[int index]
    {
        get => _list[index];
        set => _list[index] = (T)value!;
    }
}

public interface IComponent<T> : IComponent where T : IStyle<T>
{
    public new IList<IComponent<T>> Siblings { get; }

    public new IMutableComponent<T> Clone();
    IMutableComponent IComponent.Clone() => Clone();
    
    public new T Style { get; }
    IStyle IComponent.Style => Style;
    
    public void Visit(IContentVisitor visitor, T style);
    void IComponent.Visit(IContentVisitor visitor, IStyle style) => Visit(visitor, (T) style);
    
    public void VisitLiteral(IContentVisitor visitor, T style);
    void IComponent.VisitLiteral(IContentVisitor visitor, IStyle style) => VisitLiteral(visitor, (T) style);
}