using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class ItalicTests : DisplayTestBase
    {
        [UITestMethod]
        public void Italic_Simple()
        {
            var result = RenderMarkdown("*italic*");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Span FontStyle: Italic
                        Run FontStyle: Italic, Text: 'italic'"), result);
        }

        [UITestMethod]
        public void Italic_Simple_Alt()
        {
            string result = RenderMarkdown("_italic_");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Span FontStyle: Italic
                        Run FontStyle: Italic, Text: 'italic'"), result);
        }

        [UITestMethod]
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

        [UITestMethod]
        public void Italic_Inline_Alt()
        {
            string result = RenderMarkdown("This is _italic_ text");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'This is '
                    Span FontStyle: Italic
                        Run FontStyle: Italic, Text: 'bold'
                    Run Text: ' text'"), result);
        }

        [UITestMethod]
        public void Italic_Inside_Word()
        {
            string result = RenderMarkdown("before*middle*end");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'
                    Span FontStyle: Italic
                        Run FontStyle: Italic, Text: 'middle'
                    Run Text: 'end'"), result);
        }

        [UITestMethod]
        public void Italic_MultiLine()
        {
            // Does work across lines.
            string result = RenderMarkdown(CollapseWhitespace(@"
                italics *does  
                work* across line breaks"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'italics '
                    Run FontStyle: Italic, Text: 'does'
                    LineBreak
                    Run FontStyle: Italic, Text: 'work'
                    Run Text: ' across line breaks'"), result);
        }

        [UITestMethod]
        public void Italic_Negative_1()
        {
            string result = RenderMarkdown("before** middle **end");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before** middle **end'"), result);
        }

        [UITestMethod]
        public void Italic_Negative_2()
        {
            string result = RenderMarkdown("before** middle**end");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before** middle **end'"), result);
        }

        [UITestMethod]
        public void Italic_Negative_3()
        {
            // There must be a valid end italics marker otherwise the whole thing is ignored.
            string result = RenderMarkdown("This is *not italics * text");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'This is *not italics * text'"), result);
        }

        [UITestMethod]
        public void Italic_Negative_MultiParagraph()
        {
            // Doesn't work across paragraphs.
            string result = RenderMarkdown(CollapseWhitespace(@"
                italics *doesn't

                apply* across paragraphs"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'italics *doesn't'
                Paragraph
                    Run Text: 'apply* across paragraphs'"), result);
        }
    }
}
