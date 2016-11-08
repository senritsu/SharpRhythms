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

namespace SharpRhythms.Tests.Formats
{
    using System;
    using System.Linq;
    using Enums;
    using Parsers.Msd;
    using Shouldly;
    using Xunit;

    public class SmTests
    {
        [Fact]
        public void BpmChangesAreParsedCorrectly()
        {
            var parser = new SmParser(x => 180);
            var actual = parser.LoadFromFile("files/SM/bpm_changes_and_stops.sm").Tempo.Bpm.ToArray();
            actual.Length.ShouldBe(3);
            actual.Select(x => x.Time).ShouldBe(new [] {1.0, 3.0, 5.0});
            actual.Select(x => x.Bpm).ShouldBe(new [] {2.0, 4.0, 6.0});
        }

        [Fact]
        public void StopsAreParsedCorrectly()
        {
            var parser = new SmParser(x => 180);
            var actual = parser.LoadFromFile("files/SM/bpm_changes_and_stops.sm").Tempo.Interruptions.ToArray();
            actual.Length.ShouldBe(3);
            actual.Select(x => x.Time).ShouldBe(new[] { 1.0, 3.0, 5.0 });
            actual.Select(x => x.Duration).ShouldBe(new[] { 2.0, 4.0, 6.0 });
        }

        public void NotesAreParsedCorrectly()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ChartsAreParsedCorrectly()
        {
            var parser = new SmParser(x => 180);
            var actual = parser.LoadFromFile("files/SM/chart_definitions.sm").Charts;
            actual.Count.ShouldBe(2);
            actual.First().Type.ShouldBe(StepChartType.Single);
            actual.First().DescriptionOrAuthor.ShouldBe("single chart");
            actual.First().DifficultyRating.Difficulty.ShouldBe(Difficulty.Easy);
            actual.First().DifficultyRating.Meter.ShouldBe(1);
            actual.First().DifficultyRating.Radar.Stream.ShouldBe(0.1);
            actual.First().DifficultyRating.Radar.Voltage.ShouldBe(0.2);
            actual.First().DifficultyRating.Radar.Air.ShouldBe(0.3);
            actual.First().DifficultyRating.Radar.Freeze.ShouldBe(0.4);
            actual.First().DifficultyRating.Radar.Chaos.ShouldBe(0.5);
            actual.Last().Type.ShouldBe(StepChartType.Double);
            actual.Last().DescriptionOrAuthor.ShouldBe("double chart");
            actual.Last().DifficultyRating.Difficulty.ShouldBe(Difficulty.Medium);
            actual.Last().DifficultyRating.Meter.ShouldBe(2);
            actual.Last().DifficultyRating.Radar.Stream.ShouldBe(0.5);
            actual.Last().DifficultyRating.Radar.Voltage.ShouldBe(0.4);
            actual.Last().DifficultyRating.Radar.Air.ShouldBe(0.3);
            actual.Last().DifficultyRating.Radar.Freeze.ShouldBe(0.2);
            actual.Last().DifficultyRating.Radar.Chaos.ShouldBe(0.1);
        }
    }
}
