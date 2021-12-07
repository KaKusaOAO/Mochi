using System;
using KaLib.Utils;

namespace KaLib.Osu.Beatmaps.Sections
{
    public class GeneralSection : Section
    {
        
        public GeneralSection() : base("General")
        {
        }

        static GeneralSection()
        {
            RegisterSectionType("General", typeof(GeneralSection));
        }
        
        /// <summary>
        /// Location of the audio file relative to the current folder
        /// </summary>
        public string AudioFilename { get; set; }
        
        /// <summary>
        /// Milliseconds of silence before the audio starts playing
        /// </summary>
        public int AudioLeadIn { get; set; }
        
        /// <summary>
        /// Deprecated
        /// </summary>
        [Obsolete("Marked as deprecated in official documentation.")]
        public string AudioHash { get; set; }

        /// <summary>
        /// Time in milliseconds when the audio preview should start
        /// </summary>
        public int PreviewTime { get; set; } = -1;

        /// <summary>
        /// Speed of the countdown before the first hit object
        /// </summary>
        public CountdownType Countdown { get; set; } = CountdownType.Normal;
        
        /// <summary>
        /// Sample set that will be used if timing points do not override it
        /// </summary>
        public SampleSet SampleSet { get; set; }

        /// <summary>
        /// Multiplier for the threshold in time where hit objects placed close together stack (0–1)
        /// </summary>
        public decimal StackLeniency { get; set; } = 0.7m;
        
        /// <summary>
        /// Game mode
        /// </summary>
        public GameMode Mode { get; set; }
        
        public bool LetterboxInBreaks { get; set; }
        
        /// <summary>
        /// Deprecated
        /// </summary>
        [Obsolete("Marked as deprecated in official documentation.")]
        public bool StoryFireInFront { get; set; } = true;
        
        
        public bool UseSkinSprites { get; set; }
        
        /// <summary>
        /// Deprecated
        /// </summary>
        [Obsolete("Marked as deprecated in official documentation.")]
        public bool AlwaysShowPlayfield { get; set; }
        
        public OverlayPosition OverlayPosition { get; set; }
        
        public string SkinPreference { get; set; }

        public bool EpilepsyWarning { get; set; }
        
        public int CountdownOffset { get; set; }
        
        /// <summary>
        /// Whether or not the "N+1" style key layout is used for osu!mania
        /// </summary>
        public bool SpecialStyle { get; set; }

        public bool WidescreenStoryboard { get; set; }

        public bool SamplesMatchPlaybackRate { get; set; }
        
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
                    case nameof(AudioFilename):
                        AudioFilename = value;
                        continue;
                    case nameof(AudioLeadIn):
                        AudioLeadIn = int.Parse(value);
                        continue;
                    case nameof(AudioHash):
                        AudioHash = value;
                        continue;
                    case nameof(PreviewTime):
                        PreviewTime = int.Parse(value);
                        continue;
                    case nameof(Countdown):
                        Countdown = (CountdownType)int.Parse(value);
                        continue;
                    case nameof(SampleSet):
                        SampleSet = EnumUtils.Parse<SampleSet>(value);
                        continue;
                    case nameof(StackLeniency):
                        StackLeniency = decimal.Parse(value);
                        continue;
                    case nameof(Mode):
                        Mode = (GameMode)int.Parse(value);
                        continue;
                    case nameof(LetterboxInBreaks):
                        LetterboxInBreaks = int.Parse(value) == 1;
                        continue;
                    case nameof(StoryFireInFront):
                        StoryFireInFront = int.Parse(value) == 1;
                        continue;
                    case nameof(UseSkinSprites):
                        UseSkinSprites = int.Parse(value) == 1;
                        continue;
                    case nameof(AlwaysShowPlayfield):
                        AlwaysShowPlayfield = int.Parse(value) == 1;
                        continue;
                    case nameof(OverlayPosition):
                        OverlayPosition = EnumUtils.Parse<OverlayPosition>(value);
                        continue;
                    case nameof(SkinPreference):
                        SkinPreference = value;
                        continue;
                    case nameof(EpilepsyWarning):
                        EpilepsyWarning = int.Parse(value) == 1;
                        continue;
                    case nameof(CountdownOffset):
                        CountdownOffset = int.Parse(value);
                        continue;
                    case nameof(SpecialStyle):
                        SpecialStyle = int.Parse(value) == 1;
                        continue;
                    case nameof(WidescreenStoryboard):
                        WidescreenStoryboard = int.Parse(value) == 1;
                        continue;
                    case nameof(SamplesMatchPlaybackRate):
                        SamplesMatchPlaybackRate = int.Parse(value) == 1;
                        continue;
                }
                Logger.Warn($"Unhandled line in General: {line}");
            }
        }
#pragma warning restore
    }
}