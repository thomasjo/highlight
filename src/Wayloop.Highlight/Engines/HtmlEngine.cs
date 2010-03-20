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
    public class HtmlEngine : Engine
    {
        private const string StyleSpanFormat = "<span style=\"{0}\">{1}</span>";
        private const string ClassSpanFormat = "<span class=\"{0}\">{1}</span>";

        public bool UseCss { get; set; }


        protected override string PreHighlight(Definition definition, string input)
        {
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }

            return Global.HtmlEncode(input);
        }


        protected override string PostHighlight(Definition definition, string input)
        {
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }

            if (UseCss) {
                return String.Format(ClassSpanFormat, Global.CreateCssClassName(definition.Name, null), input);
            }

            return String.Format(StyleSpanFormat, Global.CreatePatternStyle(definition.Style.ForeColor, definition.Style.BackColor, definition.Style.Font), input);
        }


        protected override string ElementMatchHandler(Definition definition, Match match)
        {
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }
            if (match == null) {
                throw new ArgumentNullException("match");
            }

            var matchedPatterns = definition.Patterns.Where(pattern => match.Groups[pattern.Name].Success);
            foreach (var pattern in matchedPatterns) {
                if (pattern is MarkupPattern) {
                    return HandleMarkupPattern(definition, pattern, match);
                }

                return HandlePattern(definition, pattern, match);
            }

            return match.Value;
        }


        private string HandlePattern(Definition definition, Pattern pattern, Capture match)
        {
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }
            if (match == null) {
                throw new ArgumentNullException("match");
            }
            if (pattern == null) {
                throw new ArgumentNullException("pattern");
            }

            if (!UseCss) {
                var patternStyle = Global.CreatePatternStyle(pattern.Style.ForeColor, pattern.Style.BackColor, pattern.Style.Font);

                return String.Format(StyleSpanFormat, patternStyle, match.Value);
            }

            return String.Format(ClassSpanFormat, Global.CreateCssClassName(definition.Name, pattern.Name), match.Value);
        }


        private string HandleMarkupPattern(Definition definition, Pattern pattern, Match match)
        {
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }
            if (match == null) {
                throw new ArgumentNullException("match");
            }
            if (pattern == null) {
                throw new ArgumentNullException("pattern");
            }

            var builder = new StringBuilder();
            var markupPattern = (MarkupPattern) pattern;
            if (!UseCss) {
                var patternStyle = Global.CreatePatternStyle(markupPattern.Style.BracketForeColor, markupPattern.Style.BracketBackColor, markupPattern.Style.Font);
                builder.AppendFormat(StyleSpanFormat, patternStyle, match.Groups["openTag"].Value);
            }
            else {
                builder.AppendFormat(ClassSpanFormat, Global.CreateCssClassName(definition.Name, pattern.Name + "Bracket"), match.Groups["openTag"].Value);
            }

            builder.Append(match.Groups["ws1"].Value);

            if (!UseCss) {
                var patternStyle = Global.CreatePatternStyle(markupPattern.Style.ForeColor, markupPattern.Style.BackColor, markupPattern.Style.Font);
                builder.AppendFormat(StyleSpanFormat, patternStyle, match.Groups["tagName"].Value);
            }
            else {
                builder.AppendFormat(ClassSpanFormat, Global.CreateCssClassName(definition.Name, pattern.Name + "TagName"), match.Groups["tagName"].Value);
            }

            if (markupPattern.HighlightAttributes) {
                for (var i = 0; i < match.Groups["attribName"].Captures.Count; i++) {
                    builder.Append(match.Groups["ws2"].Captures[i].Value);
                    if (!UseCss) {
                        var patternStyle = Global.CreatePatternStyle(markupPattern.Style.AttributeNameForeColor, markupPattern.Style.AttributeNameBackColor, markupPattern.Style.Font);
                        builder.AppendFormat(StyleSpanFormat, patternStyle, match.Groups["attribName"].Captures[i].Value);
                    }
                    else {
                        builder.AppendFormat(ClassSpanFormat, Global.CreateCssClassName(definition.Name, pattern.Name + "AttributeName"), match.Groups["attribName"].Captures[i].Value);
                    }

                    builder.Append(match.Groups["ws3"].Captures[i].Value);

                    if (!UseCss) {
                        var patternStyle = Global.CreatePatternStyle(markupPattern.Style.AttributeValueForeColor, markupPattern.Style.AttributeValueBackColor, markupPattern.Style.Font);
                        builder.AppendFormat(StyleSpanFormat, patternStyle, match.Groups["attribSign"].Captures[i].Value + match.Groups["ws4"].Captures[i].Value + match.Groups["attribValue"].Captures[i].Value);
                    }
                    else {
                        builder.AppendFormat(ClassSpanFormat, Global.CreateCssClassName(definition.Name, pattern.Name + "AttributeValue"), match.Groups["attribSign"].Captures[i].Value + match.Groups["ws4"].Captures[i].Value + match.Groups["attribValue"].Captures[i].Value);
                    }
                }
            }

            builder.Append(match.Groups["ws5"].Value);

            if (!UseCss) {
                var patternStyle = Global.CreatePatternStyle(markupPattern.Style.BracketForeColor, markupPattern.Style.BracketBackColor, markupPattern.Style.Font);
                builder.AppendFormat(StyleSpanFormat, patternStyle, match.Groups["closeTag"].Value);
            }
            else {
                builder.AppendFormat(ClassSpanFormat, Global.CreateCssClassName(definition.Name, pattern.Name + "Bracket"), match.Groups["closeTag"].Value);
            }

            return builder.ToString();
        }
    }
}