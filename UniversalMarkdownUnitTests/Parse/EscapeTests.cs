using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class EscapeTests : ParseTestBase
    {
        [UITestMethod]
        public void Escape_1()
        {
            AssertEqual(@"\*escape the formatting syntax\*", new ParagraphBlock().AddChildren(
                new TextRunInline { Text = "*escape the formatting syntax*" }));
        }
    }
}
