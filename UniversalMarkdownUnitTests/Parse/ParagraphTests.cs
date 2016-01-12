using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UniversalMarkdown.Parse;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class ParagraphTests : ParseTestBase
    {
        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_Empty()
        {
            AssertEqual("", new MarkdownBlock[0]);
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_NoLineBreak()
        {
            // A line break in the markup does not translate to a line break in the resulting formatted text.
            AssertEqual(CollapseWhitespace(@"
                line 1
                line 2"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 1 line 2" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_NoLineBreak_OneSpace()
        {
            // A line break in the markup does not translate to a line break in the resulting formatted text.
            AssertEqual(CollapseWhitespace(@"
                line 1 
                line 2"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 1 line 2" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_LineBreak()
        {
            // Two spaces at the end of the line results in a line break.
            AssertEqual(CollapseWhitespace(@"
                line 1  
                line 2"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 1 \r\nline 2" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_LineBreak_ThreeSpaces()
        {
            // Three spaces at the end of the line also results in a line break.
            AssertEqual(CollapseWhitespace(@"
                line 1   
                line 2"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 1 \r\nline 2" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_NewParagraph()
        {
            // An empty line starts a new paragraph.
            AssertEqual(CollapseWhitespace(@"
                line 1

                line 2"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 1" }),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 2" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_NewParagraph_Whitespace()
        {
            // A line that contains only whitespace starts a new paragraph.
            AssertEqual(CollapseWhitespace(@"
                line 1
                      
                line 2"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 1" }),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 2" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_SpaceCompression()
        {
            // Multiple spaces are collapsed into one.
            AssertEqual("one      two",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "one two" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_TextEscaping()
        {
            // The list of escape characters from daring fireball:
            // \   backslash
            // `   backtick
            // *   asterisk
            // _   underscore
            // { } curly braces
            // []  square brackets
            // ()  parentheses
            // #   hash mark
            // +   plus sign
            // -   minus sign(hyphen)
            // .   dot
            // !   exclamation mark

            AssertEqual(@"\~\`\!\@\#\$\%\^\&\*\(\)\_\+\-\=\{\}\|\[\]\\\:\""\;\'\<\>\?\,\.\/\a\A\1",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = @"~`!\@#\$\%^&*()_+-\={}|[]\:\""\;\'<>\?\,./\a\A\1" }));
        }
    }
}
