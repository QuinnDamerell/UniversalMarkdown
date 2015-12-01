using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class NumberedListTests : DisplayTestBase
    {
        [UITestMethod]
        [TestCategory("Display - block")]
        public void NumberedList()
        {
            // The numbers are ignored, and they can be any length.
            string result = RenderMarkdown(CollapseWhitespace(@"
                1. List item 1
                2. List item 2"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'
                    Run Text: 'List item 1'
                    Run Text: 'List item 2'
                    Run Text: 'List item 3'
                    Run Text: 'after'"), result);      // TODO
        }
    }
}
