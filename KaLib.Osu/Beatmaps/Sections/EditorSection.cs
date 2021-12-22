using System;
using System.Collections.Generic;
using System.Linq;
using KaLib.Utils;

namespace KaLib.Osu.Beatmaps.Sections
{
    public class EditorSection : Section
    {
        
        public EditorSection() : base("Editor")
        {
        }

        static EditorSection()
        {
            RegisterSectionType("Editor", typeof(EditorSection));
        }

        public List<int> Bookmarks { get; set; } = new();
        public decimal DistanceSpacing { get; set; }
        public decimal BeatDivisor { get; set; }
        public int GridSize { get; set; }
        public decimal TimelineZoom { get; set; }

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
                    case nameof(Bookmarks):
                        try
                        {
                            Bookmarks.Clear();
                            Bookmarks.AddRange(value.Split(',').Select(int.Parse));
                        }
                        catch(FormatException)
                        {
                            Logger.Error(@$"Value of ""{nameof(Bookmarks)}"" contains non-integer.");
                        }
                        continue;
                    case nameof(DistanceSpacing):
                        try
                        {
                            DistanceSpacing = decimal.Parse(value);
                        }
                        catch(FormatException)
                        {
                            Logger.Error(@$"Value of ""${nameof(DistanceSpacing)}"" is not a decimal.");
                        }
                        continue;
                    case nameof(BeatDivisor):
                        try
                        {
                            BeatDivisor = decimal.Parse(value);
                        }
                        catch(FormatException)
                        {
                            Logger.Error(@$"Value of ""${nameof(BeatDivisor)}"" is not a decimal.");
                        }
                        continue;
                    case nameof(GridSize):
                        try
                        {
                            GridSize = int.Parse(value);
                        }
                        catch(FormatException)
                        {
                            Logger.Error(@$"Value of ""${nameof(GridSize)}"" is not an integer.");
                        }
                        continue;
                    case nameof(TimelineZoom):
                        try
                        {
                            TimelineZoom = decimal.Parse(value);
                        }
                        catch(FormatException)
                        {
                            Logger.Error(@$"Value of ""${nameof(TimelineZoom)}"" is not a decimal.");
                        }
                        continue;
                }
                Logger.Warn($"Unhandled line in Editor: {line}");
            }
        }
#pragma warning restore
    }
}