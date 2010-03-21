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
    // TODO: Clean up and refactor big methods into smaller, more manageable chunks.
    public class RtfEngine : Engine
    {
        private const string RtfFormat = "{0} {1}";
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


        protected override string HandlePattern(Definition definition, Pattern pattern, Match match)
        {
            var style = CreateRtfPatternStyle(pattern.Style.Colors.ForeColor, pattern.Style.Colors.BackColor, pattern.Style.Font);

            return ("{" + String.Format(RtfFormat, style, match.Value) + "}");
        }


        protected override string HandleMarkupPattern(Definition definition, Pattern pattern, Match match)
        {
            var builder = new StringBuilder();
            var markupPattern = (MarkupPattern) pattern;
            var style = CreateRtfPatternStyle(markupPattern.Style.Colors.ForeColor, markupPattern.Style.Colors.BackColor, markupPattern.Style.Font);
            var bracketStyle = CreateRtfPatternStyle(markupPattern.Style.BracketColors.ForeColor, markupPattern.Style.BracketColors.BackColor, markupPattern.Style.Font);
            string attributeNameStyle = null;
            string attributeValueStyle = null;
            if (markupPattern.HighlightAttributes) {
                attributeNameStyle = CreateRtfPatternStyle(markupPattern.Style.AttributeNameColors.ForeColor, markupPattern.Style.AttributeNameColors.BackColor, markupPattern.Style.Font);
                attributeValueStyle = CreateRtfPatternStyle(markupPattern.Style.AttributeValueColors.ForeColor, markupPattern.Style.AttributeValueColors.BackColor, markupPattern.Style.Font);
            }
            builder.AppendFormat(RtfFormat, bracketStyle, match.Groups["openTag"].Value);
            builder.Append(match.Groups["ws1"].Value);
            builder.AppendFormat(RtfFormat, style, match.Groups["tagName"].Value);
            if (attributeNameStyle != null) {
                for (var i = 0; i < match.Groups["attribName"].Captures.Count; i++) {
                    builder.Append(match.Groups["ws2"].Captures[i].Value);
                    builder.AppendFormat(RtfFormat, attributeNameStyle, match.Groups["attribName"].Captures[i].Value);
                    builder.Append(match.Groups["ws3"].Captures[i].Value);
                    builder.AppendFormat(RtfFormat, attributeValueStyle, match.Groups["attribSign"].Captures[i].Value + match.Groups["ws4"].Captures[i].Value + match.Groups["attribValue"].Captures[i].Value);
                }
            }
            builder.Append(match.Groups["ws5"].Value);
            builder.AppendFormat(RtfFormat, bracketStyle, match.Groups["closeTag"].Value);

            return ("{" + builder + "}");
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