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

namespace SharpRhythms.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions.BeatSpaceMapping;
    using Abstractions.Measure;
    using Abstractions.Note;
    using Abstractions.Timing;
    using Abstractions.Track;

    public static class MeasureExtensions
    {
        public static double BeatSpaceLength<T>(this IMeasure<T> measure)
        {
            return 4.0*measure.TimeSignature.Beats/measure.TimeSignature.BeatValue;
        }

        public static void RecalculateNoteTimes<T>(this IEnumerable<IMeasure<T>> measures,
            Tempo tempo, double songLength, double offset = 0) where T : class, ITimeIndexed, INoteValued, IMeasureTimed
        {
            var songSegments = TimeUtilities.CalculateSongSegments(tempo, songLength, offset);
            var tracker = new NoteTimeTracker(songSegments);

            foreach (var measure in measures)
            {
                tracker.NewMeasure(measure.BeatSpaceLength());

                foreach (var note in measure.Notes)
                {
                    note.Time = tracker.NextNoteTime(note.NormalizedMeasureTime);
                }
            }
        }
    }
}
