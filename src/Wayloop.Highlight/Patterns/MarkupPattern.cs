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


namespace Wayloop.Highlight.Patterns
{
    public sealed class MarkupPattern : Pattern
    {
        public bool HighlightAttributes { get; set; }
        public ColorPair BracketColors { get; set; }
        public ColorPair AttributeNameColors { get; set; }
        public ColorPair AttributeValueColors { get; set; }


        public MarkupPattern(string name, Style style, bool highlightAttributes, ColorPair bracketColors, ColorPair attributeNameColors, ColorPair attributeValueColors) : base(name, style)
        {
            HighlightAttributes = highlightAttributes;
            BracketColors = bracketColors;
            AttributeNameColors = attributeNameColors;
            AttributeValueColors = attributeValueColors;
        }


        public override string GetRegexPattern()
        {
            return @"
                (?'openTag'&lt;\??/?)
                (?'ws1'\s*?)
                (?'tagName'[\w\:]+)
                (?>
                    (?!=[\/\?]?&gt;)
                    (?'ws2'\s*?)
                    (?'attribute'
                        (?'attribName'[\w\:-]+)
                        (?'attribValue'(\s*=\s*(?:&\#39;.*?&\#39;|&quot;.*?&quot;|\w+))?)
                        # (?:(?'ws3'\s*)(?'attribSign'=)(?'ws4'\s*))
                        # (?'attribValue'(?:&\#39;.*?&\#39;|&quot;.*?&quot;|\w+))
                    )
                )*
                (?'ws5'\s*?)
                (?'closeTag'[\/\?]?&gt;)
            ";
        }
    }
}