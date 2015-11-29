using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class HeaderTests : DisplayTestBase
    {
        [UITestMethod]
        public void Header_1()
        {
            string result = RenderMarkdown("#Header 1");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph FontSize: 20, FontWeight: 700, Margin: '0,18,0,12'
                    Run FontSize: 20, FontWeight: 700, Text: 'Header 1'"), result);
        }

        [UITestMethod]
        public void Header_1_Alt()
        {
            string result = RenderMarkdown(CollapseWhitespace(@"
                Header 1
                ="));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph FontSize: 20, FontWeight: 700, Margin: '0,18,0,12'
                    Run FontSize: 20, FontWeight: 700, Text: 'Header 1'"), result);
        }

        [UITestMethod]
        public void Header_2()
        {
            string result = RenderMarkdown("##Header 2");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph FontSize: 20, Margin: '0,18,0,12'
                    Run FontSize: 20, Text: 'Header 2'"), result);
        }

        [UITestMethod]
        public void Header_2_Alt()
        {
            // Note: trailing spaces on the second line are okay.
            string result = RenderMarkdown(CollapseWhitespace(@"
                Header 2
                -  "));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph FontSize: 20, Margin: '0,18,0,12'
                    Run FontSize: 20, Text: 'Header 2'"), result);
        }

        [UITestMethod]
        public void Header_3()
        {
            string result = RenderMarkdown("###Header 3");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph FontSize: 17, FontWeight: 700, Margin: '0,18,0,12'
                    Run FontSize: 17, FontWeight: 700, Text: 'Header 3'"), result);
        }

        [UITestMethod]
        public void Header_4()
        {
            string result = RenderMarkdown("####Header 4");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph FontSize: 17, Margin: '0,18,0,12'
                    Run FontSize: 17, Text: 'Header 4'"), result);
        }

        [UITestMethod]
        public void Header_5()
        {
            string result = RenderMarkdown("#####Header 5");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph FontWeight: 700, Margin: '0,18,0,12'
                    Run FontWeight: 700, Text: 'Header 5'"), result);
        }

        [UITestMethod]
        public void Header_6()
        {
            string result = RenderMarkdown("######Header 6");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph Margin: '0,18,0,12'
                    Run Text: 'Header 6'"), result);
        }

        [UITestMethod]
        public void Header_6_WithTrailingHashSymbols()
        {
            string result = RenderMarkdown("###### Header 6 ######");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph Margin: '0,18,0,12'
                    Run Text: 'Header 6'"), result);
        }

        [UITestMethod]
        public void Header_6_Inline()
        {
            string result = RenderMarkdown(CollapseWhitespace(@"
                before
                #Header
                after"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'
                Paragraph FontSize: 20, FontWeight: 700, Margin: '0,18,0,12'
                    Run FontSize: 20, FontWeight: 700, Text: 'Header'
                Paragraph Margin: '0,12,0,0'
                    Run Text: 'after'"), result);
        }

        [UITestMethod]
        public void Header_Negative_RogueCharacter()
        {
            // The second line after a heading must be all === or all ---
            string result = RenderMarkdown(CollapseWhitespace(@"
                Header 1
                =f"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'Header 1'
                    Run Text: '=f'"), result);
        }

        [UITestMethod]
        public void Header_Negative_ExtraSpace()
        {
            // The second line after a heading must not start with a space
            string result = RenderMarkdown(CollapseWhitespace(@"
                Header 1
                    ="));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'Header 1'
                    Run Text: ' ='"), result);
        }
    }
}
