using System;
using System.Collections.Generic;
using System.Linq;
using KaLib.Utils;

namespace KaLib.Osu.Beatmaps.Sections
{
    public class DifficultySection : Section
    {
        
        public DifficultySection() : base("Difficulty")
        {
        }

        static DifficultySection()
        {
            RegisterSectionType("Difficulty", typeof(DifficultySection));
        }

        public decimal HPDrainRate { get; set; }
        public decimal CircleSize { get; set; }
        public decimal OverallDifficulty { get; set; }
        public decimal ApproachRate { get; set; }
        
        /// <summary>
        /// Base slider velocity in hundreds of osu! pixels per beat.
        /// </summary>
        public decimal SliderMultiplier { get; set; }
        
        /// <summary>
        /// Amount of slider ticks per beat.
        /// </summary>
        public decimal SliderTickRate { get; set; }

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
                    case nameof(HPDrainRate):
                        try
                        {
                            HPDrainRate = decimal.Parse(value);
                        }
                        catch(FormatException)
                        {
                            Logger.Error(@$"Value of ""{nameof(HPDrainRate)}"" is not a decimal.");
                        }
                        continue;
                    case nameof(CircleSize):
                        try
                        {
                            CircleSize = decimal.Parse(value);
                        }
                        catch(FormatException)
                        {
                            Logger.Error(@$"Value of ""${nameof(CircleSize)}"" is not a decimal.");
                        }
                        continue;
                    case nameof(OverallDifficulty):
                        try
                        {
                            OverallDifficulty = decimal.Parse(value);
                        }
                        catch(FormatException)
                        {
                            Logger.Error(@$"Value of ""${nameof(OverallDifficulty)}"" is not a decimal.");
                        }
                        continue;
                    case nameof(ApproachRate):
                        try
                        {
                            ApproachRate = decimal.Parse(value);
                        }
                        catch(FormatException)
                        {
                            Logger.Error(@$"Value of ""${nameof(ApproachRate)}"" is not an integer.");
                        }
                        continue;
                    case nameof(SliderMultiplier):
                        try
                        {
                            SliderMultiplier = decimal.Parse(value);
                        }
                        catch(FormatException)
                        {
                            Logger.Error(@$"Value of ""${nameof(SliderMultiplier)}"" is not a decimal.");
                        }
                        continue;
                    case nameof(SliderTickRate):
                        try
                        {
                            SliderTickRate = decimal.Parse(value);
                        }
                        catch(FormatException)
                        {
                            Logger.Error(@$"Value of ""${nameof(SliderTickRate)}"" is not a decimal.");
                        }
                        continue;
                }
                Logger.Warn($"Unhandled line in Difficulty: {line}");
            }
        }
#pragma warning restore
    }
}