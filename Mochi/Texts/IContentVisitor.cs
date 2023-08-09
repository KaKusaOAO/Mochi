using System;

namespace Mochi.Texts;

public interface IContentVisitor
{
    public void Accept(IContent content, IStyle style);

    public static IContentVisitor Create(Action<IContent, IStyle> action) => new Instance(action);

    private class Instance : IContentVisitor
    {
        private Action<IContent, IStyle> _delegate;

        public Instance(Action<IContent, IStyle> action)
        {
            _delegate = action;
        }

        public void Accept(IContent content, IStyle style)
        {
            _delegate(content, style);
        }
    }
}
