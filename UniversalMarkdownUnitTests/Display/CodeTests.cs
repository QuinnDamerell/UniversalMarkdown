using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class CodeTests : DisplayTestBase
    {
        [UITestMethod]
        public void Code_Inline()
        {
            string result = RenderMarkdown("Here is some `inline code` lol");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'Here is some '
                    Run Text: 'inline code'
                    Run Text: ' lol'"), result);
        }

        [UITestMethod]
        public void Code_Inline_Boundary()
        {
            string result = RenderMarkdown("before` middle `after");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'
                    Run Text: ' middle '
                    Run Text: 'after'"), result);
        }

        [UITestMethod]
        public void Code_Inline_Formatting()
        {
            // Formatting is ignored inside code.
            string result = RenderMarkdown("Here is some `ignored **formatting** inside code`");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'Here is some '
                    Run Text: 'ignored **formatting** inside code'"), result);
        }

        [UITestMethod]
        public void Code_Block()
        {
            // Multi-line code block.  Should have a border and scroll, not wrap!
            string result = RenderMarkdown(CollapseWhitespace(@"
                before

                    Code
                        More code with **stars**
                    Even more code

                after"));

            // Not a great rendering :-/
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'
                Paragraph Margin: '12,0,0,0'
                    FontFamily
                    SolidColorBrush
                        Color A: 180, B: 255, G: 255, R: 255
                        MatrixTransform
                        MatrixTransform
                    Run Text: 'Code'
                        FontFamily
                        SolidColorBrush
                            Color A: 180, B: 255, G: 255, R: 255
                            MatrixTransform
                            MatrixTransform
                Paragraph Margin: '24,0,0,0'
                    FontFamily
                    SolidColorBrush
                        Color A: 180, B: 255, G: 255, R: 255
                        MatrixTransform
                        MatrixTransform
                    Run Text: 'More code with '
                        FontFamily
                        SolidColorBrush
                            Color A: 180, B: 255, G: 255, R: 255
                            MatrixTransform
                            MatrixTransform
                    Span FontWeight: 700
                        FontFamily
                        SolidColorBrush
                            Color A: 180, B: 255, G: 255, R: 255
                            MatrixTransform
                            MatrixTransform
                        Run FontWeight: 700, Text: 'stars'
                            FontFamily
                            SolidColorBrush
                                Color A: 180, B: 255, G: 255, R: 255
                                MatrixTransform
                                MatrixTransform
                Paragraph Margin: '12,0,0,0'
                    FontFamily
                    SolidColorBrush
                        Color A: 180, B: 255, G: 255, R: 255
                        MatrixTransform
                        MatrixTransform
                    Run Text: 'Even more code'
                        FontFamily
                        SolidColorBrush
                            Color A: 180, B: 255, G: 255, R: 255
                            MatrixTransform
                            MatrixTransform
                Paragraph
                    Run Text: 'after'"), result);
        }

        [UITestMethod]
        public void Code_Block_Negative()
        {
            // Multi-line code blocks must start with a new paragraph.
            string result = RenderMarkdown(CollapseWhitespace(@"
                before
                    Code
                        More code
                    Even more code
                after"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before Code More code Even more code after'"), result);
        }
    }
}
