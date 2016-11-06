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

namespace SharpRhythms.Tests.IntegrationTests
{
    using System.Linq;
    using Enums;
    using Parsers.SM;
    using Shouldly;
    using Xunit;

    public class SimpleTestSong
    {
        [Fact]
        public void LoadsCorrectlyFromSMFormat()
        {
            var track = new SmParser(x => 180).LoadFromFile("files/SM/simple_test_song.sm");

            track.Information.Title.Original.ShouldBe("作品名");
            track.Information.Title.Alternative.ShouldBe("Sakuhinmei");
            track.Information.Subtitle.Original.ShouldBe("傍題");
            track.Information.Subtitle.Alternative.ShouldBe("Boudai");
            track.Information.Artist.Original.ShouldBe("作者");
            track.Information.Artist.Alternative.ShouldBe("Sakusha");
            track.Information.Selectable.ShouldBe(true);
            track.Information.Credit.ShouldBe("senritsu");
            track.Information.DisplayBpm.Min.ShouldBe(180);
            track.Information.DisplayBpm.Mode.ShouldBe(BpmDisplayMode.Static);
            track.Information.Genre.ShouldBe("Test Genre");
            track.Information.New.ShouldBe(false);

            track.Tempo.Bpm.Count().ShouldBe(2);
            track.Tempo.Bpm.First().Time.ShouldBe(0);
            track.Tempo.Bpm.First().Bpm.ShouldBe(120);
            track.Tempo.Bpm.Last().Time.ShouldBe(60);
            track.Tempo.Bpm.Last().Bpm.ShouldBe(180);
            track.Tempo.Interruptions.ShouldHaveSingleItem();
            track.Tempo.Interruptions.Single().Time.ShouldBe(120);
            track.Tempo.Interruptions.Single().Duration.ShouldBe(1);

            track.Files.Background.ShouldBe("bg.png");
            track.Files.Banner.ShouldBe("banner.png");
            track.Files.CdTitleIcon.ShouldBe("icon.png");
            track.Files.Lyrics.ShouldBe("test.lrc");
            track.Files.Music.ShouldBe("test.mp3");
            track.Files.MusicMd5.ShouldBe(null);

            track.Preview.SampleStart.ShouldBe(30);
            track.Preview.SampleLength.ShouldBe(10);

            track.Charts.ShouldHaveSingleItem();

            var chart = track.Charts.Single();

            chart.DescriptionOrAuthor.ShouldBe("description");
            chart.DifficultyRating.Difficulty.ShouldBe(Difficulty.Easy);
            chart.DifficultyRating.Meter.ShouldBe(1);
            chart.DifficultyRating.Radar.Stream.ShouldBe(0.1);
            chart.DifficultyRating.Radar.Voltage.ShouldBe(0.2);
            chart.DifficultyRating.Radar.Air.ShouldBe(0.3);
            chart.DifficultyRating.Radar.Freeze.ShouldBe(0.4);
            chart.DifficultyRating.Radar.Chaos.ShouldBe(0.5);

            chart.Type.ShouldBe(StepChartType.Single);
            chart.Measures.Count().ShouldBe(4);
            chart.Measures.ShouldAllBe(x => x.TimeSignature.BeatValue == 4);
            chart.Measures.ShouldAllBe(x => x.TimeSignature.Beats == 4);
            chart.Measures.ShouldAllBe(x => x.Rows.Length == 4);

            var notes = chart.Notes.ToArray();
            notes.Length.ShouldBe(16);
            notes.ShouldAllBe(x => x.Type == StepType.Normal);
            notes.ShouldAllBe(x => x.Numerator == 1);
            notes.ShouldAllBe(x => x.Denominator == 4);
            notes.Select(x => x.Direction).ShouldBe(new []
            {
                Direction.Left,
                Direction.Down,
                Direction.Up, 
                Direction.Right, 
                Direction.Left, 
                Direction.Left, 
                Direction.Left, 
                Direction.Left, 
                Direction.Down, 
                Direction.Right, 
                Direction.Down, 
                Direction.Right, 
                Direction.Left, 
                Direction.Up, 
                Direction.Left, 
                Direction.Up
            });
            //notes.Select(x => x.Time.ShouldBe(new []
            //{
            //    1.234, 
            //}))
        }
    }
}
