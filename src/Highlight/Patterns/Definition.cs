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


using System.Collections.Generic;
using System.Text;


namespace Highlight.Patterns
{
    public class Definition
    {
        public Definition(string name, bool caseSensitive, Style style, IDictionary<string, Pattern> patterns)
        {
            Name = name;
            CaseSensitive = caseSensitive;
            Style = style;
            Patterns = patterns;
        }


        public string Name { get; private set; }
        public bool CaseSensitive { get; private set; }
        public Style Style { get; private set; }
        public IDictionary<string, Pattern> Patterns { get; private set; }


        public string GetRegexPattern()
        {
            var allPatterns = new StringBuilder();
            var blockPatterns = new StringBuilder();
            var markupPatterns = new StringBuilder();
            var wordPatterns = new StringBuilder();

            foreach (var pattern in Patterns.Values) {
                if (pattern is BlockPattern) {
                    if (blockPatterns.Length > 1) {
                        blockPatterns.Append("|");
                    }
                    blockPatterns.AppendFormat("(?'{0}'{1})", pattern.Name, pattern.GetRegexPattern());
                }
                else if (pattern is MarkupPattern) {
                    if (markupPatterns.Length > 1) {
                        markupPatterns.Append("|");
                    }
                    markupPatterns.AppendFormat("(?'{0}'{1})", pattern.Name, pattern.GetRegexPattern());
                }
                else if (pattern is WordPattern) {
                    if (wordPatterns.Length > 1) {
                        wordPatterns.Append("|");
                    }
                    wordPatterns.AppendFormat("(?'{0}'{1})", pattern.Name, pattern.GetRegexPattern());
                }
            }

            if (blockPatterns.Length > 0) {
                allPatterns.AppendFormat("(?'blocks'{0})+?", blockPatterns);
            }
            if (markupPatterns.Length > 0) {
                allPatterns.AppendFormat("|(?'markup'{0})+?", markupPatterns);
            }
            if (wordPatterns.Length > 0) {
                allPatterns.AppendFormat("|(?'words'{0})+?", wordPatterns);
            }

            return allPatterns.ToString();
        }


        public override string ToString()
        {
            return Name;
        }
    }
}