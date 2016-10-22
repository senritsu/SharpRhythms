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

    public static class MsdFormat
    {
        public static Parser<char> ListSeparator = Parse.Char(',');
        public static Parser<char> MsdTagStart = Parse.Char('#');
        public static Parser<char> MsdTagSeparator = Parse.Char(':');
        public static Parser<char> MsdTagContentDelimiter = Parse.Char(';');
        public static Parser<char> MsdComplexTagContentDelimiter = Parse.Char(':');
        public static Parser<string> MsdTagName = Parse.Upper.Many().Text();

        public static Parser<IEnumerable<T>> ListOf<T, U>(Parser<T> parser, Parser<U> delimiter)
            => from leading in parser
                from rest in delimiter.Token().Then(x => parser).Many()
                select new[] {leading}.Concat(rest);

        public static Parser<string> MsdTagContent =
            from content in Parse.AnyChar.Except(MsdTagContentDelimiter).Many().Text()
            from terminator in MsdTagContentDelimiter.Token()
            select content.Trim();

        public static Parser<string> MsdTagTerminatorFollowingTag =
            MsdTagStart.Token().Select(x => x.ToString());

        public static Parser<string> MsdTagTerminatorFileEnd =
            Parse.LineTerminator.Token().End();

        public static Parser<string> MsdTagTerminator =
            MsdTagTerminatorFileEnd.Or(MsdTagTerminatorFollowingTag);

        public static Parser<MsdTag> MsdTag =
            from hash in MsdTagStart
            from name in MsdTagName
            from colon in MsdTagSeparator
            from content in MsdTagContent.Until(MsdTagTerminator)
            select new MsdTag
            {
                Name = name,
                Content = content
            };

        public static Parser<IEnumerable<MsdTag>> Parser =
            from tags in MsdTag.Many()
            select tags;

        public static Parser<TimeIndexedValue> TimedValue =
            from time in Parse.Decimal
            from equal in Parse.Char('=').Token()
            from value in Parse.Decimal
            select new TimeIndexedValue
            {
                Time = double.Parse(time, CultureInfo.InvariantCulture),
                Value = double.Parse(value, CultureInfo.InvariantCulture)
            };

        public static Parser<bool> YesOrNo = Parse.String("YES").Return(true).XOr(Parse.String("NO").Return(false));
    }
}