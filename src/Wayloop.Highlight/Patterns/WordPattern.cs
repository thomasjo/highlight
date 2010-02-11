#region License

// Copyright (c) 2010 Thomas Andre H. Johansen
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
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;


namespace Wayloop.Highlight.Patterns
{
    public class WordPattern : Pattern
    {
        public WordPattern()
        {
            Words = new ArrayList();
        }


        public WordPattern(XmlNode patternNode) : base(patternNode)
        {
            if (patternNode == null) {
                throw new ArgumentNullException("patternNode");
            }

            Words = new ArrayList();
            var wordNodes = patternNode.SelectNodes("word");
            if (wordNodes == null) {
                throw new NotImplementedException();
            }

            foreach (XmlNode wordNode in wordNodes) {
                Words.Add(Regex.Escape(wordNode.InnerText));
            }
        }


        public ArrayList Words { get; private set; }


        private string GetNonWords()
        {
            var input = string.Join("", (string[]) Words.ToArray(typeof (string)));
            var list = new ArrayList();
            foreach (Match match in Regex.Matches(input, @"\W")) {
                if (!list.Contains(match.Value)) {
                    list.Add(match.Value);
                }
            }

            return string.Join("", (string[]) list.ToArray(typeof (string)));
        }


        public override string GetPatternString()
        {
            var str = string.Empty;
            if (Words.Count > 0) {
                var nonWords = GetNonWords();
                str = string.Format(@"(?<![\w{0}])(?=[\w{0}])({1})(?<=[\w{0}])(?![\w{0}])", nonWords, string.Join("|", (string[]) Words.ToArray(typeof (string))));
            }

            return str;
        }
    }
}