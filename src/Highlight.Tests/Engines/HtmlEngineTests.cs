using Highlight.Configuration;
using Highlight.Engines;
using Highlight.Tests.Engines.Resources;
using NUnit.Framework;

namespace Highlight.Tests.Engines
{
    [TestFixture]
    public class HtmlEngineTests
    {
        private IEngine engine;
        private IConfiguration configuration;

        [SetUp]
        public void FixtureSetUp()
        {
            engine = new HtmlEngine();
            configuration = new DefaultConfiguration();
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