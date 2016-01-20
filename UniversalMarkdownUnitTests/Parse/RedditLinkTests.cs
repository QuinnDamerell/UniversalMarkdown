using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class RedditLinkTests : ParseTestBase
    {
        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_WithSlash()
        {
            AssertEqual("/r/subreddit",
                new ParagraphBlock().AddChildren(
                    new RedditLinkInline { Text = "/r/subreddit", LinkType = RedditLinkType.Subreddit }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_WithoutSlash()
        {
            AssertEqual("r/subreddit",
                new ParagraphBlock().AddChildren(
                    new RedditLinkInline { Text = "r/subreddit", LinkType = RedditLinkType.Subreddit }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_Short()
        {
            // Subreddit names can be min two chars long.
            AssertEqual("/r/ab",
                new ParagraphBlock().AddChildren(
                    new RedditLinkInline { Text = "/r/ab", LinkType = RedditLinkType.Subreddit }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_Negative_SurroundingText()
        {
            AssertEqual("bear/subreddit",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "bear/subreddit" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_Negative_PrefixOnly()
        {
            AssertEqual("r/",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "r/" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_Negative_UppercaseWithoutSlash()
        {
            AssertEqual("R/baconit",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "R/baconit" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_Negative_UppercaseWithSlash()
        {
            AssertEqual("/R/baconit",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "/R/baconit" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_Negative_TooShort()
        {
            // The subreddit name must be at least 2 chars.
            AssertEqual("r/a",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "r/a" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void UserLink_WithSlash()
        {
            AssertEqual("/u/quinbd",
                new ParagraphBlock().AddChildren(
                    new RedditLinkInline { Text = "/u/quinbd", LinkType = RedditLinkType.User }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void UserLink_WithoutSlash()
        {
            AssertEqual("u/quinbd",
                new ParagraphBlock().AddChildren(
                    new RedditLinkInline { Text = "u/quinbd", LinkType = RedditLinkType.User }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void UserLink_Short()
        {
            // User names can be one char long.
            AssertEqual("/u/u",
                new ParagraphBlock().AddChildren(
                    new RedditLinkInline { Text = "/u/u", LinkType = RedditLinkType.User }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void UserLink_Negative_PrefixOnly()
        {
            AssertEqual("u/",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "u/" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void UserLink_Negative_UppercaseWithoutSlash()
        {
            AssertEqual("U/quinbd",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "U/quinbd" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void UserLink_Negative_UppercaseWithSlash()
        {
            AssertEqual("/U/quinbd",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "/U/quinbd" }));
        }
    }
}
