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
                    new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_WithSurroundingText()
        {
            AssertEqual("Narwhal http://reddit.com fail whale",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "Narwhal " },
                    new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" },
                    new TextRunInline { Text = " fail whale" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Http_Uppercase()
        {
            AssertEqual("HTTP://reddit.com",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Url = "HTTP://reddit.com", Text = "HTTP://reddit.com" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Http_Inline()
        {
            AssertEqual("The best site (http://reddit.com) goes well with http://www.wikipedia.com, don't you think?",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "The best site (" },
                    new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" },
                    new TextRunInline { Text = ") goes well with " },
                    new RawHyperlinkInline { Url = "http://www.wikipedia.com", Text = "http://www.wikipedia.com" },
                    new TextRunInline { Text = ", don't you think?" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Https()
        {
            AssertEqual("https://reddit.com",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Url = "https://reddit.com", Text = "https://reddit.com" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Mailto()
        {
            AssertEqual("bob@bob.com",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Url = "mailto:bob@bob.com", Text = "bob@bob.com" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_AngleBrackets()
        {
            AssertEqual("<http://reddit.com>",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_AngleBracketsNoNeedForDot()
        {
            AssertEqual("<http://reddit>",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Url = "http://reddit", Text = "http://reddit" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_AngleBracketsCanEndWithPunctuation()
        {
            AssertEqual("<http://reddit.com.>",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Url = "http://reddit.com.", Text = "http://reddit.com." }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_AngleBracketsCantHaveSpaces()
        {
            AssertEqual("< http://reddit.com >",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "< " },
                    new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" },
                    new TextRunInline { Text = " >" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_StartCharacters()
        {
            AssertEqual("0http://reddit.com",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "0" },
                    new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_EndCharacters()
        {
            AssertEqual(CollapseWhitespace(@"
                http://reddit.com)

                http://reddit.com).

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
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }, new TextRunInline { Text = ")" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }, new TextRunInline { Text = ")." }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com)a", Text = "http://reddit.com)a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }, new TextRunInline { Text = "}" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com}a", Text = "http://reddit.com}a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }, new TextRunInline { Text = "]" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com]a", Text = "http://reddit.com]a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com>", Text = "http://reddit.com>" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com|", Text = "http://reddit.com|" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com`", Text = "http://reddit.com`" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com^", Text = "http://reddit.com^" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com~", Text = "http://reddit.com~" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com[", Text = "http://reddit.com[" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com(", Text = "http://reddit.com(" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com{", Text = "http://reddit.com{" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }, new TextRunInline { Text = "<" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }, new TextRunInline { Text = "<a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com#", Text = "http://reddit.com#" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com%", Text = "http://reddit.com%" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }, new TextRunInline { Text = "!" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com!a", Text = "http://reddit.com!a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }, new TextRunInline { Text = ";" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com;a", Text = "http://reddit.com;a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }, new TextRunInline { Text = "." }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com.a", Text = "http://reddit.com.a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com-", Text = "http://reddit.com-" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com+", Text = "http://reddit.com+" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com=", Text = "http://reddit.com=" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com_", Text = "http://reddit.com_" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com*", Text = "http://reddit.com*" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com&", Text = "http://reddit.com&" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }, new TextRunInline { Text = "?" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com?a", Text = "http://reddit.com?a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com", Text = "http://reddit.com" }, new TextRunInline { Text = "," }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://reddit.com,a", Text = "http://reddit.com,a" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_OtherSchemes()
        {
            AssertEqual(CollapseWhitespace(@"
                http://test.com

                https://test.com

                ftp://test.com

                mailto:dfg@test.com

                steam://test.com

                irc://test.com

                news://test.com

                mumble://test.com

                ssh://test.com"),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "http://test.com", Text = "http://test.com" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "https://test.com", Text = "https://test.com" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "ftp://test.com", Text = "ftp://test.com" }),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "mailto:" },
                    new RawHyperlinkInline { Url = "dfg@test.com" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "steam://test.com", Text = "steam://test.com" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "irc://test.com", Text = "irc://test.com" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "mumble://test.com", Text = "mumble://test.com" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Url = "ssh://test.com", Text = "ssh://test.com" }));
        }

        

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Negative_SurroundingText()
        {
            AssertEqual("thttp://reddit.com",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "thttp://reddit.com" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Negative_SchemeOnly()
        {
            AssertEqual("http:",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "http:" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Negative_PrefixOnly()
        {
            AssertEqual("http://",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "http://" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Negative_NoDot()
        {
            AssertEqual("http://localhost",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "http://localhost" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Negative_DotTooSoon()
        {
            AssertEqual("http://.com",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "http://.com" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Negative_AngleBracketsPrefixOnly()
        {
            AssertEqual("<http://>",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "<http://>" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_Negative_MailtoNeedsADot()
        {
            AssertEqual("bob@bob",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "bob@bob" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_WithSlash()
        {
            AssertEqual("/r/subreddit",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Text = "/r/subreddit", Url = "/r/subreddit", LinkType = HyperlinkType.Subreddit }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_WithoutSlash()
        {
            AssertEqual("r/subreddit",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Text = "r/subreddit", Url = "/r/subreddit", LinkType = HyperlinkType.Subreddit }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_Short()
        {
            // Subreddit names can be min two chars long.
            AssertEqual("/r/ab",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Text = "/r/ab", Url = "/r/ab", LinkType = HyperlinkType.Subreddit }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_WithBeginningEscape()
        {
            AssertEqual(@"\/r/subreddit",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "/r/subreddit" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_WithMiddleEscape()
        {
            AssertEqual(@"r\/subreddit",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "r/subreddit" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void SubredditLink_EndCharacters()
        {
            AssertEqual(CollapseWhitespace(@"
                /r/news)

                /r/news).

                /r/news)a

                /r/news}

                /r/news}a

                /r/news]

                /r/news]a

                /r/news>

                /r/news|

                /r/news`

                /r/news^

                /r/news~

                /r/news[

                /r/news(

                /r/news{

                /r/news<

                /r/news<a

                /r/news#

                /r/news%

                /r/news!

                /r/news!a

                /r/news;

                /r/news;a

                /r/news.

                /r/news.a

                /r/news-

                /r/news=

                /r/news_

                /r/news*

                /r/news&

                /r/news?

                /r/news?a

                /r/news,

                /r/news,a

                /r/news0"),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = ")" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = ")." }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = ")a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "}" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "}a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "]" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "]a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = ">" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "|" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "`" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "^" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "~" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "[" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "(" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "{" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "<" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "<a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "#" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "%" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "!" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "!a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = ";" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = ";a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "." }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = ".a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "-" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "=" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news_", Url = "/r/news_" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "*" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "&" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "?" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "?a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = "," }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news", Url = "/r/news" }, new TextRunInline { Text = ",a" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/news0", Url = "/r/news0" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_PlusCharacter()
        {
            // The plus character is treated strangely.
            AssertEqual(CollapseWhitespace(@"
                /r/+

                /r/+a

                /r/+ab

                /r/a+b

                /r/a+bc

                /r/ab+c

                /r/ab+cd

                /r/a+

                /r/ab+"),
                new ParagraphBlock().AddChildren(new TextRunInline { Text = "/r/+" }),
                new ParagraphBlock().AddChildren(new TextRunInline { Text = "/r/+a" }),
                new ParagraphBlock().AddChildren(new TextRunInline { Text = "/r/+ab" }),
                new ParagraphBlock().AddChildren(new TextRunInline { Text = "/r/a+b" }),
                new ParagraphBlock().AddChildren(new TextRunInline { Text = "/r/a+bc" }),
                new ParagraphBlock().AddChildren(new TextRunInline { Text = "/r/ab+c" }),
                new ParagraphBlock().AddChildren(new RawHyperlinkInline { Text = "/r/ab+cd", Url = "/r/ab+cd", LinkType = HyperlinkType.Subreddit }),
                new ParagraphBlock().AddChildren(new TextRunInline { Text = "/r/a+" }),
                new ParagraphBlock().AddChildren(new TextRunInline { Text = "/r/ab+" }));
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
                    new RawHyperlinkInline { Text = "/u/quinbd", Url = "/u/quinbd", LinkType = HyperlinkType.User }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void UserLink_WithoutSlash()
        {
            AssertEqual("u/quinbd",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Text = "u/quinbd", Url = "/u/quinbd", LinkType = HyperlinkType.User }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void UserLink_Short()
        {
            // User names can be one char long.
            AssertEqual("/u/u",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Text = "/u/u", Url = "/u/u", LinkType = HyperlinkType.User }));
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
