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
        public string Highlight(Definition definition, string input)
        {
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }

            const RegexOptions regexOptions = RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace;
            var evaluator = GetMatchEvaluator(definition);
            var patterns = definition.GetPatterns();

            var output = PreHighlight(definition, input);
            if (definition.CaseSensitive) {
                output = Regex.Replace(output, patterns, evaluator, regexOptions);
            }
            else {
                output = Regex.Replace(output, patterns, evaluator, regexOptions | RegexOptions.IgnoreCase);
            }
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


        protected string ElementMatchHandler(Definition definition, Match match)
        {
            if (definition == null) {
                throw new ArgumentNullException("definition");
            }
            if (match == null) {
                throw new ArgumentNullException("match");
            }
            
            var pattern = definition.Patterns.SingleOrDefault(x => match.Groups[x.Name].Success);
            if (pattern != null) {
                if (pattern is MarkupPattern) {
                    return HandleMarkupPattern(definition, pattern, match);
                }

                return HandlePattern(definition, pattern, match);
            }

            return match.Value;
        }


        protected abstract string HandlePattern(Definition definition, Pattern pattern, Match match);
        protected abstract string HandleMarkupPattern(Definition definition, Pattern pattern, Match match);


        private MatchEvaluator GetMatchEvaluator(Definition definition)
        {
            return match => ElementMatchHandler(definition, match);
        }
    }
}