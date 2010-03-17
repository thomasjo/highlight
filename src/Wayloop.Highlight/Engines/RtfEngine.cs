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
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Wayloop.Highlight.Patterns;


namespace Wayloop.Highlight.Engines
{
    public class RtfEngine : Engine
    {
        private readonly ArrayList colors = new ArrayList();
        private readonly ArrayList fonts = new ArrayList();


        protected override string PreHighlight(Definition definition, string input)
        {
            return Global.HtmlEncode(input);
        }


        protected override string PostHighlight(Definition definition, string input)
        {
            var result = input
                .Replace("{", @"\{").Replace("}", @"\}").Replace("\t", @"\tab ")
                .Replace("\r\n", @"\par ");
            var fontList = BuildFontList();
            var colorList = BuildColorList();

            return String.Format(@"{{\rtf1\ansi{{\fonttbl{{{0}}}}}{{\colortbl;{1}}}{2}}}", fontList, colorList, result);
        }


        protected override string ElementMatchHandler(Definition definition, Match match)
        {
            var builder = new StringBuilder();
            const string format = "{0} {1}";
            var str4 = string.Empty;
            var str5 = string.Empty;
            foreach (var pattern in definition.Patterns.Where(pattern => match.Groups[pattern.Name].Success)) {
                string str2;
                if (pattern is BlockPattern) {
                    str2 = CreateRtfPatternStyle(pattern.Style.ForeColor, pattern.Style.BackColor, pattern.Style.Font);

                    return ("{" + String.Format(format, str2, match.Value) + "}");
                }
                if (pattern is MarkupPattern) {
                    var markupPattern = (MarkupPattern) pattern;
                    str2 = CreateRtfPatternStyle(markupPattern.Style.ForeColor, markupPattern.Style.BackColor, markupPattern.Style.Font);
                    var str3 = CreateRtfPatternStyle(markupPattern.BracketForeColor, markupPattern.BracketBackColor, markupPattern.Style.Font);
                    if (markupPattern.HighlightAttributes) {
                        str4 = CreateRtfPatternStyle(markupPattern.AttributeNameForeColor, markupPattern.AttributeNameBackColor, markupPattern.Style.Font);
                        str5 = CreateRtfPatternStyle(markupPattern.AttributeValueForeColor, markupPattern.AttributeValueBackColor, markupPattern.Style.Font);
                    }
                    builder.AppendFormat(format, str3, match.Groups["openTag"].Value);
                    builder.Append(match.Groups["ws1"].Value);
                    builder.AppendFormat(format, str2, match.Groups["tagName"].Value);
                    if (str4 != null) {
                        for (var i = 0; i < match.Groups["attribName"].Captures.Count; i++) {
                            builder.Append(match.Groups["ws2"].Captures[i].Value);
                            builder.AppendFormat(format, str4, match.Groups["attribName"].Captures[i].Value);
                            builder.Append(match.Groups["ws3"].Captures[i].Value);
                            builder.AppendFormat(format, str5, match.Groups["attribSign"].Captures[i].Value + match.Groups["ws4"].Captures[i].Value + match.Groups["attribValue"].Captures[i].Value);
                        }
                    }
                    builder.Append(match.Groups["ws5"].Value);
                    builder.AppendFormat(format, str3, match.Groups["closeTag"].Value);

                    return ("{" + builder + "}");
                }
                if (pattern is WordPattern) {
                    str2 = CreateRtfPatternStyle(pattern.Style.ForeColor, pattern.Style.BackColor, pattern.Style.Font);

                    return ("{" + String.Format(format, str2, match.Value) + "}");
                }
            }

            return match.Value;
        }


        private string CreateRtfPatternStyle(Color foreColor, Color backColor, Font font)
        {
            return String.Concat(new object[] { @"\cf", GetIndexOfColor(foreColor), @"\highlight", GetIndexOfColor(backColor), @"\f", GetIndexOfFont(font.Name), @"\fs", font.Size * 2f });
        }


        private int GetIndexOfColor(Color color)
        {
            var color2 = new HexColor();
            if (color.Name.IndexOf("#") > -1) {
                color2.Red = Int32.Parse(color.Name.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                color2.Green = Int32.Parse(color.Name.Substring(3, 2), NumberStyles.AllowHexSpecifier);
                color2.Blue = Int32.Parse(color.Name.Substring(5, 2), NumberStyles.AllowHexSpecifier);
            }
            else {
                color2.Red = color.R;
                color2.Green = color.G;
                color2.Blue = color.B;
            }
            var index = colors.IndexOf(color2);
            if (index > -1) {
                return (index + 1);
            }
            colors.Add(color2);

            return colors.Count;
        }


        private int GetIndexOfFont(string font)
        {
            var index = fonts.IndexOf(font);
            if (index > -1) {
                return (index + 1);
            }
            fonts.Add(font);

            return fonts.Count;
        }


        private string BuildColorList()
        {
            var builder = new StringBuilder();
            foreach (var hexColor in colors.Cast<HexColor>()) {
                builder.AppendFormat(@"\red{0}\green{1}\blue{2};", hexColor.Red, hexColor.Green, hexColor.Blue);
            }
            return builder.ToString();
        }


        private string BuildFontList()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < fonts.Count; i++) {
                builder.AppendFormat(@"\f{0} {1};", i, fonts[i]);
            }
            return builder.ToString();
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct HexColor
        {
            public int Red;
            public int Green;
            public int Blue;
        }
    }
}