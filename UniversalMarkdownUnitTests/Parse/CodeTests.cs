using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class CodeTests : ParseTestBase
    {
        [UITestMethod]
        public void Code_Inline()
        {
            Assert.Fail("Not supported.");
            //AssertEqual("Here is some `inline code` lol",
            //    new ParagraphBlock().AddChildren(
            //        new TextRunInline { Text = "Here is some " },
            //        new CodeRunInline().AddChildren(
            //            new TextRunInline { Text = "inline code" }),
            //        new TextRunInline { Text = " lol" }));
        }

        [UITestMethod]
        public void Code_Inline_Boundary()
        {
            Assert.Fail("Not supported.");
            //AssertEqual("before` middle `after",
            //    new ParagraphBlock().AddChildren(
            //        new TextRunInline { Text = "before" },
            //        new CodeRunInline().AddChildren(
            //            new TextRunInline { Text = " middle " }),
            //        new TextRunInline { Text = "after" }));
        }

        [UITestMethod]
        public void Code_Inline_Formatting()
        {
            // Formatting is ignored inside code.
            Assert.Fail("Not supported.");
            //AssertEqual("Here is some `ignored **formatting** inside code`",
            //    new ParagraphBlock().AddChildren(
            //        new TextRunInline { Text = "Here is some " },
            //        new CodeRunInline().AddChildren(
            //            new TextRunInline { Text = " ignored **formatting** inside code " }));
        }

        [UITestMethod]
        public void Code_Block()
        {
            // Multi-line code block.  Should have a border and scroll, not wrap!
            AssertEqual(CollapseWhitespace(@"
                before

                    Code

                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new CodeBlock().AddChildren(
                    new TextRunInline { Text = "Code" }),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "after" }));
        }

        [UITestMethod]
        public void Code_Block_With_Indent()
        {
            // Multi-line code block.  Should have a border and scroll, not wrap!
            AssertEqual(CollapseWhitespace(@"
                before

                    Code
                      More code with **stars**
                    Even more code

                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new CodeBlock().AddChildren(
                    new TextRunInline { Text = "Code\r\n  More code with **stars**\r\nEven more code" }),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "after" }));
        }

        [UITestMethod]
        public void Code_Block_Negative()
        {
            // Multi-line code blocks must start with a new paragraph.
            AssertEqual(CollapseWhitespace(@"
                before
                    Code
                        More code
                    Even more code
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before Code More code Even more code after'" }));
        }
    }
}
