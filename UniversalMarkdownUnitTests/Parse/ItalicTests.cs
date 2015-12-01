using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class ItalicTests : ParseTestBase
    {
        [UITestMethod]
        public void Italic_Simple()
        {
            AssertEqual("*italic*",
                new ParagraphBlock().AddChildren(
                    new ItalicTextInline().AddChildren(
                        new TextRunInline { Text = "italic" })));
        }

        [UITestMethod]
        public void Italic_Simple_Alt()
        {
            AssertEqual("_italic_",
                new ParagraphBlock().AddChildren(
                    new ItalicTextInline().AddChildren(
                        new TextRunInline { Text = "italic" })));
        }

        [UITestMethod]
        public void Italic_Inline()
        {
            AssertEqual("This is *italic* text",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "This is " },
                    new ItalicTextInline().AddChildren(
                        new TextRunInline { Text = "italic" }),
                    new TextRunInline { Text = " text" }));
        }

        [UITestMethod]
        public void Italic_Inline_Alt()
        {
            AssertEqual("This is _italic_ text",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "This is " },
                    new ItalicTextInline().AddChildren(
                        new TextRunInline { Text = "italic" }),
                    new TextRunInline { Text = " text" }));
        }

        [UITestMethod]
        public void Italic_Inside_Word()
        {
            AssertEqual("before*middle*end",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" },
                    new ItalicTextInline().AddChildren(
                        new TextRunInline { Text = "middle" }),
                    new TextRunInline { Text = "end" }));
        }

        [UITestMethod]
        public void Italic_MultiLine()
        {
            // Does work across lines.
            AssertEqual(CollapseWhitespace(@"
                italics *does  
                work* across line breaks"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "italics " },
                    new ItalicTextInline().AddChildren(
                        new TextRunInline { Text = "does\r\nwork" },
                    new TextRunInline { Text = " across line breaks" })));
        }

        [UITestMethod]
        public void Italic_Negative_1()
        {
            AssertEqual("before* middle *end",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before* middle *end" }));
        }

        [UITestMethod]
        public void Italic_Negative_2()
        {
            AssertEqual("before* middle*end",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before* middle*end" }));
        }

        [UITestMethod]
        public void Italic_Negative_3()
        {
            // There must be a valid end italics marker otherwise the whole thing is ignored.
            AssertEqual("This is *not italics * text",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "This is *not italics * text" }));
        }

        [UITestMethod]
        public void Italic_Negative_MultiParagraph()
        {
            // Doesn't work across paragraphs.
            AssertEqual(CollapseWhitespace(@"
                italics *doesn't

                apply* across paragraphs"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "italics *doesn't" }),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "apply* across paragraphs" }));
        }
    }
}
