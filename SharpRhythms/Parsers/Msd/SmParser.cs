/***************************************************************************\
The MIT License (MIT)

Copyright (c) 2016 senritsu (https://github.com/senritsu)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
\***************************************************************************/

namespace SharpRhythms.Parsers.Msd
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Abstractions.Measure;
    using Abstractions.Metadata;
    using Abstractions.Track;
    using Base;
    using Enums;
    using Extensions;
    using Implementations.Stepmania;
    using Sprache;

    public class SmParser : SpecificMsdParser<StepmaniaTrack>
    {
        public SmParser(AudioLengthAccessor audioLengthAccessor) : base(audioLengthAccessor)
        {
            // track information
            TagActions["TITLE"] = (tag, track) => track.Information.Title.Original = tag.Content;
            TagActions["TITLETRANSLIT"] = (tag, track) => track.Information.Title.Alternative = tag.Content;
            TagActions["SUBTITLE"] = (tag, track) => track.Information.Subtitle.Original = tag.Content;
            TagActions["SUBTITLETRANSLIT"] = (tag, track) => track.Information.Subtitle.Alternative = tag.Content;
            TagActions["ARTIST"] = (tag, track) => track.Information.Artist.Original = tag.Content;
            TagActions["ARTISTTRANSLIT"] = (tag, track) => track.Information.Artist.Alternative = tag.Content;
            TagActions["DISPLAYBPM"] = (tag, track) => track.Information.DisplayBpm = DisplayBpm.Parse(tag.Content);
            TagActions["GENRE"] = (tag, track) => track.Information.Genre = tag.Content;
            TagActions["CREDIT"] = (tag, track) => track.Information.Credit = tag.Content;
            TagActions["SELECTABLE"] = (tag, track) => track.Information.Selectable = tag.AsBoolean();
            // files
            TagActions["MUSIC"] = (tag, track) => track.Files.Music = tag.Content;
            TagActions["CDTITLE"] = (tag, track) => track.Files.CdTitleIcon = tag.Content;
            TagActions["BACKGROUND"] = (tag, track) => track.Files.Background = tag.Content;
            TagActions["BANNER"] = (tag, track) => track.Files.Banner = tag.Content;
            TagActions["LYRICSPATH"] = (tag, track) => track.Files.Lyrics = tag.Content;
            // song preview
            TagActions["SAMPLESTART"] = (tag, track) => track.Preview.SampleStart = tag.AsDouble();
            TagActions["SAMPLELENGTH"] = (tag, track) => track.Preview.SampleLength = tag.AsDouble();
            // offset and tempo
            TagActions["OFFSET"] = (tag, track) => track.Offset = tag.AsDouble();
            TagActions["BPMS"] = (tag, track) => track.Tempo.Bpm = BpmChanges.Parse(tag.Content);
            TagActions["STOPS"] = (tag, track) => track.Tempo.Interruptions = Interruptions.Parse(tag.Content);
            // notes
            TagActions["NOTES"] = (tag, track) => track.Charts.Add(Chart.Parse(tag.Content));
        }

        protected override void PostprocessTrack(StepmaniaTrack track)
        {
            foreach (var chart in track)
            {
                var measures = chart.Measures.Cast<IMeasure<StepmaniaNote>>();
                measures.RecalculateNoteTimes(track.Tempo, SongLength, track.Offset);
            }
        }

        private static Difficulty ConvertDifficulty(string difficulty)
        {
            switch (difficulty.ToLower())
            {
                case "easy":
                    return Difficulty.Easy;
                case "medium":
                    return Difficulty.Medium;
                case "hard":
                    return Difficulty.Hard;
                case "challenge":
                    return Difficulty.Challenge;
                default:
                    throw new ArgumentException($"Unknown difficulty '{difficulty}'");
            }
        }

        private static StepChartType ConvertType(string type)
        {
            switch (type.ToLower())
            {
                case "dance-single":
                    return StepChartType.Single;
                case "dance-double":
                    return StepChartType.Double;
                default:
                    throw new ArgumentException($"Unknown chart type '{type}'");
            }
        }

        private static readonly Dictionary<char, StepType> StepTypeLookup = new Dictionary<char, StepType>
        {
            ['0'] = StepType.None,
            ['1'] = StepType.Normal,
            ['2'] = StepType.HoldStart,
            ['3'] = StepType.Release,
            ['4'] = StepType.RollStart,
            ['M'] = StepType.Mine,
            ['K'] = StepType.Keysound,
            ['L'] = StepType.Lift,
            ['F'] = StepType.Fake
        };

        private static readonly Dictionary<StepChartType, Direction[]> DirectionLookup = new Dictionary
            <StepChartType, Direction[]>
        {
            [StepChartType.Single] = new[] {Direction.Left, Direction.Down, Direction.Up, Direction.Right},
            [StepChartType.Battle] = new[] {Direction.Left, Direction.Down, Direction.Up, Direction.Right},
            [StepChartType.Solo] =
                new[]
                {
                    Direction.Left, Direction.DiagonalUpLeft, Direction.Down, Direction.Up, Direction.DiagonalUpRight,
                    Direction.Right
                },
            [StepChartType.Double] =
                new[]
                {
                    Direction.Left, Direction.Down, Direction.Up, Direction.Right, Direction.Left, Direction.Down,
                    Direction.Up, Direction.Right
                } // TODO add proper support for multiple pads
        };

        private static readonly Parser<StepmaniaRadar> Radar =
            from values in MsdTagContentParser.ListContent(Parse.DecimalInvariant)
            let numbers = values.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray()
            select new StepmaniaRadar
            {
                Stream = numbers[0],
                Voltage = numbers[1],
                Air = numbers[2],
                Freeze = numbers[3],
                Chaos = numbers[4]
            };

        private static StepmaniaRow ParseRow(string notes, StepChartType chartType, int rowIndex, int rowCount)
            => new StepmaniaRow(
                notes.Select(
                    (note, noteIndex) =>
                        new StepmaniaNote(DirectionLookup[chartType][noteIndex], rowCount, (double) rowIndex/rowCount,
                            StepTypeLookup[note])).Where(x => x.Type != StepType.None));

        private static Parser<StepmaniaMeasure> Measure(StepChartType chartType) =>
            from rowStrings in Utilities.ListOf(Parse.LetterOrDigit.Many().Text(), Parse.String("\r\n").Or(Parse.String("\n")))
            let nonEmptyRows = rowStrings.Where(x => !string.IsNullOrEmpty(x)).ToArray()
            let rows = nonEmptyRows.Select((row, rowIndex) => ParseRow(row.Trim(), chartType, rowIndex, nonEmptyRows.Length))
            select new StepmaniaMeasure(rows);

        private static readonly Parser<StepmaniaChart> Chart =
            from type in MsdTagContentParser.ComplexContentPart
            from descriptionOrAuthor in MsdTagContentParser.ComplexContentPart
            from difficulty in MsdTagContentParser.ComplexContentPart
            from meter in MsdTagContentParser.ComplexContentPart
            from radar in MsdTagContentParser.ComplexContentPart
            from measures in MsdTagContentParser.ComplexContentPart
            let chartType = ConvertType(type)

            select new StepmaniaChart
            {
                Type = chartType,
                DescriptionOrAuthor = descriptionOrAuthor,
                DifficultyRating = new StepmaniaDifficultyRating
                {
                    Difficulty = ConvertDifficulty(difficulty),
                    Meter = int.Parse(meter),
                    Radar = Radar.Parse(radar)
                },
                Measures = MsdTagContentParser.ListContent(Measure(chartType)).Parse(measures)
            };

        private static readonly Parser<DisplayBpm> DisplayBpm =
            Parse.Char('*').Select(x => new DisplayBpm {Mode = BpmDisplayMode.Random})
                .Or(Parse.Number.End().Select(x => new DisplayBpm
                {
                    Min = double.Parse(x, CultureInfo.InvariantCulture),
                    Mode = BpmDisplayMode.Static
                }))
                .Or(MsdTagContentParser.ComplexContent.End().Select(x => new DisplayBpm
                {
                    Min = double.Parse(x.First(), CultureInfo.InvariantCulture),
                    Max = double.Parse(x.Last(), CultureInfo.InvariantCulture),
                    Mode = BpmDisplayMode.Static
                }));

        private static readonly Parser<List<BpmChange>> BpmChanges =
            MsdTagContentParser.ListContent(MsdTagContentParser.TimeIndexedValue)
                .Select(values => values.Select(x => new BpmChange
                {
                    Time = x.Time,
                    Bpm = x.Value
                }).ToList());

        private static readonly Parser<List<Interruption>> Interruptions =
            MsdTagContentParser.ListContent(MsdTagContentParser.TimeIndexedValue)
                .Select(values => values.Select(x => new Interruption
                {
                    Time = x.Time,
                    Duration = x.Value
                }).ToList());

        protected override StepmaniaTrack BuildTrack(IEnumerable<MsdTag> tags) => new StepmaniaTrack();
    }
}
