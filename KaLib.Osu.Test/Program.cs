using System;
using System.Threading.Tasks;
using KaLib.Osu.Beatmaps;
using KaLib.Texts;
using KaLib.Utils;

namespace KaLib.Osu.Test
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var map = Beatmap.FromFile(@"/Users/kaka/Downloads/881510 RIOT - Overkill/RIOT - Overkill (Atalyth) [Massacre].osu");
            Logger.Info(TranslateText.Of("Beatmap Version: %s")
                .AddWith(Text.Represent(map.Version)));

            var meta = map.Metadata;
            Logger.Info($"--> {meta.Artist} - {meta.Title} [{meta.Version}]");
            Logger.Info($"    Unicode: {meta.ArtistUnicode} - {meta.TitleUnicode}");
            Logger.Info($"    Creator: {meta.Creator}");
            Logger.Info("--------------------------------------------------------");

            var general = map.General;
            Logger.Info(TranslateText.Of("    %s")
                .AddWith(LiteralText.Of("[General]").SetColor(TextColor.Aqua)));
            Logger.Info(TranslateText.Of("    Epilepsy?: %s")
                .AddWith(Text.Represent(general.EpilepsyWarning)));
            Logger.Info(TranslateText.Of("    Preview Time: %s")
                .AddWith(Text.Represent(general.PreviewTime)));
            Logger.Info("--------------------------------------------------------");
            
            var editor = map.Editor;
            Logger.Info(TranslateText.Of("    %s")
                .AddWith(LiteralText.Of("[Editor]").SetColor(TextColor.Aqua)));
            Logger.Info(TranslateText.Of("    Beat Divisor: %s")
                .AddWith(Text.Represent(editor.BeatDivisor)));
            Logger.Info("--------------------------------------------------------");
            
            var diff = map.Difficulty;
            Logger.Info(TranslateText.Of("    %s")
                .AddWith(LiteralText.Of("[Difficulty]").SetColor(TextColor.Aqua)));
            Logger.Info(LiteralText.FromLegacyText(
                ($"    &fHP: &6{diff.HPDrainRate}&f, CS: &6{diff.CircleSize}&f, OD: &6{diff.OverallDifficulty}&f, AR: &6{diff.ApproachRate}&f")
                    .Replace('&', '\u00a7')
                ));
            Logger.Info("--------------------------------------------------------");

            await Logger.WaitForActiveLogAsync();
            await Logger.FlushAsync();
            await Task.Delay(16);
        }
    }
}