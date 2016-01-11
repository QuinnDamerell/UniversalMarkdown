using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class SubredditLinkTests : ParseTestBase
    {
        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_WithSlash()
        {
            AssertEqual("/r/subreddit",
                new ParagraphBlock().AddChildren(
                    new RawSubredditInline { Text = "/r/subreddit" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_WithoutSlash()
        {
            AssertEqual("r/subreddit",
                new ParagraphBlock().AddChildren(
                    new RawSubredditInline { Text = "r/subreddit" }));
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
        public void SubredditLink_Negative_TooShort()
        {
            // The subreddit name must be at least 2 chars.
            AssertEqual("r/a",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "r/a" }));
        }
    }
}
