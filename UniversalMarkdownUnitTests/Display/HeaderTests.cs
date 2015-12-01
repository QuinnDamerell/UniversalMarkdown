using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class HeaderTests : DisplayTestBase
    {
        [UITestMethod]
        [TestCategory("Display - block")]
        public void Header_1()
        {
            string result = RenderMarkdown("#Header 1");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph FontSize: 20, FontWeight: 700, Margin: '0,18,0,12'
                    Run FontSize: 20, FontWeight: 700, Text: 'Header 1'"), result);
        }

        [UITestMethod]
        [TestCategory("Display - block")]
        public void Header_2()
        {
            string result = RenderMarkdown("##Header 2");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph FontSize: 20, Margin: '0,18,0,12'
                    Run FontSize: 20, Text: 'Header 2'"), result);
        }

        [UITestMethod]
        [TestCategory("Display - block")]
        public void Header_3()
        {
            string result = RenderMarkdown("###Header 3");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph FontSize: 17, FontWeight: 700, Margin: '0,18,0,12'
                    Run FontSize: 17, FontWeight: 700, Text: 'Header 3'"), result);
        }

        [UITestMethod]
        [TestCategory("Display - block")]
        public void Header_4()
        {
            string result = RenderMarkdown("####Header 4");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph FontSize: 17, Margin: '0,18,0,12'
                    Run FontSize: 17, Text: 'Header 4'"), result);
        }

        [UITestMethod]
        [TestCategory("Display - block")]
        public void Header_5()
        {
            string result = RenderMarkdown("#####Header 5");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph FontWeight: 700, Margin: '0,18,0,12'
                    Run FontWeight: 700, Text: 'Header 5'"), result);
        }

        [UITestMethod]
        [TestCategory("Display - block")]
        public void Header_6()
        {
            string result = RenderMarkdown("######Header 6");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph Margin: '0,18,0,12'
                    Run Text: 'Header 6'"), result);
        }
    }
}
