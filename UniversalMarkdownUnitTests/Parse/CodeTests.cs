using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class CodeTests : ParseTestBase
    {
        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Code_Inline()
        {
            AssertEqual("Here is some `inline code` lol",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "Here is some " },
                    new CodeInline { Text = "inline code" },
                    new TextRunInline { Text = " lol" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Code_Inline_Boundary()
        {
            AssertEqual("before` middle `after",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" },
                    new CodeInline { Text = " middle " },
                    new TextRunInline { Text = "after" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Code_Inline_Formatting()
        {
            // Formatting is ignored inside code.
            AssertEqual("Here is some `ignored **formatting** inside code`",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "Here is some " },
                    new CodeInline { Text = "ignored **formatting** inside code" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Code_Inline_Escape()
        {
            // Formatting is ignored inside code.
            AssertEqual(@"Here is some \`escaped code`",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "Here is some `escaped code`" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Code_Inline_Negative_CannotBeEmpty()
        {
            AssertEqual("before ``` after",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before ``` after" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Code_Block()
        {
            // Multi-line code block.  Should have a border and scroll, not wrap!
            AssertEqual(CollapseWhitespace(@"
                before

                    Code

                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new CodeBlock { Text = "Code" },
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "after" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Code_Block_With_Indent()
        {
            // Multi-line code block.  Should have a border and scroll, not wrap!
            AssertEqual(CollapseWhitespace(@"
                before

                    Code
                      More code with **stars** and   spacing
                    Even more code

                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new CodeBlock { Text = "Code\r\n  More code with **stars** and   spacing\r\nEven more code" },
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "after" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Code_Block_With_Tabs()
        {
            // A tab character can start a code block.
            // Tab characters inside the code are converted to 1-4 spaces.
            AssertEqual(CollapseWhitespace(@"
                before

                " + "\t" + @"Code
                " + "\t\t" + @"can
                " + "\tbe\t" + @"tabbed
                " + "\thole\t" + @"tabbed

                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new CodeBlock { Text = "Code\r\n    can\r\nbe  tabbed" },
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "after" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
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
