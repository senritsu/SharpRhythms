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

namespace SharpRhythms.Implementations.Stepmania
{
    using System.Collections;
    using System.Collections.Generic;
    using Abstractions;
    using Abstractions.Track;
    using Parsers;

    public class StepmaniaTrack : IEnumerable<StepmaniaChart>, IMsdTrack
    {
        public TrackInformation Information { get; set; } = new TrackInformation();
        public SongPreview Preview { get; set; } = new SongPreview();
        public Tempo Tempo { get; set; } = new Tempo();
        /// <summary>
        /// Different interpretation for .dwi files
        /// </summary>
        public double Offset { get; set; }
        public AdditionalFiles Files { get; set; } = new AdditionalFiles();
        public List<StepmaniaChart> Charts { get; set; } = new List<StepmaniaChart>();
        public Dictionary<string, string> AdditionalTags { get; set; } = new Dictionary<string, string>();

        public IEnumerator<StepmaniaChart> GetEnumerator() => Charts.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<MsdTag> UnparsedTags { get; set; }
    }
}