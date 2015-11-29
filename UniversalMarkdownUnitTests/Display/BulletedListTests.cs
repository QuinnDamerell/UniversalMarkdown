using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class BulletedListTests : DisplayTestBase
    {
        [UITestMethod]
        public void BulletedList_SingleLine()
        {
            string result = RenderMarkdown("- List");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'List'"), result);   // TODO
        }

        [UITestMethod]
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

        [UITestMethod]
        public void BulletedList_Nested()
        {
            string result = RenderMarkdown(CollapseWhitespace(@"
                - List item 1
                    - List item 1, line 2
                + List item 3"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'
                    Run Text: 'List item 1'
                    Run Text: 'List item 2'
                    Run Text: 'List item 3'
                    Run Text: 'after'"), result);   // TODO
        }

        [UITestMethod]
        public void BulletedList_Negative_SpaceRequired()
        {
            // The space is required.
            string result = RenderMarkdown("-List");
            Assert.AreEqual(CollapseWhitespace(@"
                        Paragraph
                            Run Text: '-List'"), result);
        }

        [UITestMethod]
        public void BulletedList_Negative_NewParagraph()
        {
            // Bulleted lists must start on a new paragraph
            string result = RenderMarkdown(CollapseWhitespace(@"
                before
                * List
                after"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before * List after'"), result);
        }
    }
}
