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
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Highlight.Patterns;


namespace Highlight.Engines
{
    // TODO: Refactor this engine to build proper XML using XLinq.
    public class XmlEngine : Engine
    {
        private const string ElementFormat = "<{0}>{1}</{0}>";


        protected override string PreHighlight(Definition definition, string input)
        {
            return HttpUtility.HtmlEncode(input);
        }


        protected override string PostHighlight(Definition definition, string input)
        {
            return String.Format("<highlightedInput>{0}</highlightedInput>", input);
        }


        protected override string ProcessBlockPatternMatch(Definition definition, BlockPattern pattern, Match match)
        {
            return ProcessPatternMatch(pattern, match);
        }


        protected override string ProcessMarkupPatternMatch(Definition definition, MarkupPattern pattern, Match match)
        {
            var builder = new StringBuilder();
            builder.AppendFormat(ElementFormat, "openTag", match.Groups["openTag"].Value);
            builder.AppendFormat(ElementFormat, "whitespace", match.Groups["ws1"].Value);
            builder.AppendFormat(ElementFormat, "tagName", match.Groups["tagName"].Value);

            var builder2 = new StringBuilder();
            for (var i = 0; i < match.Groups["attribute"].Captures.Count; i++) {
                builder2.AppendFormat(ElementFormat, "whitespace", match.Groups["ws2"].Captures[i].Value);
                builder2.AppendFormat(ElementFormat, "attribName", match.Groups["attribName"].Captures[i].Value);

                if (String.IsNullOrWhiteSpace(match.Groups["attribValue"].Captures[i].Value)) {
                    continue;
                }

                builder2.AppendFormat(ElementFormat, "attribValue", match.Groups["attribValue"].Captures[i].Value);
            }
            builder.AppendFormat(ElementFormat, "attribute", builder2);

            builder.AppendFormat(ElementFormat, "whitespace", match.Groups["ws5"].Value);
            builder.AppendFormat(ElementFormat, "closeTag", match.Groups["closeTag"].Value);

            return String.Format(ElementFormat, pattern.Name, builder);
        }


        protected override string ProcessWordPatternMatch(Definition definition, WordPattern pattern, Match match)
        {
            return ProcessPatternMatch(pattern, match);
        }


        private string ProcessPatternMatch(Pattern pattern, Match match)
        {
            return String.Format(ElementFormat, pattern.Name, match.Value);
        }
    }
}