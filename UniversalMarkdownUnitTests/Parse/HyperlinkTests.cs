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
    }
}
