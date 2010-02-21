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


using System.Xml;
using Wayloop.Highlight.Engines;


namespace Wayloop.Highlight
{
    public class Highlighter
    {
        private XmlDocument xmlConfiguration;


        public Highlighter()
        {
            xmlConfiguration = new XmlDocument();
            Engine = new HtmlEngine();
        }


        public Highlighter(XmlDocument xmlConfiguration) : this()
        {
            this.xmlConfiguration = xmlConfiguration;
        }


        public string ConfigurationFile { get; set; }
        public Engine Engine { get; set; }


        public string Highlight(string definitionName, string input)
        {
            if (xmlConfiguration.OuterXml.Length == 0) {
                xmlConfiguration = Global.GetConfiguration(ConfigurationFile);
            }
            var definition = new Definition(xmlConfiguration, definitionName);
            if (definition.Name != string.Empty) {
                return Engine.Highlight(definition, input);
            }

            return input;
        }
    }
}