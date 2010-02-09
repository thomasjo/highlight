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


using System.Collections;
using System.Drawing;
using System.Globalization;
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


        private string BuildColorList()
        {
            var builder = new StringBuilder();
            foreach (var color in colors) {
                var hexColor = (HexColor) color;
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


        private string CreateRtfPatternStyle(Color foreColor, Color backColor, Font font)
        {
            return string.Concat(new object[] { @"\cf", GetIndexOfColor(foreColor), @"\highlight", GetIndexOfColor(backColor), @"\f", GetIndexOfFont(font.Name), @"\fs", font.Size * 2f });
        }


        protected override string ElementMatchHandler(Match m)
        {
            var builder = new StringBuilder();
            const string format = "{0} {1}";
            var str4 = string.Empty;
            var str5 = string.Empty;
            foreach (Pattern pattern2 in Definition.Patterns) {
                if (!m.Groups[pattern2.Name].Success) {
                    continue;
                }
                string str2;
                if (pattern2 is BlockPattern) {
                    str2 = CreateRtfPatternStyle(pattern2.ForeColor, pattern2.BackColor, pattern2.Font);
                    return ("{" + string.Format(format, str2, m.Value) + "}");
                }
                if (pattern2 is MarkupPattern) {
                    var pattern = (MarkupPattern) pattern2;
                    str2 = CreateRtfPatternStyle(pattern.ForeColor, pattern.BackColor, pattern.Font);
                    var str3 = CreateRtfPatternStyle(pattern.BracketForeColor, pattern.BracketBackColor, pattern.Font);
                    if (pattern.HighlightAttributes) {
                        str4 = CreateRtfPatternStyle(pattern.AttributeNameForeColor, pattern.AttributeNameBackColor, pattern.Font);
                        str5 = CreateRtfPatternStyle(pattern.AttributeValueForeColor, pattern.AttributeValueBackColor, pattern.Font);
                    }
                    builder.AppendFormat(format, str3, m.Groups["openTag"].Value);
                    builder.Append(m.Groups["ws1"].Value);
                    builder.AppendFormat(format, str2, m.Groups["tagName"].Value);
                    if (str4 != null) {
                        for (var i = 0; i < m.Groups["attribName"].Captures.Count; i++) {
                            builder.Append(m.Groups["ws2"].Captures[i].Value);
                            builder.AppendFormat(format, str4, m.Groups["attribName"].Captures[i].Value);
                            builder.Append(m.Groups["ws3"].Captures[i].Value);
                            builder.AppendFormat(format, str5, m.Groups["attribSign"].Captures[i].Value + m.Groups["ws4"].Captures[i].Value + m.Groups["attribValue"].Captures[i].Value);
                        }
                    }
                    builder.Append(m.Groups["ws5"].Value);
                    builder.AppendFormat(format, str3, m.Groups["closeTag"].Value);
                    return ("{" + builder + "}");
                }
                if (pattern2 is WordPattern) {
                    str2 = CreateRtfPatternStyle(pattern2.ForeColor, pattern2.BackColor, pattern2.Font);
                    return ("{" + string.Format(format, str2, m.Value) + "}");
                }
            }
            return m.Value;
        }


        private int GetIndexOfColor(Color color)
        {
            var color2 = new HexColor();
            if (color.Name.IndexOf("#") > -1) {
                color2.Red = int.Parse(color.Name.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                color2.Green = int.Parse(color.Name.Substring(3, 2), NumberStyles.AllowHexSpecifier);
                color2.Blue = int.Parse(color.Name.Substring(5, 2), NumberStyles.AllowHexSpecifier);
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


        protected override void Highlight()
        {
            string str2;
            var evaluator = new MatchEvaluator(ElementMatchHandler);
            var patterns = Definition.GetPatterns();
            var input = Input.Replace("{", @"\{").Replace("}", @"\}").Replace("\t", @"\tab ");
            if (Definition.CaseSensitive) {
                str2 = Regex.Replace(input, patterns, evaluator, RegexOptions.ExplicitCapture);
            }
            else {
                str2 = Regex.Replace(input, patterns, evaluator, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            }
            str2 = str2.Replace("\r\n", @"\par ");
            Input = @"{\rtf1\ansi{\fonttbl{" + BuildFontList() + @"}}{\colortbl;" + BuildColorList() + "}" + str2 + "}";
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