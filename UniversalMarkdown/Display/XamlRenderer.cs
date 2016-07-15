﻿// Copyright (c) 2016 Quinn Damerell
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
using UniversalMarkdown.Helpers;
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
    internal class XamlRenderer
    {
        /// <summary>
        /// The markdown document that will be rendered.
        /// </summary>
        private MarkdownDocument document;

        /// <summary>
        /// An interface that is used to register hyperlinks.
        /// </summary>
        private ILinkRegister linkRegister;

        public XamlRenderer(MarkdownDocument document, ILinkRegister linkRegister)
        {
            this.document = document;
            this.linkRegister = linkRegister;
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
        public FontFamily CodeFontFamily { get; set; }

        /// <summary>
        /// The space outside of code blocks.
        /// </summary>
        public Thickness CodeMargin { get; set; }

        /// <summary>
        /// The space between the code border and the text.
        /// </summary>
        public Thickness CodePadding { get; set; }

        /// <summary>
        /// Gets or sets the font weight to use for level 1 headers.
        /// </summary>
        public FontWeight Header1FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font size for level 1 headers.
        /// </summary>
        public double Header1FontSize { get; set; }

        /// <summary>
        /// Gets or sets the margin for level 1 headers.
        /// </summary>
        public Thickness Header1Margin { get; set; }

        /// <summary>
        /// Gets or sets the font weight to use for level 2 headers.
        /// </summary>
        public FontWeight Header2FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font size for level 2 headers.
        /// </summary>
        public double Header2FontSize { get; set; }

        /// <summary>
        /// Gets or sets the margin for level 2 headers.
        /// </summary>
        public Thickness Header2Margin { get; set; }

        /// <summary>
        /// Gets or sets the font weight to use for level 3 headers.
        /// </summary>
        public FontWeight Header3FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font size for level 3 headers.
        /// </summary>
        public double Header3FontSize { get; set; }

        /// <summary>
        /// Gets or sets the margin for level 3 headers.
        /// </summary>
        public Thickness Header3Margin { get; set; }

        /// <summary>
        /// Gets or sets the font weight to use for level 4 headers.
        /// </summary>
        public FontWeight Header4FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font size for level 4 headers.
        /// </summary>
        public double Header4FontSize { get; set; }

        /// <summary>
        /// Gets or sets the margin for level 4 headers.
        /// </summary>
        public Thickness Header4Margin { get; set; }

        /// <summary>
        /// Gets or sets the font weight to use for level 5 headers.
        /// </summary>
        public FontWeight Header5FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font size for level 5 headers.
        /// </summary>
        public double Header5FontSize { get; set; }

        /// <summary>
        /// Gets or sets the margin for level 5 headers.
        /// </summary>
        public Thickness Header5Margin { get; set; }

        /// <summary>
        /// Gets or sets the font weight to use for level 6 headers.
        /// </summary>
        public FontWeight Header6FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font size for level 6 headers.
        /// </summary>
        public double Header6FontSize { get; set; }

        /// <summary>
        /// Gets or sets the margin for level 6 headers.
        /// </summary>
        public Thickness Header6Margin { get; set; }

        /// <summary>
        /// Gets or sets the brush used to render a horizontal rule.  If this is <c>null</c>, then
        /// <see cref="Foreground"/> is used.
        /// </summary>
        public Brush HorizontalRuleBrush { get; set; }

        /// <summary>
        /// The margin used for horizontal rules.
        /// </summary>
        public Thickness HorizontalRuleMargin { get; set; }

        /// <summary>
        /// Gets or sets the vertical thickness of the horizontal rule.
        /// </summary>
        public double HorizontalRuleThickness { get; set; }

        /// <summary>
        /// Gets or sets the margin used by lists.
        /// </summary>
        public Thickness ListMargin { get; set; }

        /// <summary>
        /// Gets or sets the width of the space used by list item bullets/numbers.
        /// </summary>
        public double ListGutterWidth { get; set; }

        /// <summary>
        /// Gets or sets the space between the list item bullets/numbers and the list item content.
        /// </summary>
        public double ListBulletSpacing { get; set; }

        /// <summary>
        /// Gets or sets the margin used for paragraphs.
        /// </summary>
        public Thickness ParagraphMargin { get; set; }

        /// <summary>
        /// Gets or sets the brush used to fill the background of a quote block.
        /// </summary>
        public Brush QuoteBackground { get; set; }

        /// <summary>
        /// Gets or sets the brush used to render a quote border.  If this is <c>null</c>, then
        /// <see cref="Foreground"/> is used.
        /// </summary>
        public Brush QuoteBorderBrush { get; set; }

        /// <summary>
        /// Gets or sets the thickness of quote borders.
        /// </summary>
        public Thickness QuoteBorderThickness { get; set; }

        /// <summary>
        /// Gets or sets the brush used to render the text inside a quote block.  If this is
        /// <c>null</c>, then <see cref="Foreground"/> is used.
        /// </summary>
        public Brush QuoteForeground { get; set; }

        /// <summary>
        /// Gets or sets the space outside of quote borders.
        /// </summary>
        public Thickness QuoteMargin { get; set; }

        /// <summary>
        /// Gets or sets the space between the quote border and the text.
        /// </summary>
        public Thickness QuotePadding { get; set; }

        /// <summary>
        /// Gets or sets the brush used to render table borders.  If this is <c>null</c>, then
        /// <see cref="Foreground"/> is used.
        /// </summary>
        public Brush TableBorderBrush { get; set; }

        /// <summary>
        /// Gets or sets the thickness of any table borders.
        /// </summary>
        public double TableBorderThickness { get; set; }

        /// <summary>
        /// The padding inside each cell.
        /// </summary>
        public Thickness TableCellPadding { get; set; }

        /// <summary>
        /// Gets or sets the margin used by tables.
        /// </summary>
        public Thickness TableMargin { get; set; }

        /// <summary>
        /// Gets or sets the word wrapping behavior.
        /// </summary>
        public TextWrapping TextWrapping { get; set; }
        
        /// <summary>
        /// Gets or sets the link foreground.
        /// </summary>
        public Brush LinkForeground { get; set; }


        /// <summary>
        /// Called externally to render markdown to a text block.
        /// </summary>
        /// <returns> A XAML UI element. </returns>
        public UIElement Render()
        {
            var stackPanel = new StackPanel();
            RenderBlocks(this.document.Blocks, stackPanel.Children, new RenderContext { Foreground = Foreground });

            // Set background and border properties.
            stackPanel.Background = Background;
#if WINDOWS_UWP
            stackPanel.BorderBrush = BorderBrush;
            stackPanel.BorderThickness = BorderThickness;
            stackPanel.Padding = Padding;
            return stackPanel;
#else
            var border = new Border();
            border.BorderBrush = BorderBrush;
            border.BorderThickness = BorderThickness;
            stackPanel.Margin = Padding;
            border.Child = stackPanel;
            return border;
#endif
        }

        // Helper class for holding persistent state.
        private class RenderContext
        {
            public Brush Foreground;
            public bool TrimLeadingWhitespace;
            public bool WithinHyperlink;

            public RenderContext Clone()
            {
                return (RenderContext)MemberwiseClone();
            }
        }

        #region Render Blocks

        /// <summary>
        /// Renders a list of block elements.
        /// </summary>
        /// <param name="blockElements"></param>
        /// <param name="blockUIElementCollection"></param>
        /// <param name="context"></param>
        private void RenderBlocks(IEnumerable<MarkdownBlock> blockElements, UIElementCollection blockUIElementCollection, RenderContext context)
        {
            foreach (MarkdownBlock element in blockElements)
            {
                RenderBlock(element, blockUIElementCollection, context);
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
        /// <param name="context"></param>
        private void RenderBlock(MarkdownBlock element, UIElementCollection blockUIElementCollection, RenderContext context)
        {
            switch (element.Type)
            {
                case MarkdownBlockType.Paragraph:
                    RenderParagraph((ParagraphBlock)element, blockUIElementCollection, context);
                    break;
                case MarkdownBlockType.Quote:
                    RenderQuote((QuoteBlock)element, blockUIElementCollection, context);
                    break;
                case MarkdownBlockType.Code:
                    RenderCode((CodeBlock)element, blockUIElementCollection, context);
                    break;
                case MarkdownBlockType.Header:
                    RenderHeader((HeaderBlock)element, blockUIElementCollection, context);
                    break;
                case MarkdownBlockType.List:
                    RenderListElement((ListBlock)element, blockUIElementCollection, context);
                    break;
                case MarkdownBlockType.HorizontalRule:
                    RenderHorizontalRule((HorizontalRuleBlock)element, blockUIElementCollection, context);
                    break;
                case MarkdownBlockType.Table:
                    RenderTable((TableBlock)element, blockUIElementCollection, context);
                    break;
            }
        }

        /// <summary>
        /// Renders a paragraph element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        /// <param name="context"></param>
        private void RenderParagraph(ParagraphBlock element, UIElementCollection blockUIElementCollection, RenderContext context)
        {
            var paragraph = new Paragraph();
            paragraph.Margin = ParagraphMargin;
            context.TrimLeadingWhitespace = true;
            RenderInlineChildren(paragraph.Inlines, element.Inlines, paragraph, context);

            var textBlock = CreateOrReuseRichTextBlock(blockUIElementCollection, context);
            textBlock.Blocks.Add(paragraph);
        }

        /// <summary>
        /// Renders a header element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        /// <param name="context"></param>
        private void RenderHeader(HeaderBlock element, UIElementCollection blockUIElementCollection, RenderContext context)
        {
            var textBlock = CreateOrReuseRichTextBlock(blockUIElementCollection, context);

            var paragraph = new Paragraph();
            var childInlines = paragraph.Inlines;
            switch (element.HeaderLevel)
            {
                case 1:
                    paragraph.Margin = Header1Margin;
                    paragraph.FontSize = Header1FontSize;
                    paragraph.FontWeight = Header1FontWeight;
                    break;
                case 2:
                    paragraph.Margin = Header2Margin;
                    paragraph.FontSize = Header2FontSize;
                    paragraph.FontWeight = Header2FontWeight;
                    break;
                case 3:
                    paragraph.Margin = Header3Margin;
                    paragraph.FontSize = Header3FontSize;
                    paragraph.FontWeight = Header3FontWeight;
                    break;
                case 4:
                    paragraph.Margin = Header4Margin;
                    paragraph.FontSize = Header4FontSize;
                    paragraph.FontWeight = Header4FontWeight;
                    break;
                case 5:
                    paragraph.Margin = Header5Margin;
                    paragraph.FontSize = Header5FontSize;
                    paragraph.FontWeight = Header5FontWeight;
                    break;
                case 6:
                    paragraph.Margin = Header6Margin;
                    paragraph.FontSize = Header6FontSize;
                    paragraph.FontWeight = Header6FontWeight;

                    var underline = new Underline();
                    childInlines = underline.Inlines;
                    paragraph.Inlines.Add(underline);
                    break;
            }

            // Render the children into the para inline.
            context.TrimLeadingWhitespace = true;
            RenderInlineChildren(childInlines, element.Inlines, paragraph, context);

            // Add it to the blocks
            textBlock.Blocks.Add(paragraph);
        }

        /// <summary>
        /// Renders a list element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        /// <param name="context"></param>
        private void RenderListElement(ListBlock element, UIElementCollection blockUIElementCollection, RenderContext context)
        {
            // Create a grid with two columns.
            Grid grid = new Grid();
            grid.Margin = ListMargin;

            // The first column for the bullet (or number) and the second for the text.
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(ListGutterWidth) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            for (int rowIndex = 0; rowIndex < element.Items.Count; rowIndex++)
            {
                var listItem = element.Items[rowIndex];

                // Add a row definition.
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                // Add the bullet or number.
                var bullet = CreateTextBlock(context);
                bullet.Margin = ParagraphMargin;
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
                bullet.Margin = new Thickness(0, 0, ListBulletSpacing, 0);
                Grid.SetRow(bullet, rowIndex);
                grid.Children.Add(bullet);

                // Add the list item content.
                var content = new StackPanel();
                RenderBlocks(listItem.Blocks, content.Children, context);
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
        /// <param name="context"></param>
        private void RenderHorizontalRule(HorizontalRuleBlock element, UIElementCollection blockUIElementCollection, RenderContext context)
        {
            var rectangle = new Rectangle();
            rectangle.HorizontalAlignment = HorizontalAlignment.Stretch;
            rectangle.Height = HorizontalRuleThickness;
            rectangle.Fill = HorizontalRuleBrush ?? context.Foreground;
            rectangle.Margin = HorizontalRuleMargin;

            blockUIElementCollection.Add(rectangle);
        }

        /// <summary>
        /// Renders a quote element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        private void RenderQuote(QuoteBlock element, UIElementCollection blockUIElementCollection, RenderContext context)
        {
            if (QuoteForeground != null)
            {
                context = context.Clone();
                context.Foreground = QuoteForeground;
            }

            var stackPanel = new StackPanel();
            RenderBlocks(element.Blocks, stackPanel.Children, context);

            var border = new Border();
            border.Margin = QuoteMargin;
            border.Background = QuoteBackground;
            border.BorderBrush = QuoteBorderBrush ?? context.Foreground;
            border.BorderThickness = QuoteBorderThickness;
            border.Padding = QuotePadding;
            border.Child = stackPanel;

            blockUIElementCollection.Add(border);
        }


        /// <summary>
        /// Renders a code element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        /// <param name="context"></param>
        private void RenderCode(CodeBlock element, UIElementCollection blockUIElementCollection, RenderContext context)
        {
            var textBlock = CreateTextBlock(context);
            textBlock.FontFamily = CodeFontFamily ?? FontFamily;
            textBlock.Foreground = CodeForeground ?? context.Foreground;
            textBlock.LineHeight = FontSize * 1.4;
            textBlock.Text = element.Text;

            var border = new Border();
            border.Background = CodeBackground;
            border.BorderBrush = CodeBorderBrush;
            border.BorderThickness = CodeBorderThickness;
            border.Padding = CodePadding;
            border.Margin = CodeMargin;
            border.HorizontalAlignment = HorizontalAlignment.Left;
            border.Child = textBlock;

            // Add it to the blocks
            blockUIElementCollection.Add(border);
        }

        /// <summary>
        /// Renders a table element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="blockUIElementCollection"></param>
        /// <param name="context"></param>
        private void RenderTable(TableBlock element, UIElementCollection blockUIElementCollection, RenderContext context)
        {
            var table = new MarkdownTable(element.ColumnDefinitions.Count, element.Rows.Count, TableBorderThickness, TableBorderBrush);
            table.HorizontalAlignment = HorizontalAlignment.Left;
            table.Margin = TableMargin;

            // Add each row.
            for (int rowIndex = 0; rowIndex < element.Rows.Count; rowIndex++)
            {
                var row = element.Rows[rowIndex];

                // Add each cell.
                for (int cellIndex = 0; cellIndex < Math.Min(element.ColumnDefinitions.Count, row.Cells.Count); cellIndex++)
                {
                    var cell = row.Cells[cellIndex];

                    // Cell content.
                    var cellContent = CreateOrReuseRichTextBlock(null, context);
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
                    context.TrimLeadingWhitespace = true;
                    RenderInlineChildren(paragraph.Inlines, cell.Inlines, paragraph, context);
                    cellContent.Blocks.Add(paragraph);
                    table.Children.Add(cellContent);
                }
            }

            blockUIElementCollection.Add(table);
        }

        #endregion

        #region Render Inlines

        /// <summary>
        /// Renders all of the children for the given element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="inlineElements"> The parsed inline elements to render. </param>
        /// <param name="parent"> The container element. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderInlineChildren(InlineCollection inlineCollection, IList<MarkdownInline> inlineElements, TextElement parent, RenderContext context)
        {
            foreach (MarkdownInline element in inlineElements)
            {
                RenderInline(inlineCollection, element, parent, context);
            }
        }

        /// <summary>
        /// Called to render an inline element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="parent"> The container element. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderInline(InlineCollection inlineCollection, MarkdownInline element, TextElement parent, RenderContext context)
        {
            switch (element.Type)
            {
                case MarkdownInlineType.TextRun:
                    RenderTextRun(inlineCollection, (TextRunInline)element, parent, context);
                    break;
                case MarkdownInlineType.Italic:
                    RenderItalicRun(inlineCollection, (ItalicTextInline)element, parent, context);
                    break;
                case MarkdownInlineType.Bold:
                    RenderBoldRun(inlineCollection, (BoldTextInline)element, parent, context);
                    break;
                case MarkdownInlineType.MarkdownLink:
                    RenderMarkdownLink(inlineCollection, (MarkdownLinkInline)element, parent, context);
                    break;
                case MarkdownInlineType.RawHyperlink:
                    RenderHyperlink(inlineCollection, (HyperlinkInline)element, parent, context);
                    break;
                case MarkdownInlineType.Strikethrough:
                    RenderStrikethroughRun(inlineCollection, (StrikethroughTextInline)element, parent, context);
                    break;
                case MarkdownInlineType.Superscript:
                    RenderSuperscriptRun(inlineCollection, (SuperscriptTextInline)element, parent, context);
                    break;
                case MarkdownInlineType.Code:
                    RenderCodeRun(inlineCollection, (CodeInline)element, parent, context);
                    break;
            }
        }

        /// <summary>
        /// Renders a text run element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="parent"> The container element. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderTextRun(InlineCollection inlineCollection, TextRunInline element, TextElement parent, RenderContext context)
        {
            // Create the text run
            Run textRun = new Run();
            textRun.Text = CollapseWhitespace(context, element.Text);

            // Add it
            inlineCollection.Add(textRun);
        }

        /// <summary>
        /// Renders a bold run element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="parent"> The container element. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderBoldRun(InlineCollection inlineCollection, BoldTextInline element, TextElement parent, RenderContext context)
        {
            // Create the text run
            Span boldSpan = new Span();
            boldSpan.FontWeight = FontWeights.Bold;

            // Render the children into the bold inline.
            RenderInlineChildren(boldSpan.Inlines, element.Inlines, boldSpan, context);

            // Add it to the current inlines
            inlineCollection.Add(boldSpan);
        }

        /// <summary>
        /// Renders a link element
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="parent"> The container element. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderMarkdownLink(InlineCollection inlineCollection, MarkdownLinkInline element, TextElement parent, RenderContext context)
        {
            // Avoid crash when link text is empty.
            if (element.Inlines.Count == 0)
                return;

            // Attempt to resolve references.
            element.ResolveReference(this.document);
            if (element.Url == null)
            {
                // The element couldn't be resolved, just render it as text.
                RenderInlineChildren(inlineCollection, element.Inlines, parent, context);
                return;
            }

            // HACK: Superscript is not allowed within a hyperlink.  But if we switch it around, so
            // that the superscript is outside the hyperlink, then it will render correctly.
            // This assumes that the entire hyperlink is to be rendered as superscript.
            if (AllTextIsSuperscript(element) == false)
            {
                // Regular ol' hyperlink.
                var link = new Hyperlink();
                link.Foreground = LinkForeground ?? Foreground;
                // Register the link
                this.linkRegister.RegisterNewHyperLink(link, element.Url);

                // Remove superscripts.
                RemoveSuperscriptRuns(element, insertCaret: true);

                // Render the children into the link inline.
                var childContext = context.Clone();
                childContext.WithinHyperlink = true;
                RenderInlineChildren(link.Inlines, element.Inlines, link, childContext);
                context.TrimLeadingWhitespace = childContext.TrimLeadingWhitespace;

                // Add it to the current inlines
                inlineCollection.Add(link);
            }
            else
            {
                // THE HACK IS ON!

                // Create a fake superscript element.
                var fakeSuperscript = new SuperscriptTextInline();
                fakeSuperscript.Inlines = new List<MarkdownInline> { element };

                // Remove superscripts.
                RemoveSuperscriptRuns(element, insertCaret: false);

                // Now render it.
                RenderSuperscriptRun(inlineCollection, fakeSuperscript, parent, context);

            }
        }

        /// <summary>
        /// Renders a raw link element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="parent"> The container element. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderHyperlink(InlineCollection inlineCollection, HyperlinkInline element, TextElement parent, RenderContext context)
        {
            var link = new Hyperlink();

            // Register the link
            this.linkRegister.RegisterNewHyperLink(link, element.Url);

            // Make a text block for the link
            Run linkText = new Run();
            linkText.Foreground = LinkForeground ?? Foreground;
            linkText.Text = CollapseWhitespace(context, element.Text);
            link.Inlines.Add(linkText);

            // Add it to the current inlines
            inlineCollection.Add(link);
        }

        /// <summary>
        /// Renders a text run element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="parent"> The container element. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderItalicRun(InlineCollection inlineCollection, ItalicTextInline element, TextElement parent, RenderContext context)
        {
            // Create the text run
            Span italicSpan = new Span();
            italicSpan.FontStyle = FontStyle.Italic;

            // Render the children into the italic inline.
            RenderInlineChildren(italicSpan.Inlines, element.Inlines, italicSpan, context);

            // Add it to the current inlines
            inlineCollection.Add(italicSpan);
        }

        /// <summary>
        /// Renders a strikethrough element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="parent"> The container element. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderStrikethroughRun(InlineCollection inlineCollection, StrikethroughTextInline element, TextElement parent, RenderContext context)
        {
            Span span = new Span();
            span.FontFamily = new FontFamily("Consolas");

            // Render the children into the inline.
            RenderInlineChildren(span.Inlines, element.Inlines, span, context);

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
            inlineCollection.Add(span);
        }

        /// <summary>
        /// Renders a superscript element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="parent"> The container element. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderSuperscriptRun(InlineCollection inlineCollection, SuperscriptTextInline element, TextElement parent, RenderContext context)
        {
            // Le <sigh>, InlineUIContainers are not allowed within hyperlinks.
            if (context.WithinHyperlink)
            {
                RenderInlineChildren(inlineCollection, element.Inlines, parent, context);
                return;
            }

            var paragraph = new Paragraph();
            paragraph.FontSize = parent.FontSize * 0.8;
            paragraph.FontFamily = parent.FontFamily;
            paragraph.FontStyle = parent.FontStyle;
            paragraph.FontWeight = parent.FontWeight;
            RenderInlineChildren(paragraph.Inlines, element.Inlines, paragraph, context);

            var richTextBlock = CreateOrReuseRichTextBlock(null, context);
            richTextBlock.Blocks.Add(paragraph);

            var border = new Border();
            border.Padding = new Thickness(0, 0, 0, paragraph.FontSize * 0.2);
            border.Child = richTextBlock;

            var inlineUIContainer = new InlineUIContainer();
            inlineUIContainer.Child = border;

            // Add it to the current inlines
            inlineCollection.Add(inlineUIContainer);
        }

        /// <summary>
        /// Renders a code element
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="parent"> The container element. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderCodeRun(InlineCollection inlineCollection, CodeInline element, TextElement parent, RenderContext context)
        {
            var run = new Run();
            run.FontFamily = CodeFontFamily ?? FontFamily;
            run.Text = CollapseWhitespace(context, element.Text);

            // Add it to the current inlines
            inlineCollection.Add(run);
        }

        #endregion

        #region Helper methods

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
            for (int i = 0; i < text.Length; i++)
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
        /// <param name="context"></param>
        /// <returns></returns>
        private RichTextBlock CreateOrReuseRichTextBlock(UIElementCollection blockUIElementCollection, RenderContext context)
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
            result.Foreground = context.Foreground;
            result.IsTextSelectionEnabled = IsTextSelectionEnabled;
            result.TextWrapping = TextWrapping;
            if (blockUIElementCollection != null)
                blockUIElementCollection.Add(result);
            return result;
        }

        /// <summary>
        /// Creates a new TextBlock, with default settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private TextBlock CreateTextBlock(RenderContext context)
        {
            var result = new TextBlock();
            result.CharacterSpacing = CharacterSpacing;
            result.FontFamily = FontFamily;
            result.FontSize = FontSize;
            result.FontStretch = FontStretch;
            result.FontStyle = FontStyle;
            result.FontWeight = FontWeight;
            result.Foreground = context.Foreground;
            result.IsTextSelectionEnabled = IsTextSelectionEnabled;
            result.TextWrapping = TextWrapping;
            return result;
        }

        /// <summary>
        /// Checks if all text elements inside the given container are superscript.
        /// </summary>
        /// <param name="container"></param>
        /// <returns> <c>true</c> if all text is superscript (level 1); <c>false</c> otherwise. </returns>
        private bool AllTextIsSuperscript(IInlineContainer container, int superscriptLevel = 0)
        {
            foreach (var inline in container.Inlines)
            {
                if (inline is SuperscriptTextInline)
                {
                    // Remove any nested superscripts.
                    if (AllTextIsSuperscript((IInlineContainer)inline, superscriptLevel + 1) == false)
                        return false;
                }
                else if (inline is IInlineContainer)
                {
                    // Remove any superscripts.
                    if (AllTextIsSuperscript((IInlineContainer)inline, superscriptLevel) == false)
                        return false;
                }
                else if (inline is IInlineLeaf && !Common.IsBlankOrWhiteSpace(((IInlineLeaf)inline).Text))
                {
                    if (superscriptLevel != 1)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Removes all superscript elements from the given container.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="insertCaret"></param>
        private void RemoveSuperscriptRuns(IInlineContainer container, bool insertCaret)
        {
            for (int i = 0; i < container.Inlines.Count; i++)
            {
                var inline = container.Inlines[i];
                if (inline is SuperscriptTextInline)
                {
                    // Remove any nested superscripts.
                    RemoveSuperscriptRuns((IInlineContainer)inline, insertCaret);

                    // Remove the superscript element, insert all the children.
                    container.Inlines.RemoveAt(i);
                    if (insertCaret)
                        container.Inlines.Insert(i++, new TextRunInline { Text = "^" });
                    foreach (var superscriptInline in ((SuperscriptTextInline)inline).Inlines)
                        container.Inlines.Insert(i++, superscriptInline);
                    i--;
                }
                else if (inline is IInlineContainer)
                {
                    // Remove any superscripts.
                    RemoveSuperscriptRuns((IInlineContainer)inline, insertCaret);
                }
            }
        }

        #endregion
    }
}
