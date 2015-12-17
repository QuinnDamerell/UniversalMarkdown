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
        public void Hyperlink_Http_Uppercase()
        {
            AssertEqual("HTTP://reddit.com",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Url = "HTTP://reddit.com" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Http_Inline()
        {
            AssertEqual("The best site (http://reddit.com) goes well with http://www.wikipedia.com, don't you think?",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "The best site (" },
                    new RawHyperlinkInline { Url = "http://reddit.com" },
                    new TextRunInline { Text = ") goes well with " },
                    new RawHyperlinkInline { Url = "http://www.wikipedia.com" },
                    new TextRunInline { Text = ", don't you think?" }));
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
        public void Hyperlink_AngleBrackets()
        {
            AssertEqual("<http://reddit.com>",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Url = "http://reddit.com" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_EndCharacters()
        {
            AssertEqual(CollapseWhitespace(@"
                http://reddit.com)

                http://reddit.com)a

                http://reddit.com}

                http://reddit.com}a

                http://reddit.com]

                http://reddit.com]a

                http://reddit.com>

                http://reddit.com|

                http://reddit.com`

                http://reddit.com^

                http://reddit.com~

                http://reddit.com[

                http://reddit.com(

                http://reddit.com{

                http://reddit.com<

                http://reddit.com<a

                http://reddit.com#

                http://reddit.com%

                http://reddit.com!

                http://reddit.com!a

                http://reddit.com;

                http://reddit.com;a

                http://reddit.com.

                http://reddit.com.a

                http://reddit.com-

                http://reddit.com+

                http://reddit.com=

                http://reddit.com_

                http://reddit.com*

                http://reddit.com&

                http://reddit.com?

                http://reddit.com?a

                http://reddit.com,

                http://reddit.com,a"),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com" }, new TextRunInline { Text = ")" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com)a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com" }, new TextRunInline { Text = "}" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com}a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com" }, new TextRunInline { Text = "]" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com]a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com>" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com|" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com`" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com^" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com~" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com[" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com(" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com{" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com" }, new TextRunInline { Text = "<" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com" }, new TextRunInline { Text = "<a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com#" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com%" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com" }, new TextRunInline { Text = "!" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com!a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com" }, new TextRunInline { Text = ";" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com;a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com" }, new TextRunInline { Text = "." }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com.a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com-" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com+" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com=" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com_" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com*" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com&" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com" }, new TextRunInline { Text = "?" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com?a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com" }, new TextRunInline { Text = "," }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com,a" }));
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
            AssertEqual("[reddit] ( /blog )",
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
