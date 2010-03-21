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
using System.Xml;
using Wayloop.Highlight.Collections;
using Wayloop.Highlight.Extensions;
using Wayloop.Highlight.Patterns;


namespace Wayloop.Highlight.Configuration
{
    public class XmlConfiguration : IConfiguration
    {
        private DefinitionCollection definitions;


        public XmlDocument XmlDocument { get; set; }


        public XmlConfiguration(XmlDocument xmlDocument)
        {
            if (xmlDocument == null) {
                throw new ArgumentNullException("xmlDocument");
            }

            XmlDocument = xmlDocument;
        }


        public DefinitionCollection GetDefinitions()
        {
            if (definitions == null) {
                var definitionNodes = XmlDocument.SelectNodes("definitions/definition");
                if (definitionNodes != null) {
                    definitions = new DefinitionCollection();
                    definitions.AddRange(from XmlNode node in definitionNodes select GetDefinition(node));
                }
            }

            return definitions;
        }


        private Definition GetDefinition(XmlNode definitionNode)
        {
            var name = definitionNode.GetAttributeValue("name");
            var patterns = GetPatterns(definitionNode);
            var caseSensitive = Boolean.Parse(definitionNode.GetAttributeValue("caseSensitive"));
            var style = GetDefinitionStyle(definitionNode);

            return new Definition(name, caseSensitive, style, patterns);
        }


        private PatternCollection GetPatterns(XmlNode definitionNode)
        {
            var patterns = new PatternCollection();
            var patternNodes = definitionNode.SelectNodes("pattern");
            if (patternNodes != null) {
                patterns.AddRange(from XmlNode node in patternNodes select GetPattern(node));
            }

            return patterns;
        }


        private Pattern GetPattern(XmlNode patternNode)
        {
            const StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
            var patternType = patternNode.GetAttributeValue("type");
            if (patternType.Equals("block", stringComparison)) {
                var name = patternNode.GetAttributeValue("name");
                var style = GetPatternStyle(patternNode);
                var beginsWith = patternNode.GetAttributeValue("beginsWith");
                var endsWith = patternNode.GetAttributeValue("endsWith");
                var escapesWith = patternNode.GetAttributeValue("escapesWith");

                return new BlockPattern(name, style, beginsWith, endsWith, escapesWith);
            }
            if (patternType.Equals("markup", stringComparison)) {
                var name = patternNode.GetAttributeValue("name");
                var style = GetMarkupPatternStyle(patternNode);
                var highlightAttributes = Boolean.Parse(patternNode.GetAttributeValue("highlightAttributes"));

                return new MarkupPattern(name, style, highlightAttributes);
            }
            if (patternType.Equals("word", stringComparison)) {
                var name = patternNode.GetAttributeValue("name");
                var style = GetPatternStyle(patternNode);
                var words = GetPatternWords(patternNode);

                return new WordPattern(name, style, words);
            }

            throw new InvalidOperationException(String.Format("Unknown pattern type: {0}", patternType));
        }


        private IEnumerable<string> GetPatternWords(XmlNode patternNode)
        {
            var words = new List<string>();
            var wordNodes = patternNode.SelectNodes("word");
            if (wordNodes != null) {
                words.AddRange(from XmlNode wordNode in wordNodes select Regex.Escape(wordNode.InnerText));
            }

            return words;
        }


        private PatternStyle GetPatternStyle(XmlNode patternNode)
        {
            var fontNode = patternNode.SelectSingleNode("font");
            var colors = GetPatternColors(fontNode);
            var font = GetPatternFont(fontNode);

            return new PatternStyle(colors, font);
        }


        private ColorPair GetPatternColors(XmlNode fontNode)
        {
            var foreColor = Color.FromName(fontNode.GetAttributeValue("foreColor"));
            var backColor = Color.FromName(fontNode.GetAttributeValue("backColor"));

            return new ColorPair(foreColor, backColor);
        }


        private Font GetPatternFont(XmlNode fontNode)
        {
            var fontFamily = fontNode.GetAttributeValue("name");
            if (fontFamily != null) {
                var emSize = fontNode.GetAttributeValue("size").ToSingle(11f);
                var style = Enum<FontStyle>.Parse(fontNode.GetAttributeValue("style"), FontStyle.Regular, true);

                return new Font(fontFamily, emSize, style);
            }

            return null;
        }


        private MarkupPatternStyle GetMarkupPatternStyle(XmlNode patternNode)
        {
            var patternStyle = GetPatternStyle(patternNode);
            var fontNode = patternNode.SelectSingleNode("font");
            var bracketColors = GetMarkupPatternBracketColors(fontNode);
            var attributeNameColors = GetMarkupPatternAttributeNameColors(fontNode);
            var attributeValueColors = GetMarkupPatternAttributeValueColors(fontNode);

            return new MarkupPatternStyle(patternStyle.Colors, patternStyle.Font, bracketColors, attributeNameColors, attributeValueColors);
        }


        private ColorPair GetMarkupPatternBracketColors(XmlNode fontNode)
        {
            const string xpath = "bracketStyle";
            return GetMarkupPatternColors(fontNode, xpath);
        }


        private ColorPair GetMarkupPatternAttributeNameColors(XmlNode fontNode)
        {
            const string xpath = "attributeNameStyle";
            return GetMarkupPatternColors(fontNode, xpath);
        }


        private ColorPair GetMarkupPatternAttributeValueColors(XmlNode fontNode)
        {
            const string xpath = "attributeValueStyle";
            return GetMarkupPatternColors(fontNode, xpath);
        }


        private ColorPair GetMarkupPatternColors(XmlNode fontNode, string xpath)
        {
            var node = fontNode.SelectSingleNode(xpath);
            if (node != null) {
                var foreColor = Color.FromName(node.GetAttributeValue("foreColor"));
                var backColor = Color.FromName(node.GetAttributeValue("backColor"));

                return new ColorPair(foreColor, backColor);
            }

            return new ColorPair();
        }


        private DefinitionStyle GetDefinitionStyle(XmlNode definitionNode)
        {
            const string xpath = "default/font";
            var fontNode = definitionNode.SelectSingleNode(xpath);
            var foreColor = Color.FromName(fontNode.GetAttributeValue("foreColor"));
            var backColor = Color.FromName(fontNode.GetAttributeValue("backColor"));
            var font = GetDefinitionFont(fontNode);

            return new DefinitionStyle(foreColor, backColor, font);
        }


        private Font GetDefinitionFont(XmlNode fontNode)
        {
            var fontName = fontNode.GetAttributeValue("name");
            var fontSize = Convert.ToSingle(fontNode.GetAttributeValue("size"));
            var fontStyle = (FontStyle) Enum.Parse(typeof (FontStyle), fontNode.GetAttributeValue("style"), true);

            return new Font(fontName, fontSize, fontStyle);
        }
    }
}