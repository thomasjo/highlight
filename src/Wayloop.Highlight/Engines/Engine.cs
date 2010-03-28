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
using System.Linq;
using System.Text.RegularExpressions;
using Wayloop.Highlight.Patterns;


namespace Wayloop.Highlight.Engines
{
    public abstract class Engine : IEngine
    {
        private const RegexOptions DefaultRegexOptions = RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace;


        public string Highlight(Definition definition, string input)
        {
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }

            var output = PreHighlight(definition, input);
            output = HighlightUsingRegex(definition, output);
            output = PostHighlight(definition, output);

            return output;
        }


        protected virtual string PreHighlight(Definition definition, string input)
        {
            return input;
        }


        protected virtual string PostHighlight(Definition definition, string input)
        {
            return input;
        }


        private string HighlightUsingRegex(Definition definition, string input)
        {
            var regexOptions = GetRegexOptions(definition);
            var evaluator = GetMatchEvaluator(definition);
            var regexPattern = definition.GetRegexPattern();
            var output = Regex.Replace(input, regexPattern, evaluator, regexOptions);

            return output;
        }


        private RegexOptions GetRegexOptions(Definition definition)
        {
            if (definition.CaseSensitive) {
                return DefaultRegexOptions | RegexOptions.IgnoreCase;
            }

            return DefaultRegexOptions;
        }


        private string ElementMatchHandler(Definition definition, Match match)
        {
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }
            if (match == null) {
                throw new ArgumentNullException("match");
            }

            var pattern = definition.Patterns.First(x => match.Groups[x.Key].Success).Value;
            if (pattern != null) {
                if (pattern is BlockPattern) {
                    return ProcessBlockPatternMatch(definition, (BlockPattern) pattern, match);
                }
                if (pattern is MarkupPattern) {
                    return ProcessMarkupPatternMatch(definition, (MarkupPattern) pattern, match);
                }
                if (pattern is WordPattern) {
                    return ProcessWordPatternMatch(definition, (WordPattern) pattern, match);
                }
            }

            return match.Value;
        }


        private MatchEvaluator GetMatchEvaluator(Definition definition)
        {
            return match => ElementMatchHandler(definition, match);
        }


        protected abstract string ProcessBlockPatternMatch(Definition definition, BlockPattern pattern, Match match);
        protected abstract string ProcessMarkupPatternMatch(Definition definition, MarkupPattern pattern, Match match);
        protected abstract string ProcessWordPatternMatch(Definition definition, WordPattern pattern, Match match);
    }
}