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


using System.Xml.Linq;
using NUnit.Framework;
using Wayloop.Highlight.Configuration;
using Wayloop.Highlight.Engines;
using Wayloop.Highlight.Tests.Engines.Resources;


namespace Wayloop.Highlight.Tests.Engines
{
    [TestFixture]
    public class HtmlEngineTests
    {
        private IEngine engine;
        private IConfiguration configuration;


        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            engine = new HtmlEngine();
            configuration = new XmlConfiguration(XDocument.Load(("Definitions.xml")));
        }


        [Test]
        public void Highlight_CsharpDefinitionAndCsharpInput_ReturnsExpectedOutput()
        {
            // Arrange
            var definition = configuration.Definitions["C#"];
            var input = InputOutput.CSharp_Sample1;
            var expectedOutout = InputOutput.CSharp_Sample1_HtmlOutput;

            // Act
            var output = engine.Highlight(definition, input);

            // Assert
            Assert.That(output, Is.EqualTo(expectedOutout));
        }


        [Test]
        public void Highlight_HtmlDefinitionAndXhtmlInput_ReturnsExpectedOutput()
        {
            // Arrange
            var definition = configuration.Definitions["HTML"];
            var input = InputOutput.Html_Sample1;
            var expectedOutput = InputOutput.Html_Sample1_HtmlOutput;

            // Act
            var output = engine.Highlight(definition, input);

            // Assert
            Assert.That(output, Is.EqualTo(expectedOutput));
        }


        [Test]
        public void Highlight_HtmlDefinitionAndHtmlInput_ReturnsExpectedOutput()
        {
            // Arrange
            var definition = configuration.Definitions["HTML"];
            var input = InputOutput.Html_Sample2;
            var expectedOutput = InputOutput.Html_Sample2_HtmlOutput;

            // Act
            var output = engine.Highlight(definition, input);

            // Assert
            Assert.That(output, Is.EqualTo(expectedOutput));
        }
    }
}