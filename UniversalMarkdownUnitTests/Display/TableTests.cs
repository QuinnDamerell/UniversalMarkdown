using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class TableTests : DisplayTestBase
    {
        [UITestMethod]
        [TestCategory("Display - block")]
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
    }
}
