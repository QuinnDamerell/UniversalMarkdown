using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class QuoteTests : DisplayTestBase
    {
        [UITestMethod]
        [TestCategory("Display - block")]
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
    }
}
