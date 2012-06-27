#region License

// Copyright (c) 2004-2010 Thomas Andre H. Johansen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Wayloop.Highlight.Patterns
{
    public sealed class WordPattern : Pattern
    {
        public IEnumerable<string> Words { get; private set; }


        public WordPattern(string name, Style style, IEnumerable<string> words) : base(name, style)
        {
            Words = words;
        }


        public override string GetRegexPattern()
        {
            var str = String.Empty;
            if (Words.Count() > 0) {
                var nonWords = GetNonWords();
                str = String.Format(@"(?<![\w{0}])(?=[\w{0}])({1})(?<=[\w{0}])(?![\w{0}])", nonWords, String.Join("|", Words.ToArray()));
            }

            return str;
        }


        private string GetNonWords()
        {
            var input = String.Join("", Words.ToArray());
            var list = new List<string>();
            foreach (var match in Regex.Matches(input, @"\W").Cast<Match>().Where(x => !list.Contains(x.Value))) {
                list.Add(match.Value);
            }

            return String.Join("", list.ToArray());
        }
    }
}