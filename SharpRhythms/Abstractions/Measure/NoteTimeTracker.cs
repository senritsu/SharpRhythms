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

namespace SharpRhythms.Abstractions.Measure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using BeatSpaceMapping;

    public class NoteTimeTracker
    {
        private readonly SongSegment[] _originalSegments;
        private double _beatCursor;
        private Queue<SongSegment> _segments;
        private SongSegment _currentSegment;
        private LinearInterval _measureBeatSpace;
        private double _lastBeatSpacePosition;

        public NoteTimeTracker(IEnumerable<SongSegment> songSegments)
        {
            _originalSegments = songSegments as SongSegment[] ?? songSegments.ToArray();
            Reset();
        }

        public void NewMeasure(double beatSpaceLength)
        {
            if (_measureBeatSpace != null)
            {
                _beatCursor = _measureBeatSpace.End;
            }

            _measureBeatSpace = new LinearInterval
            {
                Start = _beatCursor,
                End = _beatCursor + beatSpaceLength
            };
        }

        public double CalculateNoteTime(double beatSpacePosition)
        {
            SongSegment segment;
            if (beatSpacePosition < _lastBeatSpacePosition)
            {
                segment = _originalSegments.First(x => x.BeatSpace.End > beatSpacePosition);
                Debug.WriteLine(
                    "Calculating note times out of order is less efficient, consider sorting your notes beforehand");
            }
            else
            {
                _lastBeatSpacePosition = beatSpacePosition;

                while (_currentSegment.BeatSpace.End < beatSpacePosition)
                {
                    _currentSegment = _segments.Dequeue();
                }

                segment = _currentSegment;
            }

            return segment.BeatSpace.MapTo(segment.SongTime, beatSpacePosition);
        }

        public double CalculateNoteTimeFromMeasureTime(double normalizedMeasureTime)
        {
            var beatSpacePosition = LinearInterval.Normalized.MapTo(_measureBeatSpace, normalizedMeasureTime);

            return CalculateNoteTime(beatSpacePosition);
        }

        public void Reset()
        {
            _beatCursor = 0.0;
            _lastBeatSpacePosition = 0.0;
            _segments = new Queue<SongSegment>(_originalSegments);
            _currentSegment = _segments.Dequeue();
        }
    }
}
