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

namespace SharpRhythms.Abstractions.BeatSpaceMapping
{
    using System;

    public class LinearInterval
    {
        public static readonly LinearInterval Normalized = new LinearInterval
        {
            Start = 0,
            End = 1
        };

        public double Start { get; set; }
        public double End { get; set; }
        public double Length => End - Start;

        public double MapTo(LinearInterval targetInterval, double position, bool clamp = false)
        {
            if (position < Start || position > End)
            {
                if (clamp)
                {
                    position = Math.Max(Start, Math.Min(End, position));
                }
                else
                {
                    throw new ArgumentException("Input position is outside of interval",
                        nameof(position));
                }
            }
            var relative = position - Start;
            var normalized = relative / Length;
            return targetInterval.Start + normalized * targetInterval.Length;
        }

        public override string ToString() => $"[{Start:F3}, {End:F3}]={Length:F3}";
    }
}
