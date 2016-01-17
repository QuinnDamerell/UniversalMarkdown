// Copyright (c) 2016 Quinn Damerell
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Text;
using UniversalMarkdown.Interfaces;
using UniversalMarkdown.Parse;
using UniversalMarkdown.Parse.Elements;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace UniversalMarkdown.Display
{
    public class XamlRenderer
    {
        /// <summary>
        /// An interface that is used to register hyperlinks.
        /// </summary>
        ILinkRegister m_linkRegister;

        public XamlRenderer(ILinkRegister linkRegister)
        {
            m_linkRegister = linkRegister;
        }


        /// <summary>
        /// Gets or sets a brush that provides the background of the control.
        /// </summary>
        public Brush Background { get; set; }

        /// <summary>
        /// Gets or sets a brush that describes the border fill of a control.
        /// </summary>
        public Brush BorderBrush { get; set; }

        /// <summary>
        /// Gets or sets the border thickness of a control.
        /// </summary>
        public Thickness BorderThickness { get; set; }

        /// <summary>
        /// Gets or sets the uniform spacing between characters, in units of 1/1000 of an em.
        /// </summary>
        public int CharacterSpacing { get; set; }

        /// <summary>
        /// Gets or sets the font used to display text in the control.
        /// </summary>
        public FontFamily FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the size of the text in this control.
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// Gets or sets the degree to which a font is condensed or expanded on the screen.
        /// </summary>
        public FontStretch FontStretch { get; set; }

        /// <summary>
        /// Gets or sets the style in which the text is rendered.
        /// </summary>
        public FontStyle FontStyle { get; set; }

        /// <summary>
        /// Gets or sets the thickness of the specified font.
        /// </summary>
        public FontWeight FontWeight { get; set; }

        /// <summary>
        /// Gets or sets a brush that describes the foreground color.
        /// </summary>
        public Brush Foreground { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment of the control's content.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether text selection is enabled.
        /// </summary>
        public bool IsTextSelectionEnabled { get; set; }

        /// <summary>
        /// Gets or sets the distance between the border and its child object.
        /// </summary>
        public Thickness Padding { get; set; }

        /// <summary>
        /// Gets or sets the brush used to fill the background of a code block.
        /// </summary>
        public Brush CodeBackground { get; set; }

        /// <summary>
        /// Gets or sets the brush used to render the border fill of a code block.
        /// </summary>
        public Brush CodeBorderBrush { get; set; }

        /// <summary>
        /// Gets or sets the thickness of the border around code blocks.
        /// </summary>
        public Thickness CodeBorderThickness { get; set; }

        /// <summary>
        /// Gets or sets the brush used to render the text inside a code block.  If this is
        /// <c>null</c>, then <see cref="Foreground"/> is used.
        /// </summary>
        public Brush CodeForeground { get; set; }

        /// <summary>
        /// Gets or sets the font used to display code.  If this is <c>null</c>, then
        /// <see cref="FontFamily"/> is used.
        /// </summary>
        public FontFamily CodeFontFamily { get; set; } = new FontFamily("Consolas");

        /// <summary>
        /// The padding inside of code blocks.
        /// </summary>
        public Thickness CodePadding { get; set; } = new Thickness(9, 4, 9, 4);

        /// <summary>
        /// The margin used for paragraphs and most other block types.
        /// </summary>
        public Thickness DefaultMargin { get; set; } = new Thickness(0, 5, 0, 5);

        /// <summary>
        /// Gets or sets the brush used to render a horizontal rule.  If this is <c>null</c>, then
        /// <see cref="Foreground"/> is used.
        /// </summary>
        public Brush HorizontalRuleBrush { get; set; }

        /// <summary>
        /// The margin used for horizontal rules.
        /// </summary>
        public Thickness HorizontalRuleMargin { get; set; } = new Thickness(0, 7, 0, 7);

        /// <summary>
        /// Gets or sets the vertical thickness of the horizontal rule.
        /// </summary>
        public double HorizontalRuleThickness { get; set; } = 2;

        /// <summary>
        /// Gets or sets the brush used to render a quote border.  If this is <c>null</c>, then
        /// <see cref="Foreground"/> is used.
        /// </summary>
        public Brush QuoteBorderBrush { get; set; }

        /// <summary>
        /// Gets or sets the horizontal thickness of quote borders.
        /// </summary>
        public double QuoteBorderThickness { get; set; } = 2;

        /// <summary>
        /// Gets or sets the brush used to render table borders.  If this is <c>null</c>, then
        /// <see cref="Foreground"/> is used.
        /// </summary>
        public Brush TableBorderBrush { get; set; }

        /// <summary>
        /// Gets or sets the thickness of any table borders.
        /// </summary>
        public double TableBorderThickness { get; set; } = 1;

        /// <summary>
        /// The padding inside each cell.
        /// </summary>
        public Thickness TableCellPadding { get; set; } = new Thickness(9, 4, 9, 4);



        /// <summary>
        /// Called externally to render markdown to a text block.
        /// </summary>
        /// <param name="markdownTree"></param>
        /// <returns> A XAML UI element. </returns>
        public UIElement Render(Markdown markdownTree)
        {
            var stackPanel = new StackPanel();
            RenderBlocks(markdownTree.Blocks, stackPanel.Children);

            // Set background and border properties.
            stackPanel.Background = Background;
            stackPanel.BorderBrush = BorderBrush;
            stackPanel.BorderThickness = BorderThickness;
            stackPanel.Padding = Padding;

            return stackPanel;
        }

        #region Render Blocks

        /// <summary>
        /// Renders a list of block elements.
        /// </summary>
        /// <param name="blockElements"></param>
        /// <param name="blockUIElementCollection"></param>
        private void RenderBlocks(IEnumerable<MarkdownBlock> blockElements, UIElementCollection blockUIElementCollection)
        {
            foreach (MarkdownBlock element in blockElements)
            {
                RenderBlock(element, blockUIElementCollection);
            }

            // Remove the top margin from the first block element, the bottom margin from the last block element,
            // and collapse adjacent margins.
            FrameworkElement previousFrameworkElement = null;
            for (int i = 0; i < blockUIElementCollection.Count; i++)
            {
                var frameworkElement = blockUIElementCollection[i] as FrameworkElement;
                if (frameworkElement != null)
                {
                    if (i == 0)
                    {
                        // Remove the top margin.
                        frameworkElement.Margin = new Thickness(
                            frameworkElement.Margin.Left,
                            0,
                            frameworkElement.Margin.Right,
                            frameworkElement.Margin.Bottom);
                    }
                    else if (previousFrameworkElement != null)
                    {
                        // Remove the bottom margin.
                        frameworkElement.Margin = new Thickness(
                            frameworkElement.Margin.Left,
                            Math.Max(frameworkElement.Margin.Top, previousFrameworkElement.Margin.Bottom),
                            frameworkElement.Margin.Right,
                            frameworkElement.Margin.Bottom);
                        previousFrameworkElement.Margin = new Thickness(
                            previousFrameworkElement.Margin.Left,
                            previousFrameworkElement.Margin.Top,
                            previousFrameworkElement.Margin.Right,
                            0);

                    }
                }
                previousFrameworkElement = frameworkElement;
            }
        }

        /// <summary>
        /// Called to render a block element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        private void RenderBlock(MarkdownBlock element, UIElementCollection blockUIElementCollection)
        {
            switch (element.Type)
            {
                case MarkdownBlockType.Paragraph:
                    RenderParagraph((ParagraphBlock)element, blockUIElementCollection);
                    break;
                case MarkdownBlockType.Quote:
                    RenderQuote((QuoteBlock)element, blockUIElementCollection);
                    break;
                case MarkdownBlockType.Code:
                    RenderCode((CodeBlock)element, blockUIElementCollection);
                    break;
                case MarkdownBlockType.Header:
                    RenderHeader((HeaderBlock)element, blockUIElementCollection);
                    break;
                case MarkdownBlockType.List:
                    RenderListElement((ListBlock)element, blockUIElementCollection);
                    break;
                case MarkdownBlockType.HorizontalRule:
                    RenderHorizontalRule((HorizontalRuleBlock)element, blockUIElementCollection);
                    break;
                case MarkdownBlockType.Table:
                    RenderTable((TableBlock)element, blockUIElementCollection);
                    break;
            }
        }

        /// <summary>
        /// Renders a paragraph element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        private void RenderParagraph(ParagraphBlock element, UIElementCollection blockUIElementCollection)
        {
            var textBlock = CreateOrReuseRichTextBlock(blockUIElementCollection);

            var paragraph = new Paragraph();
            paragraph.Margin = DefaultMargin;
            RenderInlineChildren(element.Inlines, paragraph.Inlines, paragraph);
            textBlock.Blocks.Add(paragraph);
        }

        /// <summary>
        /// Renders a header element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        private void RenderHeader(HeaderBlock element, UIElementCollection blockUIElementCollection)
        {
            var textBlock = CreateOrReuseRichTextBlock(blockUIElementCollection);

            var paragraph = new Paragraph();
            var childInlines = paragraph.Inlines;
            switch (element.HeaderLevel)
            {
                case 1:
                    paragraph.Margin = new Thickness(0, 15, 0, 15);
                    paragraph.FontSize = 20.0;
                    paragraph.FontWeight = FontWeights.Bold;
                    break;
                case 2:
                    paragraph.Margin = new Thickness(0, 15, 0, 15);
                    paragraph.FontSize = 20.0;
                    break;
                case 3:
                    paragraph.Margin = new Thickness(0, 10, 0, 10);
                    paragraph.FontSize = 17.0;
                    paragraph.FontWeight = FontWeights.Bold;
                    break;
                case 4:
                    paragraph.Margin = new Thickness(0, 10, 0, 10);
                    paragraph.FontSize = 17.0;
                    break;
                case 5:
                    paragraph.Margin = new Thickness(0, 10, 0, 5);
                    paragraph.FontWeight = FontWeights.Bold;
                    break;
                case 6:
                    paragraph.Margin = new Thickness(0, 10, 0, 0);
                    var underline = new Underline();
                    childInlines = underline.Inlines;
                    paragraph.Inlines.Add(underline);
                    break;
            }

            // Render the children into the para inline.
            RenderInlineChildren(element.Inlines, childInlines, paragraph);

            // Add it to the blocks
            textBlock.Blocks.Add(paragraph);
        }

        /// <summary>
        /// Renders a list element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        private void RenderListElement(ListBlock element, UIElementCollection blockUIElementCollection)
        {
            // Create a grid with two columns.
            Grid grid = new Grid();
            grid.Margin = DefaultMargin;

            // The first column for the bullet (or number) and the second for the text.
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            for (int rowIndex = 0; rowIndex < element.Items.Count; rowIndex ++)
            {
                var listItem = element.Items[rowIndex];

                // Add a row definition.
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                // Add the bullet or number.
                var bullet = CreateTextBlock();
                switch (element.Style)
                {
                    case ListStyle.Bulleted:
                        bullet.Text = "•";
                        break;
                    case ListStyle.Numbered:
                        bullet.Text = $"{rowIndex + 1}.";
                        break;
                }
                bullet.HorizontalAlignment = HorizontalAlignment.Right;
                bullet.Margin = new Thickness(0, 0, 12, 0);
                Grid.SetRow(bullet, rowIndex);
                grid.Children.Add(bullet);

                // Add the list item content.
                var content = new StackPanel();
                RenderBlocks(listItem.Blocks, content.Children);
                Grid.SetColumn(content, 1);
                Grid.SetRow(content, rowIndex);
                grid.Children.Add(content);
            }

            blockUIElementCollection.Add(grid);
        }

        /// <summary>
        /// Renders a horizontal rule element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        private void RenderHorizontalRule(HorizontalRuleBlock element, UIElementCollection blockUIElementCollection)
        {
            var rectangle = new Rectangle();
            rectangle.HorizontalAlignment = HorizontalAlignment.Stretch;
            rectangle.Height = HorizontalRuleThickness;
            rectangle.Fill = HorizontalRuleBrush ?? Foreground;
            rectangle.Margin = HorizontalRuleMargin;

            blockUIElementCollection.Add(rectangle);
        }

        /// <summary>
        /// Renders a quote element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        private void RenderQuote(QuoteBlock element, UIElementCollection blockUIElementCollection)
        {
            var stackPanel = new StackPanel();
            RenderBlocks(element.Blocks, stackPanel.Children);

            var border = new Border();
            border.Margin = new Thickness(DefaultMargin.Left + 12, DefaultMargin.Top, DefaultMargin.Right, DefaultMargin.Bottom);
            border.BorderBrush = QuoteBorderBrush ?? Foreground;
            border.BorderThickness = new Thickness(QuoteBorderThickness, 0, 0, 0);
            border.Padding = new Thickness(12, 0, 0, 0);
            border.Child = stackPanel;

            blockUIElementCollection.Add(border);
        }


        /// <summary>
        /// Renders a code element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        private void RenderCode(CodeBlock element, UIElementCollection blockUIElementCollection)
        {
            var textBlock = CreateTextBlock();
            textBlock.Margin = new Thickness(DefaultMargin.Left, 0, DefaultMargin.Right, 0);
            textBlock.FontFamily = CodeFontFamily ?? FontFamily;
            textBlock.Foreground = CodeForeground ?? Foreground;
            textBlock.Text = element.Text;

            var border = new Border();
            border.Background = CodeBackground;
            border.BorderBrush = CodeBorderBrush;
            border.BorderThickness = CodeBorderThickness;
            border.Padding = CodePadding;
            border.Margin = DefaultMargin;
            border.Child = textBlock;

            // Add it to the blocks
            blockUIElementCollection.Add(border);
        }

        /// <summary>
        /// Renders a table element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        private void RenderTable(TableBlock element, UIElementCollection blockUIElementCollection)
        {
            var table = new MarkdownTable(element.ColumnDefinitions.Count, element.Rows.Count, TableBorderThickness, TableBorderBrush);
            table.HorizontalAlignment = HorizontalAlignment.Left;
            table.Margin = DefaultMargin;

            // Add each row.
            for (int rowIndex = 0; rowIndex < element.Rows.Count; rowIndex++)
            {
                var row = element.Rows[rowIndex];

                // Add each cell.
                for (int cellIndex = 0; cellIndex < Math.Min(element.ColumnDefinitions.Count, row.Cells.Count); cellIndex++)
                {
                    var cell = row.Cells[cellIndex];

                    // Cell content.
                    var cellContent = CreateOrReuseRichTextBlock(null);
                    cellContent.Margin = TableCellPadding;
                    Grid.SetRow(cellContent, rowIndex);
                    Grid.SetColumn(cellContent, cellIndex);
                    switch (element.ColumnDefinitions[cellIndex].Alignment)
                    {
                        case ColumnAlignment.Center:
                            cellContent.TextAlignment = TextAlignment.Center;
                            break;
                        case ColumnAlignment.Right:
                            cellContent.TextAlignment = TextAlignment.Right;
                            break;
                    }
                    if (rowIndex == 0)
                        cellContent.FontWeight = FontWeights.Bold;
                    var paragraph = new Paragraph();
                    RenderInlineChildren(cell.Inlines, paragraph.Inlines, paragraph);
                    cellContent.Blocks.Add(paragraph);
                    table.Children.Add(cellContent);
                }
            }

            blockUIElementCollection.Add(table);
        }

        #endregion

        #region Render Inlines

        // Helper class for holding persistent state.
        private class RenderContext
        {
            public Paragraph ParentParagraph;
            public bool TrimLeadingWhitespace;
        }

        /// <summary>
        /// Renders all of the children for the given element.
        /// </summary>
        /// <param name="inlineElements"> The inline elements to render. </param>
        /// <param name="currentInlines"> The list to add to. </param>
        /// <param name="parentParagraph"> The parent Paragraph. </param>
        private void RenderInlineChildren(IList<MarkdownInline> inlineElements, InlineCollection currentInlines, Paragraph parentParagraph)
        {
            RenderInlineChildren(inlineElements, currentInlines, new RenderContext { ParentParagraph = parentParagraph, TrimLeadingWhitespace = true });
        }

        /// <summary>
        /// Renders all of the children for the given element.
        /// </summary>
        /// <param name="inlineElements"> The inline elements to render. </param>
        /// <param name="currentInlines"> The list to add to. </param>
        /// <param name="context"> The parent block. </param>
        private void RenderInlineChildren(IList<MarkdownInline> inlineElements, InlineCollection currentInlines, RenderContext context)
        {
            foreach (MarkdownInline element in inlineElements)
            {
                RenderInline(element, currentInlines, context);
            }
        }

        /// <summary>
        /// Called to render an inline element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="context"></param>
        private void RenderInline(MarkdownInline element, InlineCollection currentInlines, RenderContext context)
        {
            switch (element.Type)
            {
                case MarkdownInlineType.TextRun:
                    RenderTextRun((TextRunInline)element, currentInlines, context);
                    break;
                case MarkdownInlineType.Italic:
                    RenderItalicRun((ItalicTextInline)element, currentInlines, context);
                    break;
                case MarkdownInlineType.Bold:
                    RenderBoldRun((BoldTextInline)element, currentInlines, context);
                    break;
                case MarkdownInlineType.MarkdownLink:
                    RenderMarkdownLink((MarkdownLinkInline)element, currentInlines, context);
                    break;
                case MarkdownInlineType.RawHyperlink:
                    RenderRawHyperlink((RawHyperlinkInline)element, currentInlines, context);
                    break;
                case MarkdownInlineType.RawSubreddit:
                    RenderRawSubreddit((RawSubredditInline)element, currentInlines, context);
                    break;
                case MarkdownInlineType.Strikethrough:
                    RenderStrikethroughRun((StrikethroughTextInline)element, currentInlines, context);
                    break;
                case MarkdownInlineType.Superscript:
                    RenderSuperscriptRun((SuperscriptTextInline)element, currentInlines, context);
                    break;
                case MarkdownInlineType.Code:
                    RenderCodeRun((CodeInline)element, currentInlines, context);
                    break;
            }
        }

        /// <summary>
        /// Renders a text run element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="context"></param>
        private void RenderTextRun(TextRunInline element, InlineCollection currentInlines, RenderContext context)
        {
            // Create the text run
            Run textRun = new Run();
            textRun.Text = CollapseWhitespace(context, element.Text);

            // Add it
            currentInlines.Add(textRun);
        }

        /// <summary>
        /// Renders a bold run element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="context"></param>
        private void RenderBoldRun(BoldTextInline element, InlineCollection currentInlines, RenderContext context)
        {
            // Create the text run
            Span boldSpan = new Span();
            boldSpan.FontWeight = FontWeights.Bold;

            // Render the children into the bold inline.
            RenderInlineChildren(element.Inlines, boldSpan.Inlines, context);

            // Add it to the current inlines
            currentInlines.Add(boldSpan);
        }

        /// <summary>
        /// Renders a link element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="context"></param>
        private void RenderMarkdownLink(MarkdownLinkInline element, InlineCollection currentInlines, RenderContext context)
        {
            // Create the text run
            Hyperlink link = new Hyperlink();

            // Register the link
            m_linkRegister.RegisterNewHyperLink(link, element.Url);

            // Render the children into the link inline.
            RenderInlineChildren(element.Inlines, link.Inlines, context);

            // Add it to the current inlines
            currentInlines.Add(link);
        }

        /// <summary>
        /// Renders a raw link element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="context"></param>
        private void RenderRawHyperlink(RawHyperlinkInline element, InlineCollection currentInlines, RenderContext context)
        {
            // Create the text run
            Hyperlink link = new Hyperlink();

            // Register the link
            m_linkRegister.RegisterNewHyperLink(link, element.Url);

            // Make a text block for the link
            Run linkText = new Run();
            linkText.Text = CollapseWhitespace(context, element.Url);
            link.Inlines.Add(linkText);

            // Add it to the current inlines
            currentInlines.Add(link);
        }

        /// <summary>
        /// Renders a raw subreddit element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="context"></param>
        private void RenderRawSubreddit(RawSubredditInline element, InlineCollection currentInlines, RenderContext context)
        {
            // Create the hyper link
            Hyperlink link = new Hyperlink();

            // Register the link
            m_linkRegister.RegisterNewHyperLink(link, element.Text);

            // Add the subreddit text
            Run subreddit = new Run();
            subreddit.Text = CollapseWhitespace(context, element.Text);
            link.Inlines.Add(subreddit);

            // Add it to the current inlines
            currentInlines.Add(link);
        }

        /// <summary>
        /// Renders a text run element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="context"></param>
        private void RenderItalicRun(ItalicTextInline element, InlineCollection currentInlines, RenderContext context)
        {
            // Create the text run
            Span italicSpan = new Span();
            italicSpan.FontStyle = FontStyle.Italic;

            // Render the children into the italic inline.
            RenderInlineChildren(element.Inlines, italicSpan.Inlines, context);

            // Add it to the current inlines
            currentInlines.Add(italicSpan);
        }

        /// <summary>
        /// Renders a strikethrough element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="context"></param>
        private void RenderStrikethroughRun(StrikethroughTextInline element, InlineCollection currentInlines, RenderContext context)
        {
            Span span = new Span();
            span.FontFamily = new FontFamily("Consolas");

            // Render the children into the inline.
            RenderInlineChildren(element.Inlines, span.Inlines, context);

            AlterChildRuns(span, (parentSpan, run) =>
            {
                var text = run.Text;
                var builder = new StringBuilder(text.Length * 2);
                foreach (var c in text)
                {
                    builder.Append((char)0x0336);
                    builder.Append(c);
                }
                run.Text = builder.ToString();
            });

            // Add it to the current inlines
            currentInlines.Add(span);
        }

        /// <summary>
        /// Renders a superscript element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="context"></param>
        private void RenderSuperscriptRun(SuperscriptTextInline element, InlineCollection currentInlines, RenderContext context)
        {
            var paragraph = new Paragraph();
            paragraph.FontSize = context.ParentParagraph.FontSize * 0.8;
            RenderInlineChildren(element.Inlines, paragraph.Inlines, paragraph);

            var richTextBlock = CreateOrReuseRichTextBlock(null);
            richTextBlock.Blocks.Add(paragraph);

            var border = new Border();
            border.Padding = new Thickness(0, 0, 0, paragraph.FontSize * 0.2);
            border.Child = richTextBlock;

            var inlineUIContainer = new InlineUIContainer();
            inlineUIContainer.Child = border;

            // Add it to the current inlines
            currentInlines.Add(inlineUIContainer);
        }

        /// <summary>
        /// Renders a code element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="context"></param>
        private void RenderCodeRun(CodeInline element, InlineCollection currentInlines, RenderContext context)
        {
            var run = new Run();
            run.FontFamily = CodeFontFamily ?? FontFamily;
            run.Text = CollapseWhitespace(context, element.Text);

            // Add it to the current inlines
            currentInlines.Add(run);
        }

        /// <summary>
        /// Performs an action against any runs that occur within the given span.
        /// </summary>
        /// <param name="parentSpan"></param>
        /// <param name="action"></param>
        private void AlterChildRuns(Span parentSpan, Action<Span, Run> action)
        {
            foreach (var inlineElement in parentSpan.Inlines)
            {
                if (inlineElement is Span)
                    AlterChildRuns((Span)inlineElement, action);
                else if (inlineElement is Run)
                    action(parentSpan, (Run)inlineElement);
            }
        }

        /// <summary>
        /// Removes leading whitespace, but only if this is the first run in the block.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private string CollapseWhitespace(RenderContext context, string text)
        {
            bool dontOutputWhitespace = context.TrimLeadingWhitespace;
            StringBuilder result = null;
            for (int i = 0; i < text.Length; i ++)
            {
                char c = text[i];
                if (c == ' ' || c == '\t')
                {
                    if (dontOutputWhitespace == true)
                    {
                        if (result == null)
                            result = new StringBuilder(text.Substring(0, i), text.Length);
                    }
                    else
                    {
                        if (result != null)
                            result.Append(c);
                        dontOutputWhitespace = true;
                    }
                }
                else
                {
                    if (result != null)
                        result.Append(c);
                    dontOutputWhitespace = false;
                }
            }
            context.TrimLeadingWhitespace = false;
            return result == null ? text : result.ToString();
        }

        /// <summary>
        /// Creates a new RichTextBlock, if the last element of the provided collection isn't already a RichTextBlock.
        /// </summary>
        /// <param name="blockUIElementCollection"></param>
        /// <returns></returns>
        private RichTextBlock CreateOrReuseRichTextBlock(UIElementCollection blockUIElementCollection)
        {
            // Reuse the last RichTextBlock, if possible.
            if (blockUIElementCollection != null && blockUIElementCollection.Count > 0 && blockUIElementCollection[blockUIElementCollection.Count - 1] is RichTextBlock)
                return (RichTextBlock)blockUIElementCollection[blockUIElementCollection.Count - 1];

            var result = new RichTextBlock();
            result.CharacterSpacing = CharacterSpacing;
            result.FontFamily = FontFamily;
            result.FontSize = FontSize;
            result.FontStretch = FontStretch;
            result.FontStyle = FontStyle;
            result.FontWeight = FontWeight;
            result.Foreground = Foreground;
            result.HorizontalAlignment = HorizontalAlignment;
            result.IsTextSelectionEnabled = IsTextSelectionEnabled;
            result.TextWrapping = TextWrapping.Wrap;
            if (blockUIElementCollection != null)
                blockUIElementCollection.Add(result);
            return result;
        }

        /// <summary>
        /// Creates a new TextBlock, with default settings.
        /// </summary>
        /// <returns></returns>
        private TextBlock CreateTextBlock()
        {
            var result = new TextBlock();
            result.CharacterSpacing = CharacterSpacing;
            result.FontFamily = FontFamily;
            result.FontSize = FontSize;
            result.FontStretch = FontStretch;
            result.FontStyle = FontStyle;
            result.FontWeight = FontWeight;
            result.Foreground = Foreground;
            result.HorizontalAlignment = HorizontalAlignment;
            result.IsTextSelectionEnabled = IsTextSelectionEnabled;
            result.Margin = DefaultMargin;
            result.TextWrapping = TextWrapping.Wrap;
            return result;
        }

        #endregion
    }
}
