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


namespace Wayloop.Highlight.Patterns
{
    public abstract class Pattern
    {
        protected Pattern()
        {
            Name = string.Empty;
            PatternString = string.Empty;
            PatternType = PatternType.Word;
            Font = new Font("Courier New", 10f, FontStyle.Regular);
            ForeColor = Color.Empty;
            BackColor = Color.Empty;
        }


        protected Pattern(XmlNode patternNode)
        {
            if (patternNode == null) {
                throw new ArgumentNullException("patternNode");
            }

            string familyName = null;
            string innerText = null;
            string str3 = null;
            string name = null;
            string str5 = null;
            Name = patternNode.Attributes["name"].InnerText;
            PatternString = string.Empty;
            PatternType = GetPatternType(patternNode.Attributes["type"].InnerText);
            var node = patternNode.SelectSingleNode("font");
            if (node != null) {
                if (node.Attributes["name"] != null) {
                    familyName = node.Attributes["name"].InnerText;
                }
                if (node.Attributes["size"] != null) {
                    innerText = node.Attributes["size"].InnerText;
                }
                if (node.Attributes["style"] != null) {
                    str3 = node.Attributes["style"].InnerText;
                }
                if (familyName != null) {
                    var emSize = (innerText != null) ? Convert.ToSingle(innerText) : 11f;
                    var style = (str3 != null) ? ((FontStyle) Enum.Parse(typeof (FontStyle), str3, true)) : FontStyle.Regular;
                    Font = new Font(familyName, emSize, style);
                }
                else {
                    Font = null;
                }
            }
            else {
                Font = null;
            }
            if (node != null) {
                if (node.Attributes["foreColor"] != null) {
                    name = node.Attributes["foreColor"].InnerText;
                }
                if (node.Attributes["backColor"] != null) {
                    str5 = node.Attributes["backColor"].InnerText;
                }
            }
            if (name != null) {
                ForeColor = Color.FromName(name);
            }
            if (str5 != null) {
                BackColor = Color.FromName(str5);
            }
        }


        public Color BackColor { get; private set; }
        public Font Font { get; private set; }
        public Color ForeColor { get; private set; }
        public string Name { get; private set; }
        public virtual string PatternString { get; set; }
        public PatternType PatternType { get; private set; }


        public static PatternType GetPatternType(string input)
        {
            try {
                return (PatternType) Enum.Parse(typeof (PatternType), input, true);
            }
            catch {
                return PatternType.Word;
            }
        }


        public override string ToString()
        {
            return Name;
        }
    }
}