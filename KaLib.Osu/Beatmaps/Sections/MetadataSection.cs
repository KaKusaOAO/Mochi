using System.Collections.Generic;
using System.Linq;
using KaLib.Utils;

namespace KaLib.Osu.Beatmaps.Sections
{
    public class MetadataSection : Section
    {
        
        public MetadataSection() : base("Metadata")
        {
        }

        static MetadataSection()
        {
            RegisterSectionType("Metadata", typeof(MetadataSection));
        }
        
        public string Title { get; set; }
        public string TitleUnicode { get; set; }
        public string Artist { get; set; }
        public string ArtistUnicode { get; set; }
        public string Creator { get; set; }
        
        /// <summary>
        /// The difficulty name for the beatmap.
        /// </summary>
        public string Version { get; set; }
        public string Source { get; set; }
        public List<string> Tags { get; } = new();
        public int BeatmapID { get; set; }
        public int BeatmapSetID { get; set; }
        
#pragma warning disable 0618
        public override void ReadFrom(BeatmapReader reader)
        {
            while (true)
            {
                var line = reader.ReadLine(ignoreEmptyLines: false);
                if (line.Length == 0) return;
                
                var args = line.Split(new[] { ':' }, 2);
                var key = args[0].Trim();
                var value = args[1].Trim();

                switch (key)
                {
                    case nameof(Title):
                        Title = value;
                        continue;
                    case nameof(TitleUnicode):
                        TitleUnicode = value;
                        continue;
                    case nameof(Artist):
                        Artist = value;
                        continue;
                    case nameof(ArtistUnicode):
                        ArtistUnicode = value;
                        continue;
                    case nameof(Creator):
                        Creator = value;
                        continue;
                    case nameof(Version):
                        Version = value;
                        continue;
                    case nameof(Source):
                        Source = value;
                        continue;
                    case nameof(Tags):
                        Tags.Clear();
                        Tags.AddRange(value.Split(' ').Where(s => s.Length > 0));
                        continue;
                    case nameof(BeatmapID):
                        BeatmapID = int.Parse(value);
                        continue;
                    case nameof(BeatmapSetID):
                        BeatmapSetID = int.Parse(value);
                        continue;
                }
                Logger.Warn($"Unhandled line in Metadata: {line}");
            }
        }
#pragma warning restore
    }
}