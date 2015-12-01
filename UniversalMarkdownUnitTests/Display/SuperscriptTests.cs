using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class SuperscriptTests : DisplayTestBase
    {
        [UITestMethod]
        [TestCategory("Display - inline")]
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
        [TestCategory("Display - inline")]
        public void Superscript_Nested()
        {
            string result = RenderMarkdown("Y^a^a");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'Y'
                    Run Text: 'a'
                    Run Text: 'a'"), result);   // TODO
        }
    }
}
