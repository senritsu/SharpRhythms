﻿/***************************************************************************\
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
    using System.Linq;
    using Enums;

    public class StepmaniaChart : IEnumerable<StepmaniaMeasure>, IEnumerable<StepmaniaNote>
    {
        public StepChartType Type { get; set; }
        public string DescriptionOrAuthor { get; set; }
        public StepmaniaDifficultyRating DifficultyRating { get; set; }
        public List<StepmaniaMeasure> Measures { get; set; }
        public IEnumerable<StepmaniaNote> Notes => Measures.SelectMany(x => x.Notes);

        IEnumerator<StepmaniaNote> IEnumerable<StepmaniaNote>.GetEnumerator() => Notes.GetEnumerator();
        IEnumerator<StepmaniaMeasure> IEnumerable<StepmaniaMeasure>.GetEnumerator() => Measures.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Notes.GetEnumerator();
    }
}