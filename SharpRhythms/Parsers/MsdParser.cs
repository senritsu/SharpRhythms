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

namespace SharpRhythms.Parsers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Abstractions.Timing;
    using Sprache;

    public static class MsdParser
    {
        private static readonly Parser<char> MsdTagStart = Parse.Char('#');
        private static readonly Parser<char> MsdTagSeparator = Parse.Char(':');
        private static readonly Parser<char> MsdTagContentDelimiter = Parse.Char(';');
        private static readonly Parser<char> MsdTagTerminator = Parse.Char(';');
        private static readonly Parser<string> MsdTagName = Parse.Upper.Many().Text();

        private static readonly Parser<string> EndOfInput =
            Parse.WhiteSpace.Many().Then(x => Parse.Return("").End());

        private static readonly Parser<string> EndOfTag =
            MsdTagContentDelimiter.Then(
                x => MsdTagStart.Token().Select(hash => hash.ToString())
                    .Or(EndOfInput));

        private static readonly Parser<string> MsdTagContents =
            Parse.AnyChar.Except(EndOfTag).Many().Text();

        public static Parser<MsdTag> MsdTag =
            from leadingWhitespace in Parse.WhiteSpace.Many().Optional()
            from hash in MsdTagStart
            from name in MsdTagName
            from colon in MsdTagSeparator
            from content in MsdTagContents
            from terminator in MsdTagTerminator
            select new MsdTag
            {
                Name = name,
                Content = content
            };

        public static Parser<IEnumerable<MsdTag>> Parser =
            from tags in MsdTag.Token().Many()
            select tags;
    }
}