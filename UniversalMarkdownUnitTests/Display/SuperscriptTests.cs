using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class SuperscriptTests : DisplayTestBase
    {
        [UITestMethod]
        public void Superscript_Simple()
        {
            string result = RenderMarkdown("Using the carot sign ^will create exponentials");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'Using the carot sign '
                    Run Text: 'will'
                    Run Text: ' create exponentials'"), result);    // TODO
        }

        [UITestMethod]
        public void Superscript_Nested()
        {
            string result = RenderMarkdown("Y^a^a");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'Y'
                    Run Text: 'a'
                    Run Text: 'a'"), result);   // TODO
        }

        [UITestMethod]
        public void Superscript_WithParentheses()
        {
            // The text to superscript can be enclosed in brackets.
            string result = RenderMarkdown("This is a sentence^(This is a note in superscript).");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'This is a sentence'
                    Run Text: 'This is a note in superscript'
                    Run Text: '.'"), result);   // TODO
        }

        [UITestMethod]
        public void Superscript_Negative()
        {
            // Does nothing.
            string result = RenderMarkdown("Using the carot sign ^ incorrectly");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'Using the carot sign ^ incorrectly'"), result);
        }
    }
}
