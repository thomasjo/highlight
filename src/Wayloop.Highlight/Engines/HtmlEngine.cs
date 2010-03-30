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

            return HttpUtility.HtmlEncode(input);
        }


        protected override string PostHighlight(Definition definition, string input)
        {
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }

            if (UseCss) {
                var cssClassName = HtmlEngineHelper.CreateCssClassName(definition.Name, null);

                return String.Format(ClassSpanFormat, cssClassName, input);
            }

            var cssStyle = HtmlEngineHelper.CreatePatternStyle(definition.Style.Colors.ForeColor, definition.Style.Colors.BackColor, definition.Style.Font);

            return String.Format(StyleSpanFormat, cssStyle, input);
        }


        protected override string ProcessBlockPatternMatch(Definition definition, BlockPattern pattern, Match match)
        {
            return ProcessPatternMatch(definition, pattern, match);
        }


        protected override string ProcessMarkupPatternMatch(Definition definition, MarkupPattern pattern, Match match)
        {
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }
            if (pattern == null) {
                throw new ArgumentNullException("pattern");
            }
            if (match == null) {
                throw new ArgumentNullException("match");
            }

            var result = new StringBuilder();
            if (!UseCss) {
                var patternStyle = HtmlEngineHelper.CreatePatternStyle(pattern.Style.BracketColors.ForeColor, pattern.Style.BracketColors.BackColor, pattern.Style.Font);
                result.AppendFormat(StyleSpanFormat, patternStyle, match.Groups["openTag"].Value);

                result.Append(match.Groups["ws1"].Value);

                patternStyle = HtmlEngineHelper.CreatePatternStyle(pattern.Style.Colors.ForeColor, pattern.Style.Colors.BackColor, pattern.Style.Font);
                result.AppendFormat(StyleSpanFormat, patternStyle, match.Groups["tagName"].Value);

                if (pattern.HighlightAttributes) {
                    var highlightedAttributes = ProcessMarkupPatternAttributeMatches(definition, pattern, match);
                    result.Append(highlightedAttributes);
                }

                result.Append(match.Groups["ws5"].Value);

                patternStyle = HtmlEngineHelper.CreatePatternStyle(pattern.Style.BracketColors.ForeColor, pattern.Style.BracketColors.BackColor, pattern.Style.Font);
                result.AppendFormat(StyleSpanFormat, patternStyle, match.Groups["closeTag"].Value);
            }
            else {
                var cssClassName = HtmlEngineHelper.CreateCssClassName(definition.Name, pattern.Name + "Bracket");
                result.AppendFormat(ClassSpanFormat, cssClassName, match.Groups["openTag"].Value);

                result.Append(match.Groups["ws1"].Value);

                cssClassName = HtmlEngineHelper.CreateCssClassName(definition.Name, pattern.Name + "TagName");
                result.AppendFormat(ClassSpanFormat, cssClassName, match.Groups["tagName"].Value);

                if (pattern.HighlightAttributes) {
                    var highlightedAttributes = ProcessMarkupPatternAttributeMatches(definition, pattern, match);
                    result.Append(highlightedAttributes);
                }

                result.Append(match.Groups["ws5"].Value);

                cssClassName = HtmlEngineHelper.CreateCssClassName(definition.Name, pattern.Name + "Bracket");
                result.AppendFormat(ClassSpanFormat, cssClassName, match.Groups["closeTag"].Value);
            }

            return result.ToString();
        }


        protected override string ProcessWordPatternMatch(Definition definition, WordPattern pattern, Match match)
        {
            return ProcessPatternMatch(definition, pattern, match);
        }


        private string ProcessPatternMatch(Definition definition, Pattern pattern, Match match)
        {
            if (!UseCss) {
                var patternStyle = HtmlEngineHelper.CreatePatternStyle(pattern.Style.Colors.ForeColor, pattern.Style.Colors.BackColor, pattern.Style.Font);

                return String.Format(StyleSpanFormat, patternStyle, match.Value);
            }

            return String.Format(ClassSpanFormat, HtmlEngineHelper.CreateCssClassName(definition.Name, pattern.Name), match.Value);
        }


        private string ProcessMarkupPatternAttributeMatches(Definition definition, MarkupPattern pattern, Match match)
        {
            var result = new StringBuilder();
            for (var i = 0; i < match.Groups["attribute"].Captures.Count; i++) {
                result.Append(match.Groups["ws2"].Captures[i].Value);
                if (!UseCss) {
                    var patternStyle = HtmlEngineHelper.CreatePatternStyle(pattern.Style.AttributeNameColors.ForeColor, pattern.Style.AttributeNameColors.BackColor, pattern.Style.Font);
                    result.AppendFormat(StyleSpanFormat, patternStyle, match.Groups["attribName"].Captures[i].Value);
                }
                else {
                    result.AppendFormat(ClassSpanFormat, HtmlEngineHelper.CreateCssClassName(definition.Name, pattern.Name + "AttributeName"), match.Groups["attribName"].Captures[i].Value);
                }

                if (String.IsNullOrWhiteSpace(match.Groups["attribValue"].Captures[i].Value)) {
                    continue;
                }

                if (!UseCss) {
                    var patternStyle = HtmlEngineHelper.CreatePatternStyle(pattern.Style.AttributeValueColors.ForeColor, pattern.Style.AttributeValueColors.BackColor, pattern.Style.Font);
                    result.AppendFormat(StyleSpanFormat, patternStyle, match.Groups["attribValue"].Captures[i].Value);
                }
                else {
                    result.AppendFormat(ClassSpanFormat, HtmlEngineHelper.CreateCssClassName(definition.Name, pattern.Name + "AttributeValue"), match.Groups["attribValue"].Captures[i].Value);
                }
            }

            return result.ToString();
        }
    }
}