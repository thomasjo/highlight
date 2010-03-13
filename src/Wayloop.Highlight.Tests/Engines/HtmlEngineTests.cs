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
using System.Linq;
using NUnit.Framework;
using Wayloop.Highlight.Engines;


namespace Wayloop.Highlight.Tests.Engines
{
    [TestFixture]
    public class HtmlEngineTests
    {
        [Test]
        public void Highlight_CsharpDefinitionAndCsharpInput_ReturnsExpectedOutput()
        {
            // Arrange
            var engine = new HtmlEngine();
            var definition = new Configurator { ConfigurationFile = Global.ConfigurationFile }.Definitions.Single(x => x.Name.Equals("C#", StringComparison.InvariantCultureIgnoreCase));
            const string input = @"public class A
{
    // Comment
    public void Test()
    {
        return;
    }
}";
            const string expectedOutout = @"<span style=""color: Black;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;""><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">public</span> <span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">class</span> A
{
    <span style=""color: Green;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">// Comment</span>
    <span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">public</span> <span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">void</span> Test()
    {
        <span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">return</span>;
    }
}</span>";

            // Act
            var output = engine.Highlight(definition, input);

            // Assert
            Assert.That(output, Is.EqualTo(expectedOutout));
        }


        [Test]
        public void Highlight_HtmlDefinitionAndHtmlInput_ReturnsExpectedOutput()
        {
            // Arrange
            var engine = new HtmlEngine();
            var definition = new Configurator { ConfigurationFile = Global.ConfigurationFile }.Definitions.Single(x => x.Name.Equals("HTML", StringComparison.InvariantCultureIgnoreCase));
            const string input = @"<script type=""text/javascript"" src=""http://example.org/file.js""></script>
<div id=""test""><a href=""#"">Link</a></div>";
            const string expectedOutput = @"<span style=""color: Black;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;""><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">&lt;</span><span style=""color: Maroon;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">script</span> <span style=""color: Red;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">type</span><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">=""text/javascript""</span> <span style=""color: Red;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">src</span><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">=""http://example.org/file.js""</span><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">&gt;</span><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">&lt;/</span><span style=""color: Maroon;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">script</span><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">&gt;</span>
<span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">&lt;</span><span style=""color: Maroon;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">div</span> <span style=""color: Red;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">id</span><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">=""test""</span><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">&gt;</span><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">&lt;</span><span style=""color: Maroon;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">a</span> <span style=""color: Red;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">href</span><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">=""#""</span><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">&gt;</span>Link<span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">&lt;/</span><span style=""color: Maroon;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">a</span><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">&gt;</span><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">&lt;/</span><span style=""color: Maroon;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">div</span><span style=""color: Blue;background-color: Transparent;font-family: Courier New;font-size: 11px;font-weight: normal;"">&gt;</span></span>";

            // Act
            var output = engine.Highlight(definition, input);

            // Assert
            Assert.That(output, Is.EqualTo(expectedOutput));
        }
    }
}