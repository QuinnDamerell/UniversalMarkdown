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
        public void Hyperlink_AngleBracketsNoNeedForDot()
        {
            AssertEqual("<http://reddit>",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Url = "http://reddit" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_AngleBracketsCanEndWithPunctuation()
        {
            AssertEqual("<http://reddit.com.>",
                new ParagraphBlock().AddChildren(
                    new RawHyperlinkInline { Url = "http://reddit.com." }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_AngleBracketsCantHaveSpaces()
        {
            AssertEqual("< http://reddit.com >",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "< " },
                    new RawHyperlinkInline { Url = "http://reddit.com" },
                    new TextRunInline { Text = " >" }));
        }

        [UITestMethod]
        [TestCategory("Parse - inline")]
        public void Hyperlink_StartCharacters()
        {
            AssertEqual("0http://reddit.com",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "0" },
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
    }
}
