using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class QuoteTests : DisplayTestBase
    {
        [UITestMethod]
        public void Quote_SingleLine()
        {
            string result = RenderMarkdown(">Quoted text");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph Margin: '12,12,12,12'
                    SolidColorBrush
                        Color A: 180, B: 255, G: 255, R: 255
                        MatrixTransform
                        MatrixTransform
                    Run Text: 'Quoted text'
                        SolidColorBrush
                            Color A: 180, B: 255, G: 255, R: 255
                            MatrixTransform
                            MatrixTransform"), result);   // TODO
        }

        [UITestMethod]
        public void Quote_MultiLine()
        {
            string result = RenderMarkdown(CollapseWhitespace(@"
                before
                >Quoted
                >Quoted, line 2
                after"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'
                    Run Text: 'Quoted'
                    Run Text: 'Quoted, line 2'
                    Run Text: 'after'"), result);   // TODO
        }

        [UITestMethod]
        public void Quote_Nested()
        {
            string result = RenderMarkdown(CollapseWhitespace(@"
                before
                >Quoted
                >>Nested quote
                after"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'
                    Run Text: 'Quoted'
                    Run Text: 'Nested quote'
                    Run Text: 'after'"), result);   // TODO
        }

        [UITestMethod]
        public void Quote_MultiParagraph()
        {
            // Quotes can stretch across paragraphs.
            string result = RenderMarkdown(CollapseWhitespace(@"
                >Here's a quote.

                >Another paragraph in the same quote.
                >>A nested quote.

                >Back to a single quote.

                And finally some unquoted text."));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'Here's a quote.'
                Paragraph
                    Run Text: 'Another paragraph in the same quote.'
                    Run Text: 'A nested quote.'
                Paragraph
                    Run Text: 'Back to a single quote.'
                Paragraph
                    Run Text: 'And finally some unquoted text.'"), result);   // TODO
        }
    }
}
