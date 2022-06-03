using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace KaLib.Texts
{
    public abstract class Text : IText
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

        internal virtual string ToAscii()
        {
            string extra = "";
            foreach (Text e in Extra)
            {
                extra += e.ToAscii() + (Color ?? ParentColor).ToAsciiCode();
            }
            return extra + ParentColor.ToAsciiCode();
        }

        public virtual string ToPlainText()
        {
            string extra = "";
            foreach (Text e in Extra)
            {
                extra += e.ToPlainText();
            }
            return extra;
        }

        public static Text RepresentType(Type t, TextColor color = null)
            => TranslateText.Of($"%s.{t.Name}")
                .SetColor(color ?? TextColor.Gold)
                .AddWith(
                    LiteralText.Of(t.Namespace)
                        .SetColor(TextColor.DarkGray)
                );
            
        private static Text RepresentInt(int val, TextColor color = null)
            => LiteralText.Of(val.ToString())
                .SetColor(color ?? TextColor.Gold);

        private static Text RepresentDefGoldWithRedSuffix(string s, string suffix, TextColor color = null)
            => TranslateText.Of($"{s}%s")
                .SetColor(color ?? TextColor.Gold)
                .AddWith(LiteralText.Of(suffix).SetColor(TextColor.Red));

        private static Text RepresentLong(long val, TextColor color = null)
            => RepresentDefGoldWithRedSuffix(val.ToString(), "L", color);

        private static Text RepresentFloat(float val, TextColor color = null)
            => RepresentDefGoldWithRedSuffix(val.ToString(CultureInfo.InvariantCulture), "f", color);

        private static Text RepresentDouble(double val, TextColor color = null)
            => RepresentDefGoldWithRedSuffix(val.ToString(CultureInfo.InvariantCulture), "d", color);

        private static Text RepresentDecimal(decimal val, TextColor color = null)
            => RepresentDefGoldWithRedSuffix(val.ToString(CultureInfo.InvariantCulture), "m", color);

        private static Text RepresentByte(byte val, TextColor color = null)
            => RepresentDefGoldWithRedSuffix(val.ToString(), "b", color);

        private static Text RepresentString(string val, TextColor color = null)
            => TranslateText.Of(@"""%s""")
                .SetColor(color ?? TextColor.Green)
                .AddWith(LiteralText.Of(val));

        private static Text RepresentShort(short val, TextColor color = null)
            => RepresentDefGoldWithRedSuffix(val.ToString(), "s", color);
        
        private static Text RepresentBool(bool val, TextColor color = null)
            => LiteralText.Of(val.ToString())
                .SetColor(color ?? (val ? TextColor.Green : TextColor.Red));
        
        public static Text Represent(object obj, TextColor color = null)
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

        public abstract Text CloneAsBase();
    }

    public abstract class Text<S> : Text, IText<S> where S : Text<S>
    {
        protected abstract S ResolveThis();

        public abstract S Clone();

        public override Text CloneAsBase()
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

        public S AddExtra(params IText[] texts)
        {
            S t = ResolveThis();
            foreach (IText text in texts)
            {
                Extra.Add(text);
                if (text is Text t2)
                {
                    t2.Parent = t;
                }
            }
            return t;
        }

        public S SetColor(TextColor color)
        {
            S t = ResolveThis();
            Color = color;
            return t;
        }

        public S Format(TextFormatFlag flags)
        {
            S t = ResolveThis();
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
