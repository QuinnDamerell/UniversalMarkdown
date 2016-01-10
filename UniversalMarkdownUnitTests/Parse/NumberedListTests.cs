using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.Generic;
using UniversalMarkdown.Parse;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class NumberedListTests : ParseTestBase
    {
        [UITestMethod]
        [TestCategory("Parse - block")]
        public void NumberedList_SingleLine()
        {
            AssertEqual("1. List",
                new ListBlock { Style = ListStyle.Numbered }.AddChildren(
                    new ListItemBlock { Blocks = new List<MarkdownBlock> { new ParagraphBlock().AddChildren(new TextRunInline { Text = "List" }) } }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void NumberedList_Numbering()
        {
            // The numbers are ignored, and they can be any length.
            AssertEqual(CollapseWhitespace(@"
                7. List item 1
                502. List item 2
                502456456456456456456456456456456456. List item 3"),
                new ListBlock { Style = ListStyle.Numbered }.AddChildren(
                    new ListItemBlock { Blocks = new List<MarkdownBlock> { new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }) } },
                    new ListItemBlock { Blocks = new List<MarkdownBlock> { new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" }) } },
                    new ListItemBlock { Blocks = new List<MarkdownBlock> { new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 3" }) } }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void NumberedList_Negative_SpaceRequired()
        {
            // A space is required after the dot.
            AssertEqual("1.List", new ParagraphBlock().AddChildren(
                new TextRunInline { Text = "1.List" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void NumberedList_Negative_NoLetters()
        {
            // Only digits can make a numbered list.
            AssertEqual("a. List", new ParagraphBlock().AddChildren(
                new TextRunInline { Text = "a. List" }));
        }
    }
}
