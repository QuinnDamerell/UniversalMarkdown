using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class StrikethroughTests : DisplayTestBase
    {
        [UITestMethod]
        public void Strikethrough_Simple()
        {
            var result = RenderMarkdown("~~strike~~");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Span
                        Run Text: 'strike'"), result);
        }

        [UITestMethod]
        public void Strikethrough_Inline()
        {
            string result = RenderMarkdown("This is ~~strike~~ text");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'This is '
                    Span
                        Run Text: 'strike'
                    Run Text: ' text'"), result);
        }
    }
}
