using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class ParagraphTests : DisplayTestBase
    {
        [UITestMethod]
        [TestCategory("Display - block")]
        public void Paragraph_NewParagraph()
        {
            // An empty line starts a new paragraph.
            string result = RenderMarkdown(CollapseWhitespace(@"
                line 1

                line 2"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'line 1'
                Paragraph
                    Run Text: 'line 2'"), result);
        }
    }
}
