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


using System.Drawing;
using System.Xml;
using Wayloop.Highlight.Extensions;


namespace Wayloop.Highlight.Patterns
{
    public class MarkupPattern : Pattern
    {
        public MarkupPattern()
        {
            HighlightAttributes = true;
        }


        public MarkupPattern(XmlNode patternNode) : base(patternNode)
        {
            HighlightAttributes = bool.Parse(patternNode.GetAttributeValue("highlightAttributes"));
            var node = patternNode.SelectSingleNode("font/bracketStyle");
            BracketForeColor = Color.FromName(node.GetAttributeValue("foreColor"));
            BracketBackColor = Color.FromName(node.GetAttributeValue("backColor"));
            if (!HighlightAttributes) {
                return;
            }

            var node2 = patternNode.SelectSingleNode("font/attributeNameStyle");
            AttributeNameForeColor = Color.FromName(node2.GetAttributeValue("foreColor"));
            AttributeNameBackColor = Color.FromName(node2.GetAttributeValue("backColor"));

            var node3 = patternNode.SelectSingleNode("font/attributeValueStyle");
            AttributeValueForeColor = Color.FromName(node3.GetAttributeValue("foreColor"));
            AttributeValueBackColor = Color.FromName(node3.GetAttributeValue("backColor"));
        }


        public Color AttributeNameBackColor { get; private set; }
        public Color AttributeNameForeColor { get; private set; }
        public Color AttributeValueBackColor { get; private set; }
        public Color AttributeValueForeColor { get; private set; }
        public Color BracketBackColor { get; private set; }
        public Color BracketForeColor { get; private set; }
        public bool HighlightAttributes { get; private set; }


        public override string GetPatternString()
        {
            return @"
                (?'openTag'&lt;\??/?)
                (?'ws1'\s*?)
                (?'tagName'[\w\:]+)
                (?>
                    (?!=[\/\?]?&gt;)
                    (?'ws2'\s*?)
                    (?'attribute'(?'attribName'[\w\:-]+)(?:(?'ws3'\s*)(?'attribSign'=)(?'ws4'\s*))(?'attribValue'(?:\'[^\']*\'|""[^""]*""|\w+)))
                )*
                (?'ws5'\s*?)
                (?'closeTag'[\/\?]?&gt;)
            ";
        }
    }
}