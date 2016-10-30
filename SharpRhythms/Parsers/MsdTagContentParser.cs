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
    using Abstractions.Timing;
    using Sprache;

    public static class MsdTagContentParser
    {
        private static readonly Parser<char> ListDelimiter = Parse.Char(',').Token();

        private static readonly Parser<char> ComplexContentDelimiter = Parse.Char(':');

        public static readonly Parser<string> ComplexContentPart =
            Parse.AnyChar.Except(ComplexContentDelimiter)
                .Many()
                .Text()
                .Then(
                    part =>
                        ComplexContentDelimiter.Select(d => d.ToString())
                            .Or(Parse.Return("").End())
                            .Then(x => Parse.Return(part))).Token();

        public static Parser<TimeIndexedValue> TimeIndexedValue =
            from time in Parse.DecimalInvariant
            from equal in Parse.Char('=').Token()
            from value in Parse.DecimalInvariant
            select new TimeIndexedValue
            {
                Time = double.Parse(time, CultureInfo.InvariantCulture),
                Value = double.Parse(value, CultureInfo.InvariantCulture)
            };

        public static Parser<IEnumerable<T>> ListContent<T>(Parser<T> itemParser) => Utilities.ListOf(itemParser.Except(ListDelimiter), ListDelimiter);

        public static Parser<IEnumerable<string>> ComplexContent =
            Utilities.ListOf(Parse.AnyChar.Except(ComplexContentDelimiter).Many().Text(), ComplexContentDelimiter);

        public static Parser<bool> YesOrNo = Parse.String("YES").Return(true).XOr(Parse.String("NO").Return(false));
    }
}
