using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class BoldTests : DisplayTestBase
    {
        [UITestMethod]
        [TestCategory("Display - inline")]
        public void Bold_Simple()
        {
            var result = RenderMarkdown("**bold**");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Span FontWeight: 700
                        Run FontWeight: 700, Text: 'bold'"), result);
        }
    }
}
