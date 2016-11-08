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

namespace SharpRhythms.Parsers.Msd.Base
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Formats;
    using Sprache;

    public abstract class SpecificMsdParser<T> where T : IMsdTrack, new()
    {
        private readonly AudioLengthAccessor _audioLengthAccessor;
        private T _track;

        protected delegate void TagAction(MsdTag tag, T track);
        public delegate double AudioLengthAccessor(string audioFilePath);

        protected abstract T BuildTrack(IEnumerable<MsdTag> tags);
        protected virtual void PostprocessTrack(T track) {}

        protected double SongLength => _audioLengthAccessor(_track.Files.Music);

        protected SpecificMsdParser(AudioLengthAccessor audioLengthAccessor)
        {
            _audioLengthAccessor = audioLengthAccessor;
        }

        private T Parse(string fileContents)
        {
            var commentRegex = new Regex("//[^\n]*\n");
            fileContents = commentRegex.Replace(fileContents, "");

            var tags = MsdParser.Parser.Parse(fileContents).ToArray();
            _track = BuildTrack(tags);

            foreach (var tag in MsdParser.Parser.Parse(fileContents))
            {
                if (TagActions.ContainsKey(tag.Name))
                {
                    TagActions[tag.Name](tag, _track);
                }
            }

            PostprocessTrack(_track);

            return _track;
        }

        protected Dictionary<string, TagAction> TagActions { get; } = new Dictionary<string, TagAction>();

        public T Load(string smFileContents)
        {
            return Parse(smFileContents);
        }

        public T Load(Stream smFileContentsStream)
        {
            using (var reader = new StreamReader(smFileContentsStream))
            {
                return Parse(reader.ReadToEnd());
            }
        }

        public T LoadFromFile(string path)
        {
            var fileContents = File.ReadAllText(path);
            return Parse(fileContents);
        }
    }
}
