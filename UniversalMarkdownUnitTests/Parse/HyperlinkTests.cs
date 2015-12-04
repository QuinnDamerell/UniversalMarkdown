using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class HyperlinkTests : ParseTestBase
    {
        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Http()
        {
            AssertEqual("http://reddit.com",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Url = "http://reddit.com" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Https()
        {
            AssertEqual("https://reddit.com",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Url = "https://reddit.com" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_WithLabel()
        {
            AssertEqual("[reddit](http://reddit.com)",
                new ParagraphBlock().AddChildren(
                    new MarkdownLinkInline { Url = "http://reddit.com" }.AddChildren(
                        new TextRunInline { Text = "reddit" })));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_RelativeLink()
        {
            AssertEqual("[reddit] (/blog)",
                new ParagraphBlock().AddChildren(
                    new MarkdownLinkInline { Url = "/blog" }.AddChildren(     // Should the URL be https://www.reddit.com/blog?
                        new TextRunInline { Text = "reddit" })));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_WithLabelSpacing()
        {
            AssertEqual("[reddit] (http://reddit.com)",
                new ParagraphBlock().AddChildren(
                    new MarkdownLinkInline { Url = "http://reddit.com" }.AddChildren(
                        new TextRunInline { Text = "reddit" })));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_WithLabelAndFormatting()
        {
            AssertEqual("[red**dit**](http://reddit.com)",
                new ParagraphBlock().AddChildren(
                    new MarkdownLinkInline { Url = "http://reddit.com" }.AddChildren(
                        new TextRunInline { Text = "red" },
                        new BoldTextInline().AddChildren(
                            new TextRunInline { Text = "dit" }))));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_WithTooltip()
        {
            AssertEqual(@"[Wikipedia](http://en.wikipedia.org ""tooltip text"")",
                new ParagraphBlock().AddChildren(
                    new MarkdownLinkInline { Url = "http://reddit.com", Tooltip = "tooltip text" }.AddChildren(
                        new TextRunInline { Text = "reddit" })));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Escape()
        {
            // The link stops at the first ')'
            AssertEqual(@"[test](http://en.wikipedia.org/wiki/Pica_\(disorder\))",
                new ParagraphBlock().AddChildren(
                    new MarkdownLinkInline { Url = "http://en.wikipedia.org/wiki/Pica_(disorder)" }.AddChildren(
                        new TextRunInline { Text = "test" })));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Subreddit()
        {
            AssertEqual("/r/subreddit",
                new ParagraphBlock().AddChildren(
                    new RawSubredditInline { Text = "/r/subreddit" }));
        }
    }
}
