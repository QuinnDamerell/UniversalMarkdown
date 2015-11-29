using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class ParagraphTests : DisplayTestBase
    {
        [UITestMethod]
        public void Empty()
        {
            var result = RenderMarkdown("");
            Assert.AreEqual("", result);
        }

        [UITestMethod]
        public void Paragraph_NoLineBreak()
        {
            // A line break in the markup does not translate to a line break in the resulting formatted text.
            string result = RenderMarkdown(CollapseWhitespace(@"
                line 1
                line 2"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'line 1 line 2'"), result);
        }

        [UITestMethod]
        public void Paragraph_NoLineBreak_OneSpace()
        {
            // A line break in the markup does not translate to a line break in the resulting formatted text.
            string result = RenderMarkdown(CollapseWhitespace(@"
                line 1 
                line 2"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'line 1 line 2'"), result);
        }

        [UITestMethod]
        public void Paragraph_LineBreak()
        {
            // Two spaces at the end of the line results in a line break.
            string result = RenderMarkdown(CollapseWhitespace(@"
                line 1  
                line 2"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'line 1'
                    LineBreak
                    Run Text: 'line 2'"), result);
        }

        [UITestMethod]
        public void Paragraph_LineBreak_ThreeSpaces()
        {
            // Three spaces at the end of the line also results in a line break.
            string result = RenderMarkdown(CollapseWhitespace(@"
                line 1   
                line 2"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'line 1'
                    LineBreak
                    Run Text: 'line 2'"), result);
        }

        [UITestMethod]
        public void Paragraph_NewParagraph()
        {
            // An empty line starts a new paragraph.
            string result = RenderMarkdown(CollapseWhitespace(@"
                line 1

                line 2"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'line 1'
                Paragraph
                    Run Text: 'line 2'"), result);
        }

        [UITestMethod]
        public void Paragraph_NewParagraph_Whitespace()
        {
            // A line that contains only whitespace starts a new paragraph.
            string result = RenderMarkdown(CollapseWhitespace(@"
                        line 1

                        line 2"));
            Assert.AreEqual(CollapseWhitespace(@"
                        Paragraph
                            Run Text: 'line 1'
                        Paragraph
                            Run Text: 'line 2'"), result);
        }
    }
}
