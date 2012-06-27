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
using System.Drawing;
using System.Text;
using Highlight.Patterns;


namespace Highlight.Engines
{
    internal static class HtmlEngineHelper
    {
        public static string CreateCssClassName(string definition, string pattern)
        {
            var cssClassName = definition
                .Replace("#", "sharp")
                .Replace("+", "plus")
                .Replace(".", "dot")
                .Replace("-", "");

            return String.Concat(cssClassName, pattern);
        }


        public static string CreatePatternStyle(Style style)
        {
            return CreatePatternStyle(style.Colors, style.Font);
        }


        public static string CreatePatternStyle(ColorPair colors, Font font)
        {
            var patternStyle = new StringBuilder();
            if (colors != null) {
                if (colors.ForeColor != Color.Empty) {
                    patternStyle.Append("color: " + colors.ForeColor.Name + ";");
                }
                if (colors.BackColor != Color.Empty) {
                    patternStyle.Append("background-color: " + colors.BackColor.Name + ";");
                }
            }

            if (font != null) {
                if (font.Name != null) {
                    patternStyle.Append("font-family: " + font.Name + ";");
                }
                if (font.Size > 0f) {
                    patternStyle.Append("font-size: " + font.Size + "px;");
                }
                if (font.Style == FontStyle.Regular) {
                    patternStyle.Append("font-weight: normal;");
                }
                if (font.Style == FontStyle.Bold) {
                    patternStyle.Append("font-weight: bold;");
                }
            }

            return patternStyle.ToString();
        }
    }
}