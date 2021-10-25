using System;
using System.Threading.Tasks;
using KaLib.Osu.Beatmaps;
using KaLib.Utils;

namespace KaLib.Osu.Test
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var map = Beatmap.FromFile(@"C:\Users\Kaka\AppData\Local\osu!\Songs\1513691 Takanashi Kiara - SPARKS\Takanashi Kiara - SPARKS (Jagdblume) [Ignition].osu");
            Logger.Info($"Beatmap Version: {map.Version}");

            var meta = map.Metadata;
            Logger.Info($"Song: {meta.ArtistUnicode} - {meta.TitleUnicode}");
            Logger.Info($"Song (Romanized): {meta.Artist} - {meta.Title}");

            await Logger.WaitForActiveLogAsync();
            await Logger.FlushAsync();
            await Task.Delay(16);
        }
    }
}