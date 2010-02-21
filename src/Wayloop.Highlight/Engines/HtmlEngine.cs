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


using System.Text;
using System.Text.RegularExpressions;
using Wayloop.Highlight.Patterns;


namespace Wayloop.Highlight.Engines
{
    public class HtmlEngine : Engine
    {
        public bool UseCss { get; set; }


        protected override string ElementMatchHandler(Definition definition, Match match)
        {
            var builder = new StringBuilder();
            const string format = "<span style=\"{0}\">{1}</span>";
            const string str2 = "<span class=\"{0}\">{1}</span>";
            var str3 = string.Empty;
            var str4 = string.Empty;
            var str5 = string.Empty;
            var str6 = string.Empty;
            foreach (Pattern pattern2 in definition.Patterns) {
                if (!match.Groups[pattern2.Name].Success) {
                    continue;
                }
                if (pattern2 is BlockPattern) {
                    if (!UseCss) {
                        str3 = Global.CreatePatternStyle(pattern2.Style.ForeColor, pattern2.Style.BackColor, pattern2.Style.Font);
                        return string.Format(format, str3, match.Value);
                    }
                    return string.Format(str2, Global.CreateCssClassName(definition.Name, pattern2.Name), match.Value);
                }
                if (pattern2 is MarkupPattern) {
                    var pattern = (MarkupPattern) pattern2;
                    if (!UseCss) {
                        str3 = Global.CreatePatternStyle(pattern.Style.ForeColor, pattern.Style.BackColor, pattern.Style.Font);
                        str4 = Global.CreatePatternStyle(pattern.BracketForeColor, pattern.BracketBackColor, pattern.Style.Font);
                        if (pattern.HighlightAttributes) {
                            str5 = Global.CreatePatternStyle(pattern.AttributeNameForeColor, pattern.AttributeNameBackColor, pattern.Style.Font);
                            str6 = Global.CreatePatternStyle(pattern.AttributeValueForeColor, pattern.AttributeValueBackColor, pattern.Style.Font);
                        }
                    }
                    if (!UseCss) {
                        builder.AppendFormat(format, str4, match.Groups["openTag"].Value);
                    }
                    else {
                        builder.AppendFormat(str2, Global.CreateCssClassName(definition.Name, pattern2.Name + "Bracket"), match.Groups["openTag"].Value);
                    }
                    builder.Append(match.Groups["ws1"].Value);
                    if (!UseCss) {
                        builder.AppendFormat(format, str3, match.Groups["tagName"].Value);
                    }
                    else {
                        builder.AppendFormat(str2, Global.CreateCssClassName(definition.Name, pattern2.Name + "TagName"), match.Groups["tagName"].Value);
                    }
                    if (pattern.HighlightAttributes) {
                        for (var i = 0; i < match.Groups["attribName"].Captures.Count; i++) {
                            builder.Append(match.Groups["ws2"].Captures[i].Value);
                            if (!UseCss) {
                                builder.AppendFormat(format, str5, match.Groups["attribName"].Captures[i].Value);
                            }
                            else {
                                builder.AppendFormat(str2, Global.CreateCssClassName(definition.Name, pattern2.Name + "AttributeName"), match.Groups["attribName"].Captures[i].Value);
                            }
                            builder.Append(match.Groups["ws3"].Captures[i].Value);
                            if (!UseCss) {
                                builder.AppendFormat(format, str6, match.Groups["attribSign"].Captures[i].Value + match.Groups["ws4"].Captures[i].Value + match.Groups["attribValue"].Captures[i].Value);
                            }
                            else {
                                builder.AppendFormat(str2, Global.CreateCssClassName(definition.Name, pattern2.Name + "AttributeValue"), match.Groups["attribSign"].Captures[i].Value + match.Groups["ws4"].Captures[i].Value + match.Groups["attribValue"].Captures[i].Value);
                            }
                        }
                    }
                    builder.Append(match.Groups["ws5"].Value);
                    if (!UseCss) {
                        builder.AppendFormat(format, str4, match.Groups["closeTag"].Value);
                    }
                    else {
                        builder.AppendFormat(str2, Global.CreateCssClassName(definition.Name, pattern2.Name + "Bracket"), match.Groups["closeTag"].Value);
                    }
                    return builder.ToString();
                }
                if (pattern2 is WordPattern) {
                    if (!UseCss) {
                        str3 = Global.CreatePatternStyle(pattern2.Style.ForeColor, pattern2.Style.BackColor, pattern2.Style.Font);
                        return string.Format(format, str3, match.Value);
                    }
                    return string.Format(str2, Global.CreateCssClassName(definition.Name, pattern2.Name), match.Value);
                }
            }
            return match.Value;
        }

/*
        protected override void Highlight()
        {
            var evaluator = new MatchEvaluator(ElementMatchHandler);
            var patterns = Definition.GetPatterns();
            if (Definition.CaseSensitive) {
                Input = Regex.Replace(Input, patterns, evaluator, RegexOptions.ExplicitCapture);
            }
            else {
                Input = Regex.Replace(Input, patterns, evaluator, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            }
            if (!UseCss) {
                Input = string.Format("<span style=\"{0}\">{1}</span>", Global.CreatePatternStyle(Definition.ForeColor, Definition.BackColor, Definition.Font), Input);
            }
            else {
                Input = string.Format("<span class=\"{0}\">{1}</span>", Global.CreateCssClassName(Definition.Name, null), Input);
            }
        }
*/
    }
}