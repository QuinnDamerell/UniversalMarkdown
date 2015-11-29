using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class HyperlinkTests : DisplayTestBase
    {
        [UITestMethod]
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

        [UITestMethod]
        public void Hyperlink_Https()
        {
            string result = RenderMarkdown("https://reddit.com");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Hyperlink NavigateUri: null
                        SolidColorBrush
                            Color A: 255, B: 41, G: 135, R: 204
                            MatrixTransform
                            MatrixTransform
                        Run Text: 'https://reddit.com'
                            SolidColorBrush
                                Color A: 255, B: 41, G: 135, R: 204
                                MatrixTransform
                                MatrixTransform"), result);
        }

        [UITestMethod]
        public void Hyperlink_WithLabel()
        {
            string result = RenderMarkdown("[reddit](http://reddit.com)");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Hyperlink NavigateUri: null
                        SolidColorBrush
                            Color A: 255, B: 41, G: 135, R: 204
                            MatrixTransform
                            MatrixTransform
                        Run Text: 'reddit'
                            SolidColorBrush
                                Color A: 255, B: 41, G: 135, R: 204
                                MatrixTransform
                                MatrixTransform"), result);
        }

        [UITestMethod]
        public void Hyperlink_WithTooltip()
        {
            string result = RenderMarkdown(@"[Wikipedia](http://en.wikipedia.org ""tooltip text"")");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Hyperlink NavigateUri: null
                        SolidColorBrush
                            Color A: 255, B: 41, G: 135, R: 204
                            MatrixTransform
                            MatrixTransform
                        Run Text: 'Wikipedia'
                            SolidColorBrush
                                Color A: 255, B: 41, G: 135, R: 204
                                MatrixTransform
                                MatrixTransform"), result);
        }

        [UITestMethod]
        public void Hyperlink_ParsingRule()
        {
            // The link stops at the first ')'
            string result = RenderMarkdown("[test](http://en.wikipedia.org/wiki/Pica_(disorder))");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Hyperlink NavigateUri: null
                        SolidColorBrush
                            Color A: 255, B: 41, G: 135, R: 204
                            MatrixTransform
                            MatrixTransform
                        Run Text: 'test'
                            SolidColorBrush
                                Color A: 255, B: 41, G: 135, R: 204
                                MatrixTransform
                                MatrixTransform
                    Run Text: ')'"), result);
        }

        [UITestMethod]
        public void Hyperlink_Subreddit()
        {
            string result = RenderMarkdown("/r/subreddit");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Hyperlink NavigateUri: null
                        SolidColorBrush
                            Color A: 255, B: 41, G: 135, R: 204
                            MatrixTransform
                            MatrixTransform
                        Run Text: '/r/subreddit'
                            SolidColorBrush
                                Color A: 255, B: 41, G: 135, R: 204
                                MatrixTransform
                                MatrixTransform"), result);
        }
    }
}
