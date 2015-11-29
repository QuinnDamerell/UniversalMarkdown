using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class EscapeTests : DisplayTestBase
    {
        [UITestMethod]
        public void Escape_1()
        {
            string result = RenderMarkdown(@"\*escape the formatting syntax\*");
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: '*escape the formatting syntax*'"), result);
        }
    }
}
