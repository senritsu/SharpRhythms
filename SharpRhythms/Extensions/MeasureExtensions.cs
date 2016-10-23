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
    using System.Collections.Generic;
    using Abstractions.Measure;
    using Abstractions.Note;
    using Abstractions.Timing;
    using Abstractions.Track;

    public static class MeasureExtensions
    {
        public static void RecalculateNoteTimes<T>(this IEnumerable<IMeasure<T>> measures,
            Tempo tempo, double offset = 0) where T : ITimeIndexed, INoteValued
        {
            // TODO
            // loop over measures
            // loop over notes
            // calculate cumulative time respecting bpm changes and interruptions
            // calculate time of note respecting note value
        }

        public static void RecalculateNoteTimesWithConstantNoteValues<T>(this IEnumerable<IMeasure<T>> measures,
            Tempo tempo, double offset = 0) where T : ITimeIndexed
        {
            // TODO
            // loop over measures
            // loop over notes
            // calculate cumulative time respecting bpm changes and interruptions
            // calculate time of note assuming constant note value per measure
        }
    }
}
