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
using System.Text.RegularExpressions;


namespace Wayloop.Highlight.Patterns
{
    public class BlockPattern : Pattern
    {
        public string BeginsWith { get; private set; }
        public string EndsWith { get; private set; }
        public string EscapesWith { get; private set; }


        public BlockPattern(string name, PatternStyle style, string beginsWith, string endsWith, string escapesWith) : base(name, style)
        {
            BeginsWith = beginsWith;
            EndsWith = endsWith;
            EscapesWith = escapesWith;
        }


        public override string GetPatternString()
        {
            if (String.IsNullOrEmpty(EscapesWith)) {
                if (EndsWith.CompareTo(@"\n") == 0) {
                    return String.Format(@"{0}[^\n\r]*", Global.Escape(BeginsWith));
                }

                return String.Format(@"{0}.*?{1}", Global.Escape(BeginsWith), Global.Escape(EndsWith));
            }

            return String.Format("{0}(?>{1}.|[^{2}]|.)*?{3}", new object[] { Regex.Escape(BeginsWith), Regex.Escape(EscapesWith.Substring(0, 1)), Regex.Escape(EndsWith.Substring(0, 1)), Regex.Escape(EndsWith) });
        }
    }
}