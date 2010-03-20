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
using System.Xml;
using Wayloop.Highlight.Extensions;


namespace Wayloop.Highlight.Patterns
{
    //public class MarkupPattern : Pattern<MarkupPatternStyle>
    public class MarkupPattern : Pattern
    {
        public MarkupPattern(XmlNode patternNode) : base(patternNode)
        {
            if (patternNode == null) {
                throw new ArgumentNullException("patternNode");
            }

            HighlightAttributes = bool.Parse(patternNode.GetAttributeValue("highlightAttributes"));
            Style = new MarkupPatternStyle(patternNode);
        }


        public new MarkupPatternStyle Style
        {
            get { return (MarkupPatternStyle) base.Style; }
            protected set { base.Style = value; }
        }


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