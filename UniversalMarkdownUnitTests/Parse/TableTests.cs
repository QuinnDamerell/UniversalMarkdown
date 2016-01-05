using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.Generic;
using UniversalMarkdown.Parse.Elements;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Parse
{
    [TestClass]
    public class TableTests : ParseTestBase
    {
        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Table_Simple()
        {
            AssertEqual(CollapseWhitespace(@"
                | Column 1 | Column 2 | Column 3 |
                |----------|----------|----------|
                | A        | B        | C        |"),
                new TableBlock
                {
                    ColumnDefinitions = new List<TableColumnDefinition>
                    {
                        new TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                        new TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                        new TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                    }
                }.AddChildren(
                    new TableRow().AddChildren(
                        new TextRunInline { Text = "Column 1" },
                        new TextRunInline { Text = "Column 2" },
                        new TextRunInline { Text = "Column 3" }),
                    new TableRow().AddChildren(
                        new TextRunInline { Text = "A" },
                        new TextRunInline { Text = "B" },
                        new TextRunInline { Text = "C" })));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Table_WithAlignment()
        {
            AssertEqual(CollapseWhitespace(@"
                | Column 1   | Column 2    | Column 3     |
                |:-----------|------------:|:------------:|
                | You        |          You|     You     
                  can align  |    can align|  can align   |
                | left       |        right|   center     "),
                new TableBlock
                {
                    ColumnDefinitions = new List<TableColumnDefinition>
                    {
                        new TableColumnDefinition { Alignment = ColumnAlignment.Left },
                        new TableColumnDefinition { Alignment = ColumnAlignment.Right },
                        new TableColumnDefinition { Alignment = ColumnAlignment.Center },
                    }
                }.AddChildren(
                    new TableRow().AddChildren(
                        new TextRunInline { Text = "Column 1" },
                        new TextRunInline { Text = "Column 2" },
                        new TextRunInline { Text = "Column 3" }),
                    new TableRow().AddChildren(
                        new TextRunInline { Text = "You" },
                        new TextRunInline { Text = "You" },
                        new TextRunInline { Text = "You" }),
                    new TableRow().AddChildren(
                        new TextRunInline { Text = "can align" },
                        new TextRunInline { Text = "can align" },
                        new TextRunInline { Text = "can align" }),
                    new TableRow().AddChildren(
                        new TextRunInline { Text = "left" },
                        new TextRunInline { Text = "right" },
                        new TextRunInline { Text = "center" })));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Table_Dividers()
        {
            // Too many column dividers is okay.
            AssertEqual(CollapseWhitespace(@"
                        Column A | Column B | Column C
                        -|-|-|-
                        A1 | B1 | C1"),
                new TableBlock
                {
                    ColumnDefinitions = new List<TableColumnDefinition>
                    {
                        new TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                        new TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                        new TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                    }
                }.AddChildren(
                        new TableRow().AddChildren(
                            new TextRunInline { Text = "Column A" },
                            new TextRunInline { Text = "Column B" },
                            new TextRunInline { Text = "Column C" }),
                        new TableRow().AddChildren(
                            new TextRunInline { Text = "A1" },
                            new TextRunInline { Text = "B1" },
                            new TextRunInline { Text = "C1" })));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Table_Minimal_1()
        {
            AssertEqual(CollapseWhitespace(@"
                |c
                -"),
                new TableBlock
                {
                    ColumnDefinitions = new List<TableColumnDefinition>
                    {
                        new TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                    }
                }.AddChildren(
                    new TableRow().AddChildren(
                        new TextRunInline { Text = "c" })));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Table_Minimal_2()
        {
            AssertEqual(CollapseWhitespace(@"
                c|
                -"),
                new TableBlock
                {
                    ColumnDefinitions = new List<TableColumnDefinition>
                    {
                        new TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                    }
                }.AddChildren(
                    new TableRow().AddChildren(
                        new TextRunInline { Text = "c" })));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Table_Minimal_3()
        {
            AssertEqual(CollapseWhitespace(@"
                a|b
                -|-"),
                new TableBlock
                {
                    ColumnDefinitions = new List<TableColumnDefinition>
                    {
                        new TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                        new TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                    }
                }.AddChildren(
                    new TableRow().AddChildren(
                        new TextRunInline { Text = "a" },
                        new TextRunInline { Text = "b" })));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Table_Minimal_4()
        {
            AssertEqual(CollapseWhitespace(@"
                a|b
                :|:"),
                new TableBlock
                {
                    ColumnDefinitions = new List<TableColumnDefinition>
                    {
                        new TableColumnDefinition { Alignment = ColumnAlignment.Left },
                        new TableColumnDefinition { Alignment = ColumnAlignment.Left },
                    }
                }.AddChildren(
                    new TableRow().AddChildren(
                        new TextRunInline { Text = "a" },
                        new TextRunInline { Text = "b" })));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Table_PrematureEnding()
        {
            AssertEqual(CollapseWhitespace(@"
                a|b
                -|-
                A|B
                test"),
                new TableBlock
                {
                    ColumnDefinitions = new List<TableColumnDefinition>
                    {
                        new TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                        new TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                    }
                }.AddChildren(
                    new TableRow().AddChildren(
                        new TextRunInline { Text = "a" },
                        new TextRunInline { Text = "b" }),
                    new TableRow().AddChildren(
                        new TextRunInline { Text = "A" },
                        new TextRunInline { Text = "B" })),
                new ParagraphBlock().AddChildren(new TextRunInline { Text = "test" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Table_Negative_NewParagraph()
        {
            // Must start on a new paragraph.
            AssertEqual(CollapseWhitespace(@"
                before
                | Column 1 | Column 2 | Column 3 |
                |----------|----------|----------|
                | A        | B        | C        |
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before | Column 1 | Column 2 | Column 3 | |----------|----------|----------| | A | B | C | after" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Table_Negative_TooFewDividers()
        {
            // Too few dividers doesn't work.
            AssertEqual(CollapseWhitespace(@"
                Column A | Column B | Column C
                -|-
                A1 | B1 | C1"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "Column A | Column B | Column C -|- A1 | B1 | C1" }));
        }

        [UITestMethod]
        [TestCategory("Parse - block")]
        public void Table_Negative_MissingDashes()
        {
            // The dashes are normally required
            AssertEqual(CollapseWhitespace(@"
                Column A | Column B | Column C
                ||
                A1 | B1 | C1"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "Column A | Column B | Column C || A1 | B1 | C1" }));
        }

    }
}
