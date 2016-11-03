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

namespace SharpRhythms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions.BeatSpaceMapping;
    using Abstractions.Timing;
    using Abstractions.Track;

    public static class TimeUtilities
    {
        public static double NoteDuration(double noteValue, double bpm)
        {
            return 4*noteValue*60/bpm;
        }

        public static IEnumerable<SongSegment> CalculateSongSegments(Tempo tempo, double songLength)
        {
            var bpmChanges = tempo.Bpm.ToList();
            if (bpmChanges.First().Time > 0)
            {
                bpmChanges[0] = new BpmChange { Time = 0, Bpm = bpmChanges.First().Bpm };
            }

            var segmentBoundaries =
                new Queue<ITimeIndexed>(
                    bpmChanges.Cast<ITimeIndexed>()
                        .Concat(tempo.Interruptions.Cast<ITimeIndexed>())
                        .OrderBy(x => x.Time));

            var timeCursor = 0.0;
            var beatCursor = 0.0;
            var bpm = 0.0;
            var segments = new List<SongSegment>();

            Action<double> commitSegment = end =>
            {
                var songTime = new LinearInterval
                {
                    Start = timeCursor,
                    End = end
                };
                var beatSpace = new LinearInterval
                {
                    Start = beatCursor,
                    End = beatCursor + songTime.Length * bpm / 60
                };
                segments.Add(new SongSegment
                {
                    Bpm = bpm,
                    SongTime = songTime,
                    BeatSpace = beatSpace
                });
            };

            while (segmentBoundaries.Count > 0)
            {
                var current = segmentBoundaries.Dequeue();

                if (current.Time > timeCursor)
                {
                    commitSegment(current.Time);
                    timeCursor = current.Time;
                    beatCursor += segments.Last().BeatSpace.Length;
                }

                var bpmChange = current as BpmChange;
                if (bpmChange != null)
                {
                    bpm = bpmChange.Bpm;
                }

                var interruption = current as Interruption;
                if (interruption != null)
                {
                    timeCursor += interruption.Duration;
                }
            }

            if (timeCursor < songLength)
            {
                commitSegment(songLength);
            }

            return segments;
        }
    }
}
