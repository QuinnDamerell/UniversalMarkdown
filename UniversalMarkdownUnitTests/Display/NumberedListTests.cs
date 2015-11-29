using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class NumberedListTests : DisplayTestBase
    {
        [UITestMethod]
        public void NumberedList_SingleLine()
        {
            string result = RenderMarkdown("1. List");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Text: 'List'"), result);   // TODO
        }

        [UITestMethod]
        public void NumberedList_Numbering()
        {
            // The numbers are ignored, and they can be any length.
            string result = RenderMarkdown(CollapseWhitespace(@"
                7. List item 1
                502. List item 2
                502456456456456456456456456456456456. List item 3"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'
                    Run Text: 'List item 1'
                    Run Text: 'List item 2'
                    Run Text: 'List item 3'
                    Run Text: 'after'"), result);      // TODO
        }

        [UITestMethod]
        public void NumberedList_Negative_SpaceRequired()
        {
            // A space is required after the dot.
            string result = RenderMarkdown("1.List");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Text: '1.List'"), result);   // TODO
        }
    }
}
