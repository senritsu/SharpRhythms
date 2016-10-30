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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Abstractions.Metadata;
    using Sprache;

    public static class MsdTagExtensions
    {
        public static MsdTag Find(this IEnumerable<MsdTag> tags, string tagName) => tags.FirstOrDefault(x => x.Name == tagName);

        public static IEnumerable<MsdTag> FindAll(this IEnumerable<MsdTag> tags, string tagName) => tags.Where(x => x.Name == tagName);

        public static bool AsBoolean(this MsdTag tag) => MsdTagContentParser.YesOrNo.Parse(tag.Content);

        public static double AsDouble(this MsdTag tag, double defaultValue = 0.0)
        {
            double value;
            return double.TryParse(tag.Content, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
                ? value
                : defaultValue;
        }

        public static TextWithAlternative AsTextWithAlternative(this MsdTag tag, MsdTag other = null) => new TextWithAlternative
        {
            Original = tag?.Content,
            Alternative = other?.Content
        };

        public static string TryFindString(this IEnumerable<MsdTag> tags, string tagName, string defaultValue = "")
        {
            var tag = tags.Find(tagName);
            return tag != null ? tag.Content : defaultValue;
        }

        public static double TryFindDouble(this IEnumerable<MsdTag> tags, string tagName, double defaultValue = 0.0)
        {
            var tag = tags.Find(tagName);
            return tag?.AsDouble() ?? defaultValue;
        }

        public static bool TryFindBoolean(this IEnumerable<MsdTag> tags, string tagName, bool defaultValue = false)
        {
            var tag = tags.Find(tagName);
            return tag?.AsBoolean() ?? defaultValue;
        }

        public static TextWithAlternative TryFindTextWithAlternative(this IEnumerable<MsdTag> tags, string textTagName,
            string alternativeTextTagName)
        {
            var tagArray = tags as MsdTag[] ?? tags.ToArray();
            return new TextWithAlternative
            {
                Original = tagArray.TryFindString(textTagName),
                Alternative = tagArray.TryFindString(alternativeTextTagName)
            };
        } 
    }
}
