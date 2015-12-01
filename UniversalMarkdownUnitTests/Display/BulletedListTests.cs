using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class BulletedListTests : DisplayTestBase
    {
        [UITestMethod]
        [TestCategory("Display - block")]
        public void BulletedList_Alt()
        {
            string result = RenderMarkdown(CollapseWhitespace(@"
                before

                - List item 1
                * List item 2
                + List item 3

                after"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'
                    Run Text: 'List item 1'
                    Run Text: 'List item 2'
                    Run Text: 'List item 3'
                    Run Text: 'after'"), result);   // TODO
        }
    }
}
