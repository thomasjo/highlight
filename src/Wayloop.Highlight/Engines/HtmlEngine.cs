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
using System.Text;
using System.Text.RegularExpressions;
using Wayloop.Highlight.Patterns;


namespace Wayloop.Highlight.Engines
{
    public class HtmlEngine : Engine
    {
        public bool UseCss { get; set; }


        protected override string PreHighlight(Definition definition, string input)
        {
            return Global.HtmlEncode(input);
        }


        protected override string PostHighlight(Definition definition, string input)
        {
            var result = Global.HtmlDecode(input);

            if (UseCss) {
                return String.Format("<span class=\"{0}\">{1}</span>", Global.CreateCssClassName(definition.Name, null), result);
            }

            return String.Format("<span style=\"{0}\">{1}</span>", Global.CreatePatternStyle(definition.ForeColor, definition.BackColor, definition.Font), result);
        }


        protected override string ElementMatchHandler(Definition definition, Match match)
        {
            var builder = new StringBuilder();
            const string format = "<span style=\"{0}\">{1}</span>";
            const string str2 = "<span class=\"{0}\">{1}</span>";
            var str3 = String.Empty;
            var str4 = String.Empty;
            var str5 = String.Empty;
            var str6 = String.Empty;
            foreach (var pattern in definition.Patterns) {
                if (!match.Groups[pattern.Name].Success) {
                    continue;
                }
                if (pattern is BlockPattern) {
                    if (!UseCss) {
                        str3 = Global.CreatePatternStyle(pattern.Style.ForeColor, pattern.Style.BackColor, pattern.Style.Font);

                        return String.Format(format, str3, match.Value);
                    }

                    return String.Format(str2, Global.CreateCssClassName(definition.Name, pattern.Name), match.Value);
                }
                if (pattern is MarkupPattern) {
                    var markupPattern = (MarkupPattern) pattern;
                    if (!UseCss) {
                        str3 = Global.CreatePatternStyle(markupPattern.Style.ForeColor, markupPattern.Style.BackColor, markupPattern.Style.Font);
                        str4 = Global.CreatePatternStyle(markupPattern.BracketForeColor, markupPattern.BracketBackColor, markupPattern.Style.Font);
                        if (markupPattern.HighlightAttributes) {
                            str5 = Global.CreatePatternStyle(markupPattern.AttributeNameForeColor, markupPattern.AttributeNameBackColor, markupPattern.Style.Font);
                            str6 = Global.CreatePatternStyle(markupPattern.AttributeValueForeColor, markupPattern.AttributeValueBackColor, markupPattern.Style.Font);
                        }
                    }
                    if (!UseCss) {
                        builder.AppendFormat(format, str4, match.Groups["openTag"].Value);
                    }
                    else {
                        builder.AppendFormat(str2, Global.CreateCssClassName(definition.Name, pattern.Name + "Bracket"), match.Groups["openTag"].Value);
                    }
                    builder.Append(match.Groups["ws1"].Value);
                    if (!UseCss) {
                        builder.AppendFormat(format, str3, match.Groups["tagName"].Value);
                    }
                    else {
                        builder.AppendFormat(str2, Global.CreateCssClassName(definition.Name, pattern.Name + "TagName"), match.Groups["tagName"].Value);
                    }
                    if (markupPattern.HighlightAttributes) {
                        for (var i = 0; i < match.Groups["attribName"].Captures.Count; i++) {
                            builder.Append(match.Groups["ws2"].Captures[i].Value);
                            if (!UseCss) {
                                builder.AppendFormat(format, str5, match.Groups["attribName"].Captures[i].Value);
                            }
                            else {
                                builder.AppendFormat(str2, Global.CreateCssClassName(definition.Name, pattern.Name + "AttributeName"), match.Groups["attribName"].Captures[i].Value);
                            }
                            builder.Append(match.Groups["ws3"].Captures[i].Value);
                            if (!UseCss) {
                                builder.AppendFormat(format, str6, match.Groups["attribSign"].Captures[i].Value + match.Groups["ws4"].Captures[i].Value + match.Groups["attribValue"].Captures[i].Value);
                            }
                            else {
                                builder.AppendFormat(str2, Global.CreateCssClassName(definition.Name, pattern.Name + "AttributeValue"), match.Groups["attribSign"].Captures[i].Value + match.Groups["ws4"].Captures[i].Value + match.Groups["attribValue"].Captures[i].Value);
                            }
                        }
                    }
                    builder.Append(match.Groups["ws5"].Value);
                    if (!UseCss) {
                        builder.AppendFormat(format, str4, match.Groups["closeTag"].Value);
                    }
                    else {
                        builder.AppendFormat(str2, Global.CreateCssClassName(definition.Name, pattern.Name + "Bracket"), match.Groups["closeTag"].Value);
                    }
                    return builder.ToString();
                }
                if (pattern is WordPattern) {
                    if (!UseCss) {
                        str3 = Global.CreatePatternStyle(pattern.Style.ForeColor, pattern.Style.BackColor, pattern.Style.Font);

                        return String.Format(format, str3, match.Value);
                    }

                    return String.Format(str2, Global.CreateCssClassName(definition.Name, pattern.Name), match.Value);
                }
            }

            return match.Value;
        }
    }
}