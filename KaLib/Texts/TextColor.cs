using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using KaLib.Structs;

namespace KaLib.Texts
{
    public class TextColor
    {
        public const char ColorChar = '\u00a7';

        private string _name;
        private int _ordinal;
        private string _toString;
        private Color _color;

        private static readonly Dictionary<char, TextColor> _byChar = new();
        private static readonly Dictionary<string, TextColor> _byName = new();

        private static int _count;

        public static readonly TextColor Black      = new('0', "black",       new Color(0));
        public static readonly TextColor DarkBlue   = new('1', "dark_blue",   new Color(0xaa));
        public static readonly TextColor DarkGreen  = new('2', "dark_green",  new Color(0xaa00));
        public static readonly TextColor DarkAqua   = new('3', "dark_aqua",   new Color(0xaaaa));
        public static readonly TextColor DarkRed    = new('4', "dark_red",    new Color(0xaa0000));
        public static readonly TextColor DarkPurple = new('5', "dark_purple", new Color(0xaa00aa));
        public static readonly TextColor Gold       = new('6', "gold",        new Color(0xffaa00));
        public static readonly TextColor Gray       = new('7', "gray",        new Color(0xaaaaaa));
        public static readonly TextColor DarkGray   = new('8', "dark_gray",   new Color(0x555555));
        public static readonly TextColor Blue       = new('9', "blue",        new Color(0x5555ff));
        public static readonly TextColor Green      = new('a', "green",       new Color(0x55ff55));
        public static readonly TextColor Aqua       = new('b', "aqua",        new Color(0x55ffff));
        public static readonly TextColor Red        = new('c', "red",         new Color(0xff5555));
        public static readonly TextColor Purple     = new('d', "purple",      new Color(0xff55ff));
        public static readonly TextColor Yellow     = new('e', "yellow",      new Color(0xffff55));
        public static readonly TextColor White      = new('f', "white",       new Color(0xffffff));

        private TextColor(char code, string name, Color color)
        {
            this._name = name;
            _toString = ColorChar + "" + code;
            _ordinal = _count++;
            this._color = color;

            _byChar.Add(code, this);
            _byName.Add(name, this);
        }

        private TextColor(string name, string toString, int rgb)
        {
            this._name = name;
            this._toString = toString;
            _ordinal = -1;
            _color = new Color(rgb);
        }

        public static TextColor Of(Color color)
        {
            return Of("#" + $"{color.RGB:x6}");
        }

        public static TextColor Of(string name)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name), "name cannot be null");

            if(name.StartsWith("#") && name.Length == 7)
            {
                int rgb;
                try
                {
                    rgb = int.Parse(name.Substring(1), NumberStyles.HexNumber);
                } catch(FormatException)
                {
                    throw new ArgumentException("Illegal hex string " + name);
                }

                var magic = ColorChar + "x";
                magic = name.Substring(1).Aggregate(magic, (current, c) => current + (ColorChar + "" + c));

                return new TextColor(name, magic, rgb);
            }

            if(_byName.TryGetValue(name, out var defined))
            {
                return defined;
            }

            throw new ArgumentException("Could not parse TextColor " + name);
        }

        public static TextColor Of(char code)
            => _byChar.ContainsKey(code) ? _byChar[code] : null;

        public override string ToString() => _toString;

        public string Name => _name;

        public Color Color => _color;

        private static TextColor[] _predefined = {
            Black,
            DarkBlue,
            DarkGreen,
            DarkAqua,
            DarkRed,
            DarkPurple,
            Gold,
            Gray,
            DarkGray,
            Blue,
            Green,
            Aqua,
            Red,
            Purple,
            Yellow,
            White
        };

        public TextColor ToNearestPredefinedColor()
        {
            var c = _toString[1];
            if (c != 'x')
            {
                return this;
            }

            TextColor closest = null;
            var cl = Color;

            var smallestDiff = 0;
            foreach (var tc in _predefined)
            {
                var rAverage = (tc.Color.R + cl.R) / 2;
                var rDiff = tc.Color.R - cl.R;
                var gDiff = tc.Color.G - cl.G;
                var bDiff = tc.Color.B - cl.B;

                var diff = ((2 + (rAverage >> 8)) * rDiff * rDiff)
                           + (4 * gDiff * gDiff)
                           + ((2 + ((255 - rAverage) >> 8)) * bDiff * bDiff);

                if (closest == null || diff < smallestDiff)
                {
                    closest = tc;
                    smallestDiff = diff;
                }
            }

            return closest;
        }

        public static char[] McCodes()
        {
            return "0123456789abcdef".ToCharArray();
        }
        
        public string ToAsciiCode() => AsciiColor.FromTextColor(this).ToAsciiCode();
    }

    internal static class TextColorEx
    {
        public static string GetAsciiCode(this TextColor color) => color?.ToAsciiCode() ?? AsciiColor.Reset.ToAsciiCode();
    }
}
