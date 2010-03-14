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
using System.Linq;
using System.Xml;
using Wayloop.Highlight.Collections;
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
                    definitions.AddRange(
                        from XmlNode node in definitionNodes
                        select GetDefinition(node)
                        );
                }
            }

            return definitions;
        }


        private Definition GetDefinition(XmlNode definitionNode)
        {
            var name = definitionNode.Attributes["name"].InnerText;
            var patterns = GetPatterns(definitionNode);
            var caseSensitive = Boolean.Parse(definitionNode.Attributes["caseSensitive"].InnerText);
            var style = GetDefinitionStyle(definitionNode);

            return new Definition(name, caseSensitive, style, patterns);
        }


        private PatternCollection GetPatterns(XmlNode definitionNode)
        {
            var patterns = new PatternCollection();
            var patternNodes = definitionNode.SelectNodes("pattern");
            if (patternNodes != null) {
                foreach (XmlNode node in patternNodes) {
                    Pattern pattern;
                    var innerText = node.Attributes["type"].InnerText;
                    if (innerText.Equals("block")) {
                        pattern = new BlockPattern(node);
                    }
                    else if (innerText.Equals("markup")) {
                        pattern = new MarkupPattern(node);
                    }
                    else {
                        pattern = new WordPattern(node);
                    }
                    patterns.Add(pattern);
                }
            }

            return patterns;
        }


        private DefinitionStyle GetDefinitionStyle(XmlNode definitionNode)
        {
            const string xpath = "default/font";
            var fontNode = definitionNode.SelectSingleNode(xpath);
            var foreColor = Color.FromName(fontNode.Attributes["foreColor"].InnerText);
            var backColor = Color.FromName(fontNode.Attributes["backColor"].InnerText);
            var font = GetDefinitionFont(fontNode);

            return new DefinitionStyle(foreColor, backColor, font);
        }


        private Font GetDefinitionFont(XmlNode fontNode)
        {
            var fontName = fontNode.Attributes["name"].InnerText;
            var fontSize = Convert.ToSingle(fontNode.Attributes["size"].InnerText);
            var fontStyle = (FontStyle) Enum.Parse(typeof (FontStyle), fontNode.Attributes["style"].InnerText, true);

            return new Font(fontName, fontSize, fontStyle);
        }
    }
}