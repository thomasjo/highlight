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
using System.Text;
using System.Xml;
using Wayloop.Highlight.Collections;
using Wayloop.Highlight.Patterns;


namespace Wayloop.Highlight
{
    public class Definition
    {
        public Definition()
        {
        }


        public Definition(string name, XmlDocument xmlDocument)
        {
            var xpath = String.Format("definitions/definition[translate(@name,'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ')='{0}']", name.ToUpper());
            var definitionNode = xmlDocument.SelectSingleNode(xpath);
            if (definitionNode != null) {
                Name = definitionNode.Attributes["name"].InnerText;
                Patterns = PatternCollection.GetAllPatterns(xmlDocument, this);
                CaseSensitive = bool.Parse(definitionNode.Attributes["caseSensitive"].InnerText);
                SetDefaults(definitionNode);
            }
        }


        public Color BackColor { get; private set; }
        public bool CaseSensitive { get; private set; }
        public Font Font { get; private set; }
        public Color ForeColor { get; private set; }
        public string Name { get; private set; }
        public PatternCollection Patterns { get; private set; }


        public string GetPatterns()
        {
            var allPatterns = new StringBuilder();
            var blockPatterns = new StringBuilder();
            var markupPatterns = new StringBuilder();
            var wordPatterns = new StringBuilder();

            foreach (Pattern pattern in Patterns) {
                if (pattern is BlockPattern) {
                    if (blockPatterns.Length > 1) {
                        blockPatterns.Append("|");
                    }
                    blockPatterns.AppendFormat("(?'{0}'{1})", pattern.Name, pattern.GetPatternString());
                }
                else if (pattern is MarkupPattern) {
                    if (markupPatterns.Length > 1) {
                        markupPatterns.Append("|");
                    }
                    markupPatterns.AppendFormat("(?'{0}'{1})", pattern.Name, pattern.GetPatternString());
                }
                else if (pattern is WordPattern) {
                    if (wordPatterns.Length > 1) {
                        wordPatterns.Append("|");
                    }
                    wordPatterns.AppendFormat("(?'{0}'{1})", pattern.Name, pattern.GetPatternString());
                }
            }
            if (blockPatterns.Length > 0) {
                allPatterns.AppendFormat("(?'blocks'{0})+", blockPatterns);
            }
            if (markupPatterns.Length > 0) {
                allPatterns.AppendFormat("|(?'markup'{0})+", markupPatterns);
            }
            if (wordPatterns.Length > 0) {
                allPatterns.AppendFormat("|(?'words'{0})+", wordPatterns);
            }

            return allPatterns.ToString();
        }


        private void SetDefaults(XmlNode definitionNode)
        {
            const string xpath = "default/font";
            var node = definitionNode.SelectSingleNode(xpath);
            var innerText = node.Attributes["name"].InnerText;
            var emSize = Convert.ToSingle(node.Attributes["size"].InnerText);
            var style = (FontStyle) Enum.Parse(typeof (FontStyle), node.Attributes["style"].InnerText, true);
            Font = new Font(innerText, emSize, style);
            ForeColor = Color.FromName(node.Attributes["foreColor"].InnerText);
            BackColor = Color.FromName(node.Attributes["backColor"].InnerText);
        }


        public override string ToString()
        {
            return Name;
        }
    }
}