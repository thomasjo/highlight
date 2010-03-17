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


namespace Wayloop.Highlight.Patterns
{
    public abstract class Pattern
    {
        private const PatternType DefaultPatternType = PatternType.Word;


        protected Pattern()
        {
            PatternType = DefaultPatternType;
        }


        protected Pattern(XmlNode patternNode)
        {
            if (patternNode == null) {
                throw new ArgumentNullException("patternNode");
            }

            Name = patternNode.Attributes["name"].InnerText;
            PatternType = GetPatternType(patternNode.Attributes["type"].InnerText);
            Style = new PatternStyle(patternNode);
        }


        public string Name { get; private set; }
        public PatternType PatternType { get; private set; }
        public PatternStyle Style { get; private set; }

        public abstract string GetPatternString();


        public static PatternType GetPatternType(string input)
        {
            try {
                return (PatternType) Enum.Parse(typeof (PatternType), input, true);
            }
            catch {
                return DefaultPatternType;
            }
        }


        public override string ToString()
        {
            return Name;
        }
    }
}