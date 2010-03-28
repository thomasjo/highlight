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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Wayloop.Highlight.Extensions;
using Wayloop.Highlight.Patterns;


namespace Wayloop.Highlight.Configuration
{
    public class XmlConfiguration : IConfiguration
    {
        private IDictionary<string, Definition> definitions;


        public IDictionary<string, Definition> Definitions
        {
            get { return GetDefinitions(); }
        }


        public XDocument XmlDocument { get; set; }


        public XmlConfiguration(XDocument xmlDocument)
        {
            if (xmlDocument == null) {
                throw new ArgumentNullException("xmlDocument");
            }

            XmlDocument = xmlDocument;
        }


        private IDictionary<string, Definition> GetDefinitions()
        {
            if (definitions == null) {
                definitions = XmlDocument
                    .Descendants("definition")
                    .Select(GetDefinition)
                    .ToDictionary(x => x.Name);
            }

            return definitions;
        }


        private Definition GetDefinition(XElement definitionElement)
        {
            var name = definitionElement.GetAttributeValue("name");
            var patterns = GetPatterns(definitionElement);
            var caseSensitive = Boolean.Parse(definitionElement.GetAttributeValue("caseSensitive"));
            var style = GetDefinitionStyle(definitionElement);

            return new Definition(name, caseSensitive, style, patterns);
        }


        private IDictionary<string, Pattern> GetPatterns(XContainer definitionElement)
        {
            var patterns = definitionElement
                .Descendants("pattern")
                .Select(GetPattern)
                .ToDictionary(x => x.Name);

            return patterns;
        }


        private Pattern GetPattern(XElement patternElement)
        {
            const StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
            var patternType = patternElement.GetAttributeValue("type");
            if (patternType.Equals("block", stringComparison)) {
                return GetBlockPattern(patternElement);
            }
            if (patternType.Equals("markup", stringComparison)) {
                return GetMarkupPattern(patternElement);
            }
            if (patternType.Equals("word", stringComparison)) {
                return GetWordPattern(patternElement);
            }

            throw new InvalidOperationException(String.Format("Unknown pattern type: {0}", patternType));
        }


        private Pattern GetBlockPattern(XElement patternElement)
        {
            var name = patternElement.GetAttributeValue("name");
            var style = GetPatternStyle(patternElement);
            var beginsWith = patternElement.GetAttributeValue("beginsWith");
            var endsWith = patternElement.GetAttributeValue("endsWith");
            var escapesWith = patternElement.GetAttributeValue("escapesWith");

            return new BlockPattern(name, style, beginsWith, endsWith, escapesWith);
        }


        private Pattern GetMarkupPattern(XElement patternElement)
        {
            var name = patternElement.GetAttributeValue("name");
            var style = GetMarkupPatternStyle(patternElement);
            var highlightAttributes = Boolean.Parse(patternElement.GetAttributeValue("highlightAttributes"));

            return new MarkupPattern(name, style, highlightAttributes);
        }


        private Pattern GetWordPattern(XElement patternElement)
        {
            var name = patternElement.GetAttributeValue("name");
            var style = GetPatternStyle(patternElement);
            var words = GetPatternWords(patternElement);

            return new WordPattern(name, style, words);
        }


        private IEnumerable<string> GetPatternWords(XContainer patternElement)
        {
            var words = new List<string>();
            var wordElements = patternElement.Descendants("word");
            if (wordElements != null) {
                words.AddRange(from wordElement in wordElements select Regex.Escape(wordElement.Value));
            }

            return words;
        }


        private PatternStyle GetPatternStyle(XContainer patternElement)
        {
            var fontElement = patternElement.Descendants("font").Single();
            var colors = GetPatternColors(fontElement);
            var font = GetPatternFont(fontElement);

            return new PatternStyle(colors, font);
        }


        private ColorPair GetPatternColors(XElement fontElement)
        {
            var foreColor = Color.FromName(fontElement.GetAttributeValue("foreColor"));
            var backColor = Color.FromName(fontElement.GetAttributeValue("backColor"));

            return new ColorPair(foreColor, backColor);
        }


        private Font GetPatternFont(XElement fontElement)
        {
            var fontFamily = fontElement.GetAttributeValue("name");
            if (fontFamily != null) {
                var emSize = fontElement.GetAttributeValue("size").ToSingle(11f);
                var style = Enum<FontStyle>.Parse(fontElement.GetAttributeValue("style"), FontStyle.Regular, true);

                return new Font(fontFamily, emSize, style);
            }

            return null;
        }


        private MarkupPatternStyle GetMarkupPatternStyle(XContainer patternElement)
        {
            var patternStyle = GetPatternStyle(patternElement);
            var fontElement = patternElement.Descendants("font").Single();
            var bracketColors = GetMarkupPatternBracketColors(fontElement);
            var attributeNameColors = GetMarkupPatternAttributeNameColors(fontElement);
            var attributeValueColors = GetMarkupPatternAttributeValueColors(fontElement);

            return new MarkupPatternStyle(patternStyle.Colors, patternStyle.Font, bracketColors, attributeNameColors, attributeValueColors);
        }


        private ColorPair GetMarkupPatternBracketColors(XContainer fontElement)
        {
            const string descendantName = "bracketStyle";
            return GetMarkupPatternColors(fontElement, descendantName);
        }


        private ColorPair GetMarkupPatternAttributeNameColors(XContainer fontElement)
        {
            const string descendantName = "attributeNameStyle";
            return GetMarkupPatternColors(fontElement, descendantName);
        }


        private ColorPair GetMarkupPatternAttributeValueColors(XContainer fontElement)
        {
            const string descendantName = "attributeValueStyle";
            return GetMarkupPatternColors(fontElement, descendantName);
        }


        private ColorPair GetMarkupPatternColors(XContainer fontElement, XName descendantName)
        {
            var element = fontElement.Descendants(descendantName).SingleOrDefault();
            if (element != null) {
                var foreColor = Color.FromName(element.GetAttributeValue("foreColor"));
                var backColor = Color.FromName(element.GetAttributeValue("backColor"));

                return new ColorPair(foreColor, backColor);
            }

            return new ColorPair();
        }


        private DefinitionStyle GetDefinitionStyle(XNode definitionElement)
        {
            const string xpath = "default/font";
            var fontElement = definitionElement.XPathSelectElement(xpath);
            var colors = GetDefinitionColors(fontElement);
            var font = GetDefinitionFont(fontElement);

            return new DefinitionStyle(colors, font);
        }


        private ColorPair GetDefinitionColors(XElement fontElement)
        {
            var foreColor = Color.FromName(fontElement.GetAttributeValue("foreColor"));
            var backColor = Color.FromName(fontElement.GetAttributeValue("backColor"));

            return new ColorPair(foreColor, backColor);
        }


        private Font GetDefinitionFont(XElement fontElement)
        {
            var fontName = fontElement.GetAttributeValue("name");
            var fontSize = Convert.ToSingle(fontElement.GetAttributeValue("size"));
            var fontStyle = (FontStyle) Enum.Parse(typeof (FontStyle), fontElement.GetAttributeValue("style"), true);

            return new Font(fontName, fontSize, fontStyle);
        }
    }
}