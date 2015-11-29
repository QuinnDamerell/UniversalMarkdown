using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class BoldTests : DisplayTestBase
    {
        [UITestMethod]
        public void Bold_Simple()
        {
            var result = RenderMarkdown("**bold**");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Span FontWeight: 700
                        Run FontWeight: 700, Text: 'bold'"), result);
        }

        [UITestMethod]
        public void Bold_Simple_Alt()
        {
            string result = RenderMarkdown("__bold__");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Span FontWeight: 700
                        Run FontWeight: 700, Text: 'bold'"), result);
        }

        [UITestMethod]
        public void Bold_Inline()
        {
            string result = RenderMarkdown("This is **bold** text");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'This is '
                    Span FontWeight: 700
                        Run FontWeight: 700, Text: 'bold'
                    Run Text: ' text'"), result);
        }

        [UITestMethod]
        public void Bold_Inline_Alt()
        {
            string result = RenderMarkdown("This is __bold__ text");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'This is '
                    Span FontWeight: 700
                        Run FontWeight: 700, Text: 'bold'
                    Run Text: ' text'"), result);
        }

        [UITestMethod]
        public void Bold_Inside_Word()
        {
            string result = RenderMarkdown("before**middle**end");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'
                    Span FontWeight: 700
                        Run FontWeight: 700, Text: 'middle'
                    Run Text: 'end'"), result);
        }

        [UITestMethod]
        public void Bold_Negative_1()
        {
            string result = RenderMarkdown("before** middle **end");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before** middle **end'"), result);
        }

        [UITestMethod]
        public void Bold_Negative_2()
        {
            string result = RenderMarkdown("before** middle**end");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before** middle **end'"), result);
        }
    }
}
