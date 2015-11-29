using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class TableTests : DisplayTestBase
    {
        [UITestMethod]
        public void Table_Simple()
        {
            string result = RenderMarkdown(CollapseWhitespace(@"
                | Column 1 | Column 2 | Column 3 |
                |----------|----------|----------|
                | A        | B        | C        |"));
            Assert.AreEqual(CollapseWhitespace(@"
                        Paragraph
                            "), result);    // TODO
        }

        [UITestMethod]
        public void Table_WithAlignment()
        {
            string result = RenderMarkdown(CollapseWhitespace(@"
                | Column 1   | Column 2    | Column 3     |
                |:-----------|------------:|:------------:|
                | You        |          You|     You     
                | can align  |    can align|  can align   
                | left       |        right|   center     "));
            Assert.AreEqual(CollapseWhitespace(@"
                        Paragraph
                            "), result);    // TODO
        }

        [UITestMethod]
        public void Table_Dividers()
        {
            // Too many column dividers is okay.
            string result = RenderMarkdown(CollapseWhitespace(@"
                        Column A | Column B | Column C
                        -|-|-|-
                        A1 | B1 | C1"));
            Assert.AreEqual(CollapseWhitespace(@"
                        Paragraph
                            "), result);    // TODO
        }

        [UITestMethod]
        public void Tables_Negative_NewParagraph()
        {
            // Must start on a new paragraph.
            string result = RenderMarkdown(CollapseWhitespace(@"
                before
                | Column 1 | Column 2 | Column 3 |
                |----------|----------|----------|
                | A        | B        | C        |
                after"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before Column 1 | Column 2 | Column 3 | |----------|----------|----------| | A        | B        | C        | after'"), result);
        }

        [UITestMethod]
        public void Tables_Negative_TooFewDividers()
        {
            // But too few doesn't work.
            string result = RenderMarkdown(CollapseWhitespace(@"
                        Column A | Column B | Column C
                        -|-
                        A1 | B1 | C1"));
            Assert.AreEqual(CollapseWhitespace(@"
                        Paragraph
                            Run Text: 'Column A | Column B | Column C'
                            Run Text: '-|-'
                            Run Text: 'A1 | B1 | C1'"), result);
        }

    }
}
