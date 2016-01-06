using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class BulletedListTests : ParseTestBase
    {
        [UITestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_SingleLine()
        {
            AssertEqual("- List",
                new ListBlock().AddChildren(
                    new TextRunInline { Text = "List" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Simple()
        {
            AssertEqual(CollapseWhitespace(@"
                before

                - List item 1
                * List item 2
                + List item 3

                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new ListBlock().AddChildren(
                    new TextRunInline { Text = "List item 1" },
                    new TextRunInline { Text = "List item 2" },
                    new TextRunInline { Text = "List item 3" }),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "after" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Nested()
        {
            AssertEqual(CollapseWhitespace(@"
                - List item 1
                    - Nested item
                + List item 2"),
                new ListBlock().AddChildren(
                    new TextRunInline { Text = "List item 1" },
                    new ListBlock().AddChildren(
                        new TextRunInline { Text = "Nested item" }),
                    new TextRunInline { Text = "List item 2" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Negative_SpaceRequired()
        {
            // The space is required.
            AssertEqual("-List",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "-List" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Negative_NewParagraph()
        {
            // Bulleted lists must start on a new paragraph
            AssertEqual(CollapseWhitespace(@"
                before
                * List
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before * List after" }));
        }
    }
}
