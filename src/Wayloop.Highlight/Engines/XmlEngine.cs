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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Wayloop.Highlight.Patterns;


namespace Wayloop.Highlight.Engines
{
    public class XmlEngine : Engine
    {
        private const string ElementFormat = "<{0}>{1}</{0}>";


        protected override string PreHighlight(Definition definition, string input)
        {
            return Global.HtmlEncode(input);
        }


        protected override string PostHighlight(Definition definition, string input)
        {
            return String.Format("<highlightedInput>{0}</highlightedInput>", input);
        }


        protected override string ElementMatchHandler(Definition definition, Match match)
        {
            foreach (var pattern in definition.Patterns.Where(x => match.Groups[x.Name].Success)) {
                if (pattern is MarkupPattern) {
                    return HandleMarkupPattern(match, pattern);
                }

                return HandlePattern(match, pattern);
            }

            return match.Value;
        }


        private string HandleMarkupPattern(Match match, Pattern pattern)
        {
            var builder = new StringBuilder();
            builder.AppendFormat(ElementFormat, "openTag", match.Groups["openTag"].Value);
            builder.AppendFormat(ElementFormat, "whitespace", match.Groups["ws1"].Value);
            builder.AppendFormat(ElementFormat, "tagName", match.Groups["tagName"].Value);

            var builder2 = new StringBuilder();
            for (var i = 0; i < match.Groups["attribName"].Captures.Count; i++) {
                builder2.AppendFormat(ElementFormat, "whitespace", match.Groups["ws2"].Captures[i].Value);
                builder2.AppendFormat(ElementFormat, "attribName", match.Groups["attribName"].Captures[i].Value);
                builder2.AppendFormat(ElementFormat, "whitespace", match.Groups["ws3"].Captures[i].Value);
                builder2.AppendFormat(ElementFormat, "attribValue", match.Groups["attribSign"].Captures[i].Value + match.Groups["ws4"].Captures[i].Value + match.Groups["attribValue"].Captures[i].Value);
            }
            builder.AppendFormat(ElementFormat, "attribute", builder2);

            builder.AppendFormat(ElementFormat, "whitespace", match.Groups["ws5"].Value);
            builder.AppendFormat(ElementFormat, "closeTag", match.Groups["closeTag"].Value);

            return String.Format(ElementFormat, pattern.Name, builder);
        }


        private string HandlePattern(Capture match, Pattern pattern)
        {
            return String.Format(ElementFormat, pattern.Name, match.Value);
        }
    }
}