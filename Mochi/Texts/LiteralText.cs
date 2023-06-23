using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mochi.Texts;

public class LiteralText : Text<LiteralText>
{
    public string Text { get; set; }

    private LiteralText(string? text = null)
    {
        Text = text ?? "";
    }

    protected override LiteralText ResolveThis() => this;

    public static LiteralText Of(string? text) => new(text);

    public static IText FromLegacyText(string message)
    {
        List<LiteralText> texts = new();
        StringBuilder sb = new();
        LiteralText t = new();

        for (var i = 0; i < message.Length; i++)
        {
            var c = message[i];
            if (c == '\u00a7')
            {
                if (++i >= message.Length) break;
                c = message[i];

                // lower case
                if (c >= 'A' && c <= 'Z') c += (char)32;

                TextColor? color;
                if (c == 'x' && i + 12 < message.Length)
                {
                    StringBuilder hex = new("#");
                    for (var j = 0; j < 6; j++)
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
        var result = Of(Text);
        return CloneToTarget(result);
    }

    public override string ToAnsi()
    {
        var extra = base.ToAnsi();
        var color = (Color ?? ParentColor).GetAnsiCode();
        return color + Text + extra;
    }

    public override string ToPlainText()
    {
        var extra = base.ToPlainText();

        var result = "";
        for (var i = 0; i < Text.Length; i++)
        {
            var b = Text;
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