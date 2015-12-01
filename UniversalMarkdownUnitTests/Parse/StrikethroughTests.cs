using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class StrikethroughTests : ParseTestBase
    {
        [UITestMethod]
        public void Strikethrough_Simple()
        {
            Assert.Fail("Not implemented");
            //AssertEqual("~~strike~~",
            //    new ParagraphBlock().AddChildren(
            //        new StrikethroughTextInline().AddChildren(
            //            new TextRunInline { Text = "strike" })));
        }

        [UITestMethod]
        public void Strikethrough_Inline()
        {
            Assert.Fail("Not implemented");
            //AssertEqual("This is ~~strike~~ text",
            //    new ParagraphBlock().AddChildren(
            //        new TextRunInline { Text = "This is " },
            //        new StrikethroughTextInline().AddChildren(
            //            new TextRunInline { Text = "strike" }),
            //        new TextRunInline { Text = " text" }));
        }
    }
}