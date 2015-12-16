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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Helpers;
using UniversalMarkdown.Interfaces;
using UniversalMarkdown.Parse;
using UniversalMarkdown.Parse.Elements;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace UniversalMarkdown.Display
{
    public class RenderToRichTextBlock
    {
        /// <summary>
        /// The rich text block we will dump the markdown into.
        /// </summary>
        RichTextBlock m_richTextBlock;

        /// <summary>
        /// A register class used to listen to hyper link clicks.
        /// </summary>
        ILinkRegister m_linkRegister;

        public RenderToRichTextBlock(RichTextBlock richTextBlock, ILinkRegister linkRegister)
        {
            m_richTextBlock = richTextBlock;
            m_linkRegister = linkRegister;
        }

        /// <summary>
        /// Called externally to render markdown to a text block.
        /// </summary>
        /// <param name="markdownTree"></param>
        public void Render(Markdown markdownTree)
        {
            // Clear anything that currently exists
            m_richTextBlock.Blocks.Clear();

            // For the root, loop through the block types and render them
            foreach (MarkdownBlock element in markdownTree.Children)
            {
                RenderBlock(element, m_richTextBlock.Blocks);
            }
        }

        /// <summary>
        /// Called to render a block element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentBlocks"></param>
        private void RenderBlock(MarkdownBlock element, BlockCollection currentBlocks)
        {
            switch (element.Type)
            {
                case MarkdownBlockType.Paragraph:
                    RenderPargraph((ParagraphBlock)element, currentBlocks);
                    break;
                case MarkdownBlockType.Quote:
                    RenderQuote((QuoteBlock)element, currentBlocks);
                    break;
                case MarkdownBlockType.Code:
                    RenderCode((CodeBlock)element, currentBlocks);
                    break;
                case MarkdownBlockType.Header:
                    RenderHeader((HeaderBlock)element, currentBlocks);
                    break;
                case MarkdownBlockType.ListElement:
                    RenderListElement((ListElementBlock)element, currentBlocks);
                    break;
                case MarkdownBlockType.HorizontalRule:
                    RenderHorizontalRule((HorizontalRuleBlock)element, currentBlocks);
                    break;
                case MarkdownBlockType.LineBreak:
                    RenderLineBreak((LineBreakBlock)element, currentBlocks);
                    break;
            }
        }

        /// <summary>
        /// Called to render an inline element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        private void RenderInline(MarkdownInline element, InlineCollection currentInlines, ref bool trimTextStart)
        {
            switch(element.Type)
            {
                case MarkdownInlineType.TextRun:
                    RenderTextRun((TextRunInline)element, currentInlines, ref trimTextStart);
                    break;
                case MarkdownInlineType.Bold:
                    RenderBoldRun((BoldTextInline)element, currentInlines, ref trimTextStart);
                    break;
                case MarkdownInlineType.MarkdownLink:
                    RenderMarkdownLink((MarkdownLinkInline)element, currentInlines, ref trimTextStart);
                    break;
                case MarkdownInlineType.Italic:
                    RenderItalicRun((ItalicTextInline)element, currentInlines, ref trimTextStart);
                    break;
                case MarkdownInlineType.RawHyperlink:
                    RenderRawHyperlink((RawHyperlinkInline)element, currentInlines, ref trimTextStart);
                    break;
                case MarkdownInlineType.RawSubreddit:
                    RenderRawSubreddit((RawSubredditInline)element, currentInlines, ref trimTextStart);
                    break;
                case MarkdownInlineType.Strikethrough:
                    RenderStrikethroughRun((StrikethroughTextInline)element, currentInlines, ref trimTextStart);
                    break;
                case MarkdownInlineType.Superscript:
                    RenderSuperscriptRun((SuperscriptTextInline)element, currentInlines, ref trimTextStart);
                    break;
                case MarkdownInlineType.Code:
                    RenderCodeRun((SuperscriptTextInline)element, currentInlines, ref trimTextStart);
                    break;
            }
        }

        /// <summary>
        /// Renders all of the children for the given element.
        /// </summary>
        /// <param name="rootElemnet">The root element to render children of</param>
        /// <param name="currentInlines">The inlines where they should go</param>
        /// <param name="trimTextStart">If true the first text box start will be trimed so there is no leading space</param>
        private void RenderInlineChildren(MarkdownElement rootElemnet, InlineCollection currentInlines, ref bool trimTextStart)
        {
            foreach (MarkdownInline element in rootElemnet.Children)
            {
                RenderInline(element, currentInlines, ref trimTextStart);
            }
        }

        #region Render Blocks

        /// <summary>
        /// Renders a paragraph element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentBlocks"></param>
        private void RenderPargraph(ParagraphBlock element, BlockCollection currentBlocks)
        {
            // Make a new paragraph
            Paragraph paragraph = new Paragraph();

            // Add some padding to differentiate the blocks
            paragraph.Margin = new Thickness(0, 12, 0, 0);

            // Add it to the blocks
            currentBlocks.Add(paragraph);

            // Render the children into the para inline.
            bool trimTextStart = true;
            RenderInlineChildren(element, paragraph.Inlines, ref trimTextStart);
        }

        /// <summary>
        /// Renders a header element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentBlocks"></param>
        private void RenderHeader(HeaderBlock element, BlockCollection currentBlocks)
        {
            // Make the new header paragraph
            Paragraph headerPara = new Paragraph();
            headerPara.Margin = new Thickness(0, 18, 0, 12);

            // Set the style
            switch (element.HeaderLevel)
            {
                case 1:
                    headerPara.FontSize = 20;
                    headerPara.FontWeight = FontWeights.Bold;
                    break;
                case 2:
                    headerPara.FontSize = 20;
                    break;
                case 3:
                    headerPara.FontSize = 17;
                    headerPara.FontWeight = FontWeights.Bold;
                    break;
                case 4:
                    headerPara.FontSize = 17;
                    break;
                case 5:
                default:
                    headerPara.FontWeight = FontWeights.Bold;
                    break;
            }

            // Add it to the blocks
            currentBlocks.Add(headerPara);

            // Render the children into the para inline.
            bool trimTextStart = true;
            RenderInlineChildren(element, headerPara.Inlines, ref trimTextStart);
        }

        /// <summary>
        /// Renders a list element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentBlocks"></param>
        private void RenderListElement(ListElementBlock element, BlockCollection currentBlocks)
        {
            // Create a grid for the dot and the text
            Grid grid = new Grid();

            // The first column for the dot the second for the text
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            // Make the dot container and the new text box
            TextBlock dotText = new TextBlock();
            dotText.Text = element.ListBullet;
            Grid.SetColumn(dotText, 0);
            // Add the indent
            dotText.Margin = new Thickness(12 * element.ListIndent, 2, 0, 2);
            grid.Children.Add(dotText);

            RichTextBlock listText = new RichTextBlock();
            Grid.SetColumn(listText, 1);
            // Give the text some space from the dot and also from the top and bottom
            listText.Margin = new Thickness(6, 2, 0, 2);
            grid.Children.Add(listText);

            // Make the inline container
            InlineUIContainer uiConainter = new InlineUIContainer();
            uiConainter.Child = grid;

            // Make a paragraph to hold our list
            Paragraph blockParagraph = new Paragraph();
            blockParagraph.Inlines.Add(uiConainter);

            // Make a paragraph to hold our list test
            Paragraph inlineParagraph = new Paragraph();
            listText.Blocks.Add(inlineParagraph);

            // Add it to the blocks
            currentBlocks.Add(blockParagraph);

            // Render the children into the rich.
            bool trimTextStart = true;
            RenderInlineChildren(element, inlineParagraph.Inlines, ref trimTextStart);
        }

        /// <summary>
        /// Renders a horizontal rule element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentBlocks"></param>
        private void RenderHorizontalRule(HorizontalRuleBlock element, BlockCollection currentBlocks)
        {
            // This is going to be weird. To make this work we need to make a UI element
            // and fill it with text to make it stretch. If we don't fill it with text I can't
            // make it stretch the width of the box, so for now this is an "ok" hack.
            InlineUIContainer contianer = new InlineUIContainer();
            Grid grid = new Grid();
            grid.Height = 2;
            grid.Background = new SolidColorBrush(Color.FromArgb(255, 153, 153, 153));

            // Add the expanding text block.
            TextBlock magicExpandingTextBlock = new TextBlock();
            magicExpandingTextBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 153, 153, 153));
            magicExpandingTextBlock.Text = "This is Quinn writing magic text. You will never see this. Like a ghost! I love Marilyn Welniak! This needs to be really long! RRRRREEEEEAAAAALLLLYYYYY LLLOOOONNNGGGG. This is Quinn writing magic text. You will never see this. Like a ghost! I love Marilyn Welniak! This needs to be really long! RRRRREEEEEAAAAALLLLYYYYY LLLOOOONNNGGGG";
            grid.Children.Add(magicExpandingTextBlock);

            // Add the grid.
            contianer.Child = grid;

            // Make the new horizontal rule paragraph
            Paragraph horzPara = new Paragraph();
            horzPara.Margin = new Thickness(0, 12, 0, 12);
            horzPara.Inlines.Add(contianer);

            // Add it
            currentBlocks.Add(horzPara);
        }

        /// <summary>
        /// Renders a line break element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentBlocks"></param>
        private void RenderLineBreak(LineBreakBlock element, BlockCollection currentBlocks)
        {
            // Make the new horizontal rule paragraph
            Paragraph lineBreakPara = new Paragraph();
            lineBreakPara.Margin = new Thickness(0, 12, 0, 12);

            // Add it
            currentBlocks.Add(lineBreakPara);
        }

        /// <summary>
        /// Renders a quote element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentBlocks"></param>
        private void RenderQuote(QuoteBlock element, BlockCollection currentBlocks)
        {
            // Make the new quote paragraph
            Paragraph quotePara = new Paragraph();
            quotePara.Margin = new Thickness(element.QuoteIndent * 12, 12, 12, 12);
            quotePara.Foreground = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255));

            // Add it to the blocks
            currentBlocks.Add(quotePara);

            // Render the children into the para inline.
            bool trimTextStart = true;
            RenderInlineChildren(element, quotePara.Inlines, ref trimTextStart);
        }


        /// <summary>
        /// Renders a code element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentBlocks"></param>
        private void RenderCode(CodeBlock element, BlockCollection currentBlocks)
        {
            // Make the new code paragraph
            Paragraph codePara = new Paragraph();
            codePara.Margin = new Thickness(12 * element.CodeIndent, 0, 0, 0);
            codePara.Foreground = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255));
            codePara.FontFamily = new FontFamily("Courier New");

            // Add it to the blocks
            currentBlocks.Add(codePara);

            // Render the children into the para inline.
            bool trimTextStart = true;
            RenderInlineChildren(element, codePara.Inlines, ref trimTextStart);
        }

        #endregion

        #region Render Inlines

        /// <summary>
        /// Renders a text run element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="trimTextStart">If true this element should trin the start of the text and set to fales.</param>
        private void RenderTextRun(TextRunInline element, InlineCollection currentInlines, ref bool trimTextStart)
        {
            // Create the text run
            Run textRun = new Run();
            textRun.Text = element.Text;

            // Check if we should trim the starting text. This allows us to trim the text starting a block
            // but nothing else. If we do a trim set it to false so no one else does.
            if(trimTextStart)
            {
                trimTextStart = false;
                textRun.Text = textRun.Text.TrimStart();
            }

            // Add it
            currentInlines.Add(textRun);
        }

        /// <summary>
        /// Renders a bold run element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="trimTextStart">If true this element should trin the start of the text and set to fales.</param>
        private void RenderBoldRun(BoldTextInline element, InlineCollection currentInlines, ref bool trimTextStart)
        {
            // Create the text run
            Span boldSpan = new Span();
            boldSpan.FontWeight = FontWeights.Bold;

            // Render the children into the bold inline.
            RenderInlineChildren(element, boldSpan.Inlines, ref trimTextStart);

            // Add it to the current inlines
            currentInlines.Add(boldSpan);
        }

        /// <summary>
        /// Renders a link element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="trimTextStart">If true this element should trin the start of the text and set to fales.</param>
        private void RenderMarkdownLink(MarkdownLinkInline element, InlineCollection currentInlines, ref bool trimTextStart)
        {
            // Create the text run
            Hyperlink link = new Hyperlink();

            // Register the link
            m_linkRegister.RegisterNewHyperLink(link, element.Url);

            // Render the children into the link inline.
            RenderInlineChildren(element, link.Inlines, ref trimTextStart);

            // Add it to the current inlines
            currentInlines.Add(link);
        }

        /// <summary>
        /// Renders a raw link element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="trimTextStart">If true this element should trin the start of the text and set to fales.</param>
        private void RenderRawHyperlink(RawHyperlinkInline element, InlineCollection currentInlines, ref bool trimTextStart)
        {
            // Create the text run
            Hyperlink link = new Hyperlink();

            // Register the link
            m_linkRegister.RegisterNewHyperLink(link, element.Url);

            // Make a text block for the link
            Run linkText = new Run();
            linkText.Text = element.Url;
            link.Inlines.Add(linkText);

            if (trimTextStart)
            {
                trimTextStart = false;
                linkText.Text = linkText.Text.Trim();
            }

            // Add it to the current inlines
            currentInlines.Add(link);
        }

        /// <summary>
        /// Renders a raw subreddit element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="trimTextStart">If true this element should trin the start of the text and set to fales.</param>
        private void RenderRawSubreddit(RawSubredditInline element, InlineCollection currentInlines, ref bool trimTextStart)
        {
            // Create the hyper link
            Hyperlink link = new Hyperlink();

            // Register the link
            m_linkRegister.RegisterNewHyperLink(link, element.Text);

            // Add the subreddit text
            Run subreddit = new Run();
            subreddit.Text = element.Text;
            link.Inlines.Add(subreddit);

            if(trimTextStart)
            {
                trimTextStart = false;
                subreddit.Text = element.Text.Trim();
            }

            // Add it to the current inlines
            currentInlines.Add(link);
        }

        /// <summary>
        /// Renders a text run element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="trimTextStart">If true this element should trin the start of the text and set to fales.</param>
        private void RenderItalicRun(ItalicTextInline element, InlineCollection currentInlines, ref bool trimTextStart)
        {
            // Create the text run
            Span italicSpan = new Span();
            italicSpan.FontStyle = FontStyle.Italic;

            // Render the children into the italic inline.
            RenderInlineChildren(element, italicSpan.Inlines, ref trimTextStart);

            // Add it to the current inlines
            currentInlines.Add(italicSpan);
        }

        /// <summary>
        /// Renders a strikethrough element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="trimTextStart">If true this element should trin the start of the text and set to fales.</param>
        private void RenderStrikethroughRun(StrikethroughTextInline element, InlineCollection currentInlines, ref bool trimTextStart)
        {
            // TODO: make this actually strikethrough somehow...
            Span span = new Span();

            // Render the children into the inline.
            RenderInlineChildren(element, span.Inlines, ref trimTextStart);

            // Add it to the current inlines
            currentInlines.Add(span);
        }

        /// <summary>
        /// Renders a superscript element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="trimTextStart">If true this element should trin the start of the text and set to fales.</param>
        private void RenderSuperscriptRun(SuperscriptTextInline element, InlineCollection currentInlines, ref bool trimTextStart)
        {
            Span span = new Span();
            Typography.SetVariants(span, FontVariants.Superscript);

            // Render the children into the inline.
            RenderInlineChildren(element, span.Inlines, ref trimTextStart);

            // Add it to the current inlines
            currentInlines.Add(span);
        }

        /// <summary>
        /// Renders a code element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentInlines"></param>
        /// <param name="trimTextStart">If true this element should trin the start of the text and set to fales.</param>
        private void RenderCodeRun(SuperscriptTextInline element, InlineCollection currentInlines, ref bool trimTextStart)
        {
            Span span = new Span();
            span.FontFamily = new FontFamily("Consolas");

            // Render the children into the inline.
            RenderInlineChildren(element, span.Inlines, ref trimTextStart);

            // Add it to the current inlines
            currentInlines.Add(span);
        }

        #endregion
    }
}
