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
using System.Drawing;
using System.Xml;
using Wayloop.Highlight.Extensions;


namespace Wayloop.Highlight.Patterns
{
    public class PatternStyle
    {
        public PatternStyle()
        {
            Font = new Font("Courier New", 11f, FontStyle.Regular);
            ForeColor = Color.Empty;
            BackColor = Color.Empty;
        }


        public PatternStyle(XmlNode patternNode)
        {
            if (patternNode == null) {
                throw new ArgumentNullException("patternNode");
            }

            var fontNode = patternNode.SelectSingleNode("font");
            if (fontNode == null) {
                return;
            }

            var fontFamilyValue = fontNode.GetAttributeValue("name");
            if (fontFamilyValue != null) {
                var fontSizeValue = fontNode.GetAttributeValue("size");
                var fontStyleValue = fontNode.GetAttributeValue("style");
                var emSize = (fontSizeValue != null) ? Convert.ToSingle(fontSizeValue) : 11f;
                var style = (fontStyleValue != null) ? ((FontStyle) Enum.Parse(typeof (FontStyle), fontStyleValue, true)) : FontStyle.Regular;
                Font = new Font(fontFamilyValue, emSize, style);
            }

            var foreColorValue = fontNode.GetAttributeValue("foreColor");
            if (foreColorValue != null) {
                ForeColor = Color.FromName(foreColorValue);
            }
            var backColorValue = fontNode.GetAttributeValue("backColor");
            if (backColorValue != null) {
                BackColor = Color.FromName(backColorValue);
            }
        }


        public PatternStyle(Color foreColor, Color backColor, Font font)
        {
            ForeColor = foreColor;
            BackColor = backColor;
            Font = font;
        }


        public Color ForeColor { get; private set; }
        public Color BackColor { get; private set; }
        public Font Font { get; private set; }
    }
}