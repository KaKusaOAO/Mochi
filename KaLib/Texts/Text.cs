using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace KaLib.Texts
{
    public static class Text
    {
        public static IText RepresentType(Type t, TextColor color = null)
            => TranslateText.Of($"%s.{t.Name}")
                .SetColor(color ?? TextColor.Gold)
                .AddWith(
                    LiteralText.Of(t.Namespace)
                        .SetColor(TextColor.DarkGray)
                );
            
        private static IText RepresentInt(int val, TextColor color = null)
            => LiteralText.Of(val.ToString())
                .SetColor(color ?? TextColor.Gold);

        private static IText RepresentDefGoldWithRedSuffix(string s, string suffix, TextColor color = null)
            => TranslateText.Of($"{s}%s")
                .SetColor(color ?? TextColor.Gold)
                .AddWith(LiteralText.Of(suffix).SetColor(TextColor.Red));

        private static IText RepresentLong(long val, TextColor color = null)
            => RepresentDefGoldWithRedSuffix(val.ToString(), "L", color);

        private static IText RepresentFloat(float val, TextColor color = null)
            => RepresentDefGoldWithRedSuffix(val.ToString(CultureInfo.InvariantCulture), "f", color);

        private static IText RepresentDouble(double val, TextColor color = null)
            => RepresentDefGoldWithRedSuffix(val.ToString(CultureInfo.InvariantCulture), "d", color);

        private static IText RepresentDecimal(decimal val, TextColor color = null)
            => RepresentDefGoldWithRedSuffix(val.ToString(CultureInfo.InvariantCulture), "m", color);

        private static IText RepresentByte(byte val, TextColor color = null)
            => RepresentDefGoldWithRedSuffix(val.ToString(), "b", color);

        private static IText RepresentString(string val, TextColor color = null)
            => TranslateText.Of(@"""%s""")
                .SetColor(color ?? TextColor.Green)
                .AddWith(LiteralText.Of(val));

        private static IText RepresentShort(short val, TextColor color = null)
            => RepresentDefGoldWithRedSuffix(val.ToString(), "s", color);
        
        private static IText RepresentBool(bool val, TextColor color = null)
            => LiteralText.Of(val.ToString())
                .SetColor(color ?? (val ? TextColor.Green : TextColor.Red));
        
        public static IText Represent(object obj, TextColor color = null)
        {
            switch (obj)
            {
                case null:
                    return LiteralText.Of("null").SetColor(TextColor.Red);
                case int i:
                    return RepresentInt(i, color);
                case long l:
                    return RepresentLong(l, color);
                case float f:
                    return RepresentFloat(f, color);
                case double d:
                    return RepresentDouble(d, color);
                case decimal d:
                    return RepresentDecimal(d, color);
                case byte b:
                    return RepresentByte(b, color);
                case bool b:
                    return RepresentBool(b, color);
                case string s:
                    return RepresentString(s, color);
                case short s:
                    return RepresentShort(s, color);
                case Type t:
                    return RepresentType(t, color);
            }
            return TranslateText.Of("(%s)")
                .SetColor(color)
                .AddWith(RepresentType(obj.GetType()));
        }
    }

    public abstract class Text<T> : IText<T>, IMutableText where T : Text<T>
    {
        public ICollection<IText> Extra { get; set; } = new List<IText>();
        public IText Parent { get; set; }
        public TextColor Color { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Obfuscated { get; set; }
        public bool Underline { get; set; }
        public bool Strikethrough { get; set; }
        public bool Reset { get; set; }

        public bool ShouldSerializeExtra() => Extra.Count > 0;
        public TextColor ParentColor
        {
            get
            {
                if (Parent == null) return Environment.OSVersion.Platform == PlatformID.Win32NT ? TextColor.Gray : TextColor.White;
                return Parent.Color ?? Parent.ParentColor;
            }
        }

        public virtual string ToAscii()
        {
            string extra = "";
            foreach (IText e in Extra)
            {
                extra += e.ToAscii() + (Color ?? ParentColor).ToAsciiCode();
            }
            return extra + ParentColor.ToAsciiCode();
        }

        public virtual string ToPlainText()
        {
            string extra = "";
            foreach (IText e in Extra)
            {
                extra += e.ToPlainText();
            }
            return extra;
        }
        
        protected abstract T ResolveThis();

        public abstract T Clone();
        public IMutableText MutableCopy() => Clone();

        IText IText.Clone()
        {
            var clone = Clone();
            foreach (var extra in Extra) clone.Extra.Add(extra);
            clone.Color = Color;
            clone.Bold = Bold; 
            clone.Italic = Italic; 
            clone.Obfuscated = Obfuscated; 
            clone.Strikethrough = Strikethrough; 
            clone.Underline = Underline; 
            clone.Reset = Reset; 
            return clone;
        }

        public T AddExtra(params IText[] texts)
        {
            T t = ResolveThis();
            foreach (IText text in texts)
            {
                Extra.Add(text);
                text.Parent = this;
            }
            return t;
        }

        public T SetColor(TextColor color)
        {
            T t = ResolveThis();
            Color = color;
            return t;
        }

        public T Format(TextFormatFlag flags)
        {
            T t = ResolveThis();
            Bold = flags.HasFlag(TextFormatFlag.Bold);
            Italic = flags.HasFlag(TextFormatFlag.Italic);
            Obfuscated = flags.HasFlag(TextFormatFlag.Obfuscated);
            Strikethrough = flags.HasFlag(TextFormatFlag.Strikethrough);
            Underline = flags.HasFlag(TextFormatFlag.Underline);
            Reset = flags.HasFlag(TextFormatFlag.Reset);
            return t;
        }
    }
}
