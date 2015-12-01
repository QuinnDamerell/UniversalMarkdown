using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class HyperlinkTests : DisplayTestBase
    {
        [UITestMethod]
        [TestCategory("Display - inline")]
        public void Hyperlink_Http()
        {
            string result = RenderMarkdown("http://reddit.com");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Hyperlink NavigateUri: null
                        SolidColorBrush
                            Color A: 255, B: 41, G: 135, R: 204
                            MatrixTransform
                            MatrixTransform
                        Run Text: 'http://reddit.com'
                            SolidColorBrush
                                Color A: 255, B: 41, G: 135, R: 204
                                MatrixTransform
                                MatrixTransform"), result);
        }
    }
}
