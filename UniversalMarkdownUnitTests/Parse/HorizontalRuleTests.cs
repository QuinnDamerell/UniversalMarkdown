﻿using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class HorizontalRuleTests : ParseTestBase
    {
        [UITestMethod]
        public void HorizontalRule_Simple()
        {
            AssertEqual("***",
                new HorizontalRuleBlock());
        }

        [UITestMethod]
        public void HorizontalRule_StarsAndSpaces()
        {
            AssertEqual("* * * * *",
                new HorizontalRuleBlock());
        }

        [UITestMethod]
        public void HorizontalRule_Alt()
        {
            AssertEqual("---",
                new HorizontalRuleBlock());
        }

        [UITestMethod]
        public void HorizontalRule_BeforeAfter()
        {
            // Text on other lines is okay.
            AssertEqual(CollapseWhitespace(@"
                before
                *****
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new HorizontalRuleBlock(),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "after" }));
        }

        [UITestMethod]
        public void HorizontalRule_Negative()
        {
            // Text on the same line is not.
            AssertEqual(CollapseWhitespace(@"
                before
                *****d
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before ****d after" }));
        }

        [UITestMethod]
        public void HorizontalRule_Negative_FourStars()
        {
            // Also, must be at least 3 stars.
            AssertEqual(CollapseWhitespace(@"
                before
                **
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before ** after" }));
        }
    }
}