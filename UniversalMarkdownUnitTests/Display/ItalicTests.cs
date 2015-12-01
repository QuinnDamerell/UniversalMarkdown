using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class ItalicTests : DisplayTestBase
    {
        [UITestMethod]
        [TestCategory("Display - inline")]
        public void Italic_Inline()
        {
            string result = RenderMarkdown("This is *italic* text");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'This is '
                    Span FontStyle: Italic
                        Run FontStyle: Italic, Text: 'italic'
                    Run Text: ' text'"), result);
        }
    }
}
