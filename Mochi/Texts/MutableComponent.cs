﻿using System.Collections.Generic;
using System.Linq;

namespace Mochi.Texts;

public class MutableComponent<T> : IMutableComponent<T> where T : IStyle<T>
{
    public IContent Content { get; }
    public IList<IComponent<T>> Siblings { get; } = new List<IComponent<T>>();
    private readonly DowncastList<IComponent<T>, IComponent> _downcastList;
    IList<IComponent> IComponent.Siblings => _downcastList;
    
    public T Style { get; set; }
    
    public MutableComponent(IContent content, T style)
    {
        Content = content;
        Style = style;
        _downcastList = new DowncastList<IComponent<T>, IComponent>(Siblings);
    }
    
    public virtual MutableComponent<T> Clone()
    {
        var result = new MutableComponent<T>(Content, Style);
        foreach (var clone in Siblings.Select(x => x.Clone()))
        {
            result.Siblings.Add(clone);
        }
        
        return result;
    }
    IMutableComponent<T> IComponent<T>.Clone() => Clone();

    public void Visit(IContentVisitor visitor, T style)
    {
        style = Style.ApplyTo(style);
        Content.Visit(visitor, style);

        foreach (var sibling in Siblings)
        {
            sibling.Visit(visitor, style);
        }
    }
    
    public void VisitLiteral(IContentVisitor visitor, T style)
    {
        style = Style.ApplyTo(style);
        Content.VisitLiteral(visitor, style);

        foreach (var sibling in Siblings)
        {
            sibling.VisitLiteral(visitor, style);
        }
    }
}

public class MutableComponent : MutableComponent<Style>
{
    public MutableComponent(IContent content) : base(content, Style.Empty)
    {
    }

    public new MutableComponent Clone() => (MutableComponent) base.Clone();
}

public class GenericMutableComponent : IMutableComponent
{
    public IContent Content { get; }
    public IStyle Style { get; set; }
    public IList<IComponent> Siblings { get; } = new List<IComponent>();
    
    public GenericMutableComponent(IContent content, IStyle style)
    {
        Content = content;
        Style = style;
    }
    
    public IMutableComponent Clone()
    {
        var result = new GenericMutableComponent(Content, Style);
        foreach (var clone in Siblings.Select(x => x.Clone()))
        {
            result.Siblings.Add(clone);
        }
        return result;
    }

    public void Visit(IContentVisitor visitor, IStyle style)
    {
        style = Style.ApplyTo(style);
        Content.Visit(visitor, style);

        foreach (var sibling in Siblings)
        {
            sibling.Visit(visitor, style);
        }
    }
    
    public void VisitLiteral(IContentVisitor visitor, IStyle style)
    {
        style = Style.ApplyTo(style);
        Content.VisitLiteral(visitor, style);

        foreach (var sibling in Siblings)
        {
            sibling.VisitLiteral(visitor, style);
        }
    }
}