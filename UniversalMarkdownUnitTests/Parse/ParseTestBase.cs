using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Text;
using UniversalMarkdown.Parse;

namespace UniversalMarkdownUnitTests.Parse
{
    /// <summary>
    /// The base class for our display unit tests.
    /// </summary>
    public abstract class ParseTestBase : TestBase
    {
        protected void AssertEqual(string markdown, params MarkdownBlock[] expectedAst)
        {
            var expected = new StringBuilder();
            foreach (var block in expectedAst)
            {
                SerializeElement(expected, block, indentLevel: 0);
            }

            var parser = new MarkdownDocument();
            parser.Parse(markdown);

            var actual = new StringBuilder();
            foreach (var block in parser.Blocks)
            {
                SerializeElement(actual, block, indentLevel: 0);
            }

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }
    }
}
