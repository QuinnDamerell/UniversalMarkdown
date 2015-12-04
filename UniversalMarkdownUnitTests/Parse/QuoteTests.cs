using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class QuoteTests : ParseTestBase
    {
        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Quote_SingleLine()
        {
            AssertEqual(">Quoted text",
                new QuoteBlock().AddChildren(
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "Quoted text" })));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Quote_MultiLine_1()
        {
            AssertEqual(CollapseWhitespace(@"
                >Quoted  
                line 1

                >Quoted  
                line 2"),
                new QuoteBlock().AddChildren(
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "Quoted line 1" }),
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "Quoted line 2" })));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Quote_MultiLine()
        {
            AssertEqual(CollapseWhitespace(@"
                before
                >Quoted
                >Quoted, line 2
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new QuoteBlock().AddChildren(
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "Quoted Quoted, line 2 after" })));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Quote_Nested()
        {
            AssertEqual(CollapseWhitespace(@"
                >Quoted
                >>Nested quote"),
                new QuoteBlock().AddChildren(
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "Quoted" }),
                    new QuoteBlock().AddChildren(
                        new ParagraphBlock().AddChildren(
                            new TextRunInline { Text = "Nested quote" }))));
        }
    }
}
