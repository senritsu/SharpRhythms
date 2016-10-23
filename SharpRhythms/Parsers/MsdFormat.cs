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
        public static Parser<char> ListSeparator = Parse.Char(',').Token();
        public static Parser<char> MsdTagStart = Parse.Char('#');
        public static Parser<char> MsdTagSeparator = Parse.Char(':');
        public static Parser<char> MsdTagContentDelimiter = Parse.Char(';');
        public static Parser<char> MsdTagTerminator = Parse.Char(';');
        public static Parser<char> MsdComplexTagContentDelimiter = Parse.Char(':');
        public static Parser<string> MsdTagName = Parse.Upper.Many().Text();

        public static Parser<IEnumerable<TItem>> ListOf<TItem, TDelimiter>(Parser<TItem> parser, Parser<TDelimiter> delimiter)
            => from leading in parser
                from rest in delimiter.Then(x => parser).Many()
                select new[] {leading}.Concat(rest);

        public static Parser<string> EndOfInput =
            Parse.WhiteSpace.Many().Then(x => Parse.Return("").End());

        public static Parser<string> EndOfTag =
            MsdTagContentDelimiter.Then(
                x => MsdTagStart.Token().Select(hash => hash.ToString())
                    .Or(EndOfInput));

        public static Parser<string> MsdTagContent =
            Parse.AnyChar.Except(EndOfTag).Many().Text();

        public static Parser<MsdTag> MsdTag =
            from leadingWhitespace in Parse.WhiteSpace.Many().Optional()
            from hash in MsdTagStart
            from name in MsdTagName
            from colon in MsdTagSeparator
            from content in MsdTagContent
            from terminator in MsdTagTerminator
            select new MsdTag
            {
                Name = name,
                Contents = ListOf(Parse.AnyChar.Except(MsdTagContentDelimiter).Many().Text(), MsdTagContentDelimiter).Parse(content)
            };

        public static Parser<IEnumerable<MsdTag>> Parser =
            from tags in MsdTag.Token().Many()
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