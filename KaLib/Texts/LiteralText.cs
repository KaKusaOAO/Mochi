using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaLib.Texts
{
    public class LiteralText : Text<LiteralText>
    {
        public string Text { get; set; }

        protected override LiteralText ResolveThis() => this;

        public static LiteralText Of(string text)
        {
            return new LiteralText
            {
                Text = text
            };
        }

        public static Text FromLegacyText(string message)
        {
            List<LiteralText> texts = new();
            StringBuilder sb = new();
            LiteralText t = new();

            for (int i = 0; i < message.Length; i++)
            {
                char c = message[i];
                if (c == '\u00a7')
                {
                    if (++i >= message.Length) break;
                    c = message[i];

                    // lower case
                    if (c >= 'A' && c <= 'Z') c += (char)32;

                    TextColor color;
                    if (c == 'x' && i + 12 < message.Length)
                    {
                        StringBuilder hex = new("#");
                        for (int j = 0; j < 6; j++)
                        {
                            hex.Append(message[i + 2 + j * 2]);
                        }

                        try
                        {
                            color = TextColor.Of(hex.ToString());
                        }
                        catch (ArgumentException)
                        {
                            color = null;
                        }
                    }
                    else
                    {
                        color = TextColor.Of(c);
                    }
                    if (color == null) continue;
                    
                    // push old text to the list
                    if (sb.Length > 0)
                    {
                        var old = t;
                        t = old.Clone();
                        old.Text = sb.ToString();
                        sb.Clear();
                        texts.Add(old);
                    }

                    t = new LiteralText
                    {
                        Color = color
                    };
                    continue;
                }

                sb.Append(c);
            }

            t.Text = sb.ToString();
            texts.Add(t);

            var result = new LiteralText();
            foreach (var text in texts)
            {
                result.AddExtra(text);
            }

            return result;
        }

        public override LiteralText Clone()
        {
            LiteralText result = Of(Text);
            result.AddExtra(result.Extra.ToArray());
            return result;
        }

        internal override string ToAscii()
        {
            string extra = base.ToAscii();
            string color = (Color ?? ParentColor).ToAsciiCode();
            return color + Text + extra;
        }

        public override string ToPlainText()
        {
            string extra = base.ToAscii();

            string result = "";
            for (int i = 0; i < Text.Length; i++)
            {
                string b = Text;
                if (b[i] == TextColor.ColorChar && TextColor.McCodes().ToList().IndexOf(b[i + 1]) > -1)
                {
                    i += 2;
                }
                else
                {
                    result += b[i];
                }
            }

            return result + extra;
        }
    }
}