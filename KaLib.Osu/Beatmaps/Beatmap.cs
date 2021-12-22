using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KaLib.Osu.Beatmaps.Sections;
using KaLib.Texts;
using KaLib.Utils;
using KaLib.Utils.Extensions;

namespace KaLib.Osu.Beatmaps
{
    public class Beatmap
    {
        public List<Section> Sections { get; } = new();

        public GeneralSection General => Sections.WhereOfType<Section, GeneralSection>();
        public MetadataSection Metadata => Sections.WhereOfType<Section, MetadataSection>();
        public EditorSection Editor => Sections.WhereOfType<Section, EditorSection>();
        public DifficultySection Difficulty => Sections.WhereOfType<Section, DifficultySection>();

        public int Version { get; private set; }

        public static Beatmap FromFile(string path)
        {
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BeatmapReader(stream);
            var result = new Beatmap
            {
                Version = reader.ReadVersion()
            };

            while (reader.Available > 0)
            {
                var section = reader.ReadSection();
                if (section == null) continue;
                if (result.Sections.Find(x => x.Name == section.Name) != null)
                {
                    continue;
                }
                
                result.Sections.Add(section);
            }

            return result;
        }
    }

    public class BeatmapReader : IDisposable
    {
        private Stream Parent { get; }

        public BeatmapReader(Stream stream)
        {
            Parent = stream;
        }

        public long Available => Parent.Length - Parent.Position;

        /// <summary>
        /// Read the version line at the very beginning of the beatmap file.
        /// </summary>
        /// <returns>The format version of the beatmap file.</returns>
        /// <exception cref="InvalidDataException">Throws if the read line doesn't match the required format.</exception>
        public int ReadVersion()
        {
            string line = ReadLine();
            const string prefix = "osu file format v";
            if (!line.StartsWith(prefix))
            {
                throw new InvalidDataException("Not an osu beatmap.");
            }

            line = line.Substring(prefix.Length);
            int version = int.Parse(line);
            return version;
        }

        /// <summary>
        /// Read a section from the beatmap file.
        /// </summary>
        /// <returns>A <see cref="Section"/>-based instance represents the section read from the file.</returns>
        public Section ReadSection()
        {
            var header = ReadLine();
            header = header.Substring(1, header.Length - 2);
            var section = Section.CreateByName(header);
            if (section != null)
            {
                section.ReadFrom(this);
            }
            else
            {
                Logger.Warn(TranslateText.Of("Ignoring unknown section: %s")
                    .AddWith(Text.Represent(header)));
                while (ReadLine(ignoreEmptyLines: false).Length > 0)
                {
                }
            }
            return section;
        }

        public string ReadLine(bool ignoreComments = true, bool ignoreEmptyLines = true)
        {
            while (true)
            {
                List<byte> arr = new();
                while (true)
                {
                    var read = Parent.ReadByte();
                    if (read == -1) break;

                    var c = (byte)read;
                    if (c == '\n') break;
                    arr.Add(c);
                }

                var result = Encoding.UTF8.GetString(arr.ToArray());
                result = result.Replace("\r", "");
                if (ignoreEmptyLines && result.Length == 0) continue;
                if (ignoreComments && result.StartsWith("//")) continue;
                return result;
            }
        }

        public void Dispose()
        {
            Parent?.Dispose();
        }
    }

    public enum SampleSet
    {
        Normal, Soft, Drum
    }
    
    public enum CountdownType
    {
        None, Normal, Half, Double
    }

    public enum GameMode
    {
        Classic, Taiko, Catch, Mania
    }

    public enum OverlayPosition
    {
        NoChange, Below, Above
    }
}