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


using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalMarkdown.Parse;
using UniversalMarkdown.Parse.Elements;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using System.Numerics;
using Microsoft.Graphics.Canvas.Brushes;
using UniversalMarkdown.Helpers;

namespace UniversalMarkdown.Display
{
    public class MarkdownRenderer
    {
        private Markdown markdownTree;

        /// <summary>
        /// The primary text color.
        /// </summary>
        public Color ForegroundColor { get; set; } = new Color { A = 255, R = 255, G = 255, B = 255 };

        /// <summary>
        /// Gets or sets the amount of space above and below paragraphs.
        /// </summary>
        public double ParagraphSpacing { get; set; } = 5.0;

        /// <summary>
        /// The font size for paragraph text.
        /// </summary>
        public double ParagraphFontSize { get; set; } = 13;

        /// <summary>
        /// Gets or sets the amount of space above and below list.
        /// </summary>
        public double ListSpacing { get; set; } = 5.0;

        /// <summary>
        /// Gets or sets the amount of space to the left of lists that is used for bullets/numbers.
        /// </summary>
        public double ListLeftPadding { get; set; } = 40.0;

        /// <summary>
        /// Gets or sets the amount of space between bullets/numbers and the list content.
        /// </summary>
        public double ListBulletPadding { get; set; } = 10.0;

        /// <summary>
        /// The font to use for code blocks.
        /// </summary>
        public string CodeFontFamily { get; set; } = "Consolas";

        /// <summary>
        /// Gets or sets the amount of space above and below code blocks.
        /// </summary>
        public double CodeSpacing { get; set; } = 4.0;

        /// <summary>
        /// Gets or sets the color of text inside a code block or inline code section.
        /// </summary>
        public Color CodeTextColor { get; set; } = new Color { A = 255, R = 212, G = 212, B = 212 };

        /// <summary>
        /// The color of hyperlinks.
        /// </summary>
        public Color LinkColor { get; set; } = Colors.LightBlue;

        /// <summary>
        /// The color of a horizontal rule.
        /// </summary>
        public Color HorizontalRuleColor { get; set; } = Colors.White;

        /// <summary>
        /// The thickness of a horizontal rule.
        /// </summary>
        public double HorizontalRuleThickness { get; set; } = 1.0;

        /// <summary>
        /// The amount of space above and below the horizontal rule.
        /// </summary>
        public double HorizontalRuleSpacing { get; set; } = 7.0;

        /// <summary>
        /// The size of superscript text, as a fraction of the regular size (e.g. use 0.8 to have
        /// superscript text that is 80% of the regular font size).
        /// </summary>
        public double SuperscriptSize { get; set; } = 0.8;

        /// <summary>
        /// The color of the table borders.
        /// </summary>
        public Color TableBorderColor { get; set; } = Colors.White;

        /// <summary>
        /// The thickness of table borders.
        /// </summary>
        public double TableBorderThickness { get; set; } = 1.0;

        /// <summary>
        /// The amount of space above and below tables.
        /// </summary>
        public double TableSpacing { get; set; } = 7.0;

        /// <summary>
        /// The amount of horizontal and vertical space within each cell.
        /// </summary>
        public Size TableCellPadding { get; set; } = new Size(9, 4);


        public MarkdownRenderer(Markdown markdownTree)
        {
            if (markdownTree == null)
                throw new ArgumentNullException(nameof(markdownTree));
            this.markdownTree = markdownTree;
        }

        private class DrawingContext
        {
            /// <summary>
            /// The device which handles resource creation.
            /// </summary>
            public CanvasDevice Device { get; private set; }

            /// <summary>
            /// The drawing session to draw to.  Will be <c>null</c> when measuring.
            /// </summary>
            public CanvasDrawingSession DrawingSession { get; private set; }

            public bool MeasureOnly { get; private set; }

            /// <summary>
            /// The current horizontal offset.
            /// </summary>
            public double X { get; private set; }

            /// <summary>
            /// The current vertical offset.
            /// </summary>
            public double Y { get; private set; }

            /// <summary>
            /// The amount of horizontal space available.
            /// </summary>
            public double AvailableWidth { get; private set; }

            /// <summary>
            /// The actual width of the content.
            /// </summary>
            public double ContentWidth { get; private set; }

            /// <summary>
            /// The actual height of the content.
            /// </summary>
            public double ContentHeight { get { return Y - initialY; } }

            private double initialY;
            private double previousBottomMargin;

            private DrawingContext()
            {
            }

            public DrawingContext(CanvasDevice device, Size availableSize)
            {
                Device = device;
                AvailableWidth = availableSize.Width;
                MeasureOnly = true;
            }

            public DrawingContext(CanvasDrawingSession drawingSession, Rect rect)
            {
                Device = drawingSession.Device;
                DrawingSession = drawingSession;
                X = rect.Left;
                initialY = Y = rect.Top;
                AvailableWidth = rect.Width;
            }

            public void ApplyTopAndBottomMargins(double margin)
            {
                ApplyTopAndBottomMargins(margin, margin);
            }

            public void ApplyTopAndBottomMargins(double topMargin, double bottomMargin)
            {
                if (Y > initialY)
                    Y += Math.Max(previousBottomMargin, topMargin);
                previousBottomMargin = bottomMargin;
            }

            public void ApplyBlockContent(double width, double height)
            {
                ContentWidth = Math.Max(ContentWidth, width);
                Y += height;
            }

            public DrawingContext Clone(double x, double availableWidth)
            {
                var result = new DrawingContext();
                result.Device = Device;
                result.DrawingSession = DrawingSession;
                result.MeasureOnly = MeasureOnly;
                result.X = x;
                result.Y = Y;
                result.AvailableWidth = availableWidth;
                result.ContentWidth = 0;
                result.initialY = Y;
                result.previousBottomMargin = 0;
                return result;
            }
        }

        public void Draw(CanvasDrawingSession drawingSession, Rect rect)
        {
            var context = new DrawingContext(drawingSession, rect);
            foreach (var block in this.markdownTree.Blocks)
            {
                RenderBlock(context, block);
            }
        }

        public Size Measure(CanvasDevice device, Size availableSize)
        {
            var context = new DrawingContext(device, availableSize);
            foreach (var block in this.markdownTree.Blocks)
            {
                RenderBlock(context, block);
            }
            return new Size(context.ContentWidth, context.Y);
        }

        private void RenderBlock(DrawingContext context, MarkdownBlock block)
        {
            switch (block.Type)
            {
                case MarkdownBlockType.Paragraph:
                    {
                        context.ApplyTopAndBottomMargins(ParagraphSpacing);

                        var textFormat = new CanvasTextFormat();
                        textFormat.FontSize = (float)this.ParagraphFontSize;
                        RenderInlines(context, ((ParagraphBlock)block).Inlines, textFormat);
                        break;
                    }

                case MarkdownBlockType.Header:
                    {
                        var textFormat = new CanvasTextFormat();
                        Action<CanvasTextLayout, int> initTextLayout = null;
                        switch (((HeaderBlock)block).HeaderLevel)
                        {
                            case 1:
                                context.ApplyTopAndBottomMargins(15.0);
                                textFormat.FontSize = 20;
                                textFormat.FontWeight = FontWeights.Bold;
                                break;
                            case 2:
                                context.ApplyTopAndBottomMargins(15.0);
                                textFormat.FontSize = 20;
                                break;
                            case 3:
                                context.ApplyTopAndBottomMargins(10.0);
                                textFormat.FontSize = 17;
                                textFormat.FontWeight = FontWeights.Bold;
                                break;
                            case 4:
                                context.ApplyTopAndBottomMargins(10.0);
                                textFormat.FontSize = 17;
                                break;
                            case 5:
                                context.ApplyTopAndBottomMargins(10.0, 5.0);
                                textFormat.FontSize = (float)this.ParagraphFontSize;
                                textFormat.FontWeight = FontWeights.Bold;
                                break;
                            case 6:
                                context.ApplyTopAndBottomMargins(10.0, 5.0);
                                textFormat.FontSize = (float)this.ParagraphFontSize;
                                initTextLayout = (textLayout, textLength) => textLayout.SetUnderline(0, textLength, hasUnderline: true);
                                break;
                        }
                        RenderInlines(context, ((HeaderBlock)block).Inlines, textFormat, initTextLayout);
                        break;
                    }

                case MarkdownBlockType.HorizontalRule:
                    context.ApplyTopAndBottomMargins(HorizontalRuleSpacing);
                    if (!context.MeasureOnly)
                    {
                        context.DrawingSession.FillRectangle(new Rect(context.X, context.Y, context.AvailableWidth, HorizontalRuleThickness), HorizontalRuleColor);
                    }
                    context.ApplyBlockContent(context.AvailableWidth, HorizontalRuleThickness);
                    break;

                case MarkdownBlockType.Code:
                    {
                        context.ApplyTopAndBottomMargins(CodeSpacing);
                        var textFormat = new CanvasTextFormat();
                        textFormat.FontSize = (float)ParagraphFontSize;
                        textFormat.FontFamily = CodeFontFamily;
                        using (var textLayout = new CanvasTextLayout(context.Device, ((CodeBlock)block).Text, textFormat, (float)context.AvailableWidth, 0))
                        {
                            if (!context.MeasureOnly)
                            {
                                context.DrawingSession.DrawTextLayout(textLayout, (float)context.X, (float)context.Y, CodeTextColor);
                            }
                            context.ApplyBlockContent(textLayout.LayoutBounds.Width, textLayout.LayoutBounds.Height);
                        }
                        break;
                    }

                case MarkdownBlockType.List:
                    {
                        var list = (ListBlock)block;
                        context.ApplyTopAndBottomMargins(ListSpacing);

                        var bulletTextFormat = new CanvasTextFormat();
                        bulletTextFormat.FontSize = (float)this.ParagraphFontSize;
                        bulletTextFormat.HorizontalAlignment = CanvasHorizontalAlignment.Right;

                        for (int i = 0; i < list.Items.Count; i++)
                        {
                            // Draw the bullet or number.
                            if (!context.MeasureOnly)
                            {
                                string bulletText = "";
                                switch (((ListBlock)block).Style)
                                {
                                    case ListStyle.Bulleted:
                                        bulletText = "•";
                                        break;
                                    case ListStyle.Numbered:
                                        bulletText = string.Format("{0}.", i + 1);
                                        break;
                                }
                                context.DrawingSession.DrawText(bulletText, new Rect(context.X - ListLeftPadding, context.Y, ListLeftPadding - ListBulletPadding, 0), ForegroundColor, bulletTextFormat);
                            }

                            // Draw the list item contents.
                            var listItemContext = context.Clone(context.X + ListLeftPadding, context.AvailableWidth - ListLeftPadding);
                            foreach (var itemBlock in list.Items[i].Blocks)
                            {
                                RenderBlock(listItemContext, itemBlock);
                            }
                            context.ApplyBlockContent(listItemContext.ContentWidth, listItemContext.ContentHeight);
                        }
                        break;
                    }

                case MarkdownBlockType.Table:
                    {
                        var table = (TableBlock)block;
                        var columnCount = table.ColumnDefinitions.Count;

                        var headerTextFormat = new CanvasTextFormat();
                        headerTextFormat.FontSize = (float)ParagraphFontSize;
                        headerTextFormat.FontWeight = FontWeights.Bold;
                        var regularTextFormat = new CanvasTextFormat();
                        regularTextFormat.FontSize = (float)ParagraphFontSize;

                        context.ApplyTopAndBottomMargins(5);

                        // Measure the width of each column.
                        var measuredColumnWidths = new double[columnCount];
                        for (int colIndex = 0; colIndex < columnCount; colIndex++)
                        {
                            // Take the max of all the rows.
                            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex ++)
                            {
                                var row = table.Rows[rowIndex];
                                if (row.Cells.Count <= colIndex)
                                    continue;

                                // Measure the cell contents.
                                var context2 = new DrawingContext(context.Device, new Size(double.PositiveInfinity, double.PositiveInfinity));
                                RenderInlines(context2, row.Cells[colIndex].Inlines, rowIndex == 0 ? headerTextFormat : regularTextFormat);
                                measuredColumnWidths[colIndex] = Math.Max(measuredColumnWidths[colIndex], context2.ContentWidth);
                            }
                        }

                        // Calculate the available content width, excluding padding and borders.
                        double borderAndPaddingWidth = (columnCount + 1) * TableBorderThickness + 2 * columnCount * TableCellPadding.Width;
                        double remainingContentWidth = context.AvailableWidth - borderAndPaddingWidth;

                        // If the table would be wider than the available width, shrink the columns so they fit.
                        var columnWidths = new double[columnCount];
                        int remainingColumnCount = columnCount;
                        while (remainingColumnCount > 0)
                        {
                            // Calculate the fair width of all columns.
                            double fairWidth = remainingContentWidth / remainingColumnCount;

                            // Are there any columns less than that?  If so, they get what they are asking for.
                            bool recalculationNeeded = false;
                            for (int i = 0; i < columnCount; i++)
                            {
                                if (columnWidths[i] == 0 && measuredColumnWidths[i] < fairWidth)
                                {
                                    columnWidths[i] = measuredColumnWidths[i];
                                    remainingColumnCount--;
                                    remainingContentWidth -= columnWidths[i];
                                    recalculationNeeded = true;
                                }
                            }

                            // If there are no columns less than the fair width, every remaining column gets that width.
                            if (recalculationNeeded == false)
                            {
                                for (int i = 0; i < columnCount; i++)
                                {
                                    if (columnWidths[i] == 0)
                                    {
                                        columnWidths[i] = fairWidth;
                                    }
                                }
                                break;
                            }
                        }

                        // Render each row.
                        double tableWidth = columnWidths.Sum() + borderAndPaddingWidth;
                        double initialY = context.Y;
                        for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                        {
                            var row = table.Rows[rowIndex];

                            // Draw the row border.
                            if (!context.MeasureOnly)
                            {
                                context.DrawingSession.FillRectangle(new Rect(context.X, context.Y, tableWidth, TableBorderThickness), TableBorderColor);
                            }

                            context.ApplyBlockContent(tableWidth, TableBorderThickness + TableCellPadding.Height);
                            double cellX = context.X;
                            double measuredRowHeight = 0;
                            for (int colIndex = 0; colIndex < columnCount; colIndex++)
                            {
                                if (row.Cells.Count <= colIndex)
                                    continue;
                                cellX += TableBorderThickness + TableCellPadding.Width;

                                var textFormat = rowIndex == 0 ? headerTextFormat : regularTextFormat;
                                switch (table.ColumnDefinitions[colIndex].Alignment)
                                {
                                    case ColumnAlignment.Center:
                                        textFormat.HorizontalAlignment = CanvasHorizontalAlignment.Center;
                                        break;
                                    case ColumnAlignment.Right:
                                        textFormat.HorizontalAlignment = CanvasHorizontalAlignment.Right;
                                        break;
                                    default:
                                        textFormat.HorizontalAlignment = CanvasHorizontalAlignment.Left;
                                        break;
                                }

                                var cellContext = context.Clone(cellX, columnWidths[colIndex]);
                                RenderInlines(cellContext, row.Cells[colIndex].Inlines, textFormat);
                                measuredRowHeight = Math.Max(measuredRowHeight, cellContext.ContentHeight);
                                cellX += columnWidths[colIndex] + TableCellPadding.Width;
                            }
                            context.ApplyBlockContent(tableWidth, measuredRowHeight + TableCellPadding.Height);
                        }

                        // Draw the column borders and the bottom border.
                        if (!context.MeasureOnly)
                        {
                            double x = context.X;
                            for (int colIndex = 0; colIndex < columnCount; colIndex++)
                            {
                                context.DrawingSession.FillRectangle(new Rect(x, initialY, TableBorderThickness, context.Y - initialY), TableBorderColor);
                                x += TableBorderThickness + columnWidths[colIndex] + 2 * TableCellPadding.Width;
                            }
                            context.DrawingSession.FillRectangle(new Rect(x, initialY, TableBorderThickness, context.Y - initialY), TableBorderColor);
                            context.DrawingSession.FillRectangle(new Rect(context.X, context.Y, tableWidth, TableBorderThickness), TableBorderColor);
                        }
                        context.ApplyBlockContent(tableWidth, TableBorderThickness);
                        break;
                    }
            }
        }

        private class CustomBrush
        {
            public int SuperscriptLevel { get; set; }

            public CustomBrush(CustomBrush previousCustomBrush)
            {
                if (previousCustomBrush != null)
                {
                    SuperscriptLevel = previousCustomBrush.SuperscriptLevel;
                }
            }
        }

        private class CustomTextRenderer : ICanvasTextRenderer
        {
            private MarkdownRenderer renderer;
            private DrawingContext context;
            private ICanvasBrush defaultBrush;

            public CustomTextRenderer(MarkdownRenderer renderer, DrawingContext context)
            {
                this.renderer = renderer;
                this.context = context;
                this.defaultBrush = new CanvasSolidColorBrush(this.context.Device, this.renderer.ForegroundColor);
            }

            /// <summary>
            /// Gets the current DPI.
            /// </summary>
            public float Dpi
            {
                get { return this.context.DrawingSession.Dpi; }
            }

            /// <summary>
            /// Gets whether pixel snapping is disabled.
            /// </summary>
            public bool PixelSnappingDisabled
            {
                get { return false; }
            }

            /// <summary>
            /// Gets the transform with which text should be drawn.
            /// </summary>
            public Matrix3x2 Transform
            {
                get { return Matrix3x2.Identity; }
            }

            /// <summary>
            /// Draw a sequence of identically-formatted text characters.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="fontFace"></param>
            /// <param name="fontSize"></param>
            /// <param name="glyphs"></param>
            /// <param name="isSideways"></param>
            /// <param name="bidiLevel"></param>
            /// <param name="brush"></param>
            /// <param name="measuringMode"></param>
            /// <param name="localeName"></param>
            /// <param name="textString"></param>
            /// <param name="clusterMapIndices"></param>
            /// <param name="characterIndex"></param>
            /// <param name="glyphOrientation"></param>
            public void DrawGlyphRun(Vector2 point, CanvasFontFace fontFace, float fontSize, CanvasGlyph[] glyphs, bool isSideways, uint bidiLevel, object brush, CanvasTextMeasuringMode measuringMode, string localeName, string textString, int[] clusterMapIndices, uint characterIndex, CanvasGlyphOrientation glyphOrientation)
            {
                if (glyphs == null)
                    return;

                // Custom handling for inline code spans and superscript.
                var customBrush = brush as CustomBrush;
                if (customBrush != null)
                {
                    if (customBrush.SuperscriptLevel > 0)
                    {
                        float originalFontSize = fontSize / (float)Math.Pow(this.renderer.SuperscriptSize, customBrush.SuperscriptLevel);
                        float multiplier = 1.0f;
                        for (int i = 0; i < customBrush.SuperscriptLevel; i++)
                        {
                            point = new Vector2(point.X + fontFace.SuperscriptPosition.X * originalFontSize,
                                point.Y - fontFace.SuperscriptPosition.Y * originalFontSize * multiplier);
                            multiplier *= (float)this.renderer.SuperscriptSize;
                        }
                    }
                    
                    //if (customBrush.RenderAsCode)
                    //    this.context.DrawingSession.DrawRectangle()

                }
                


                this.context.DrawingSession.DrawGlyphRun(point, fontFace, fontSize, glyphs, isSideways, bidiLevel, brush as ICanvasBrush ?? this.defaultBrush, measuringMode, localeName, textString, clusterMapIndices, characterIndex);
            }

            /// <summary>
            /// Draw an inline object.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="inlineObject"></param>
            /// <param name="isSideways"></param>
            /// <param name="isRightToLeft"></param>
            /// <param name="brush"></param>
            /// <param name="glyphOrientation"></param>
            public void DrawInlineObject(Vector2 point, ICanvasTextInlineObject inlineObject, bool isSideways, bool isRightToLeft, object brush, CanvasGlyphOrientation glyphOrientation)
            {
                //throw new NotImplementedException();
            }

            /// <summary>
            /// Draw a strikethrough.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="strikethroughWidth"></param>
            /// <param name="strikethroughThickness"></param>
            /// <param name="strikethroughOffset"></param>
            /// <param name="textDirection"></param>
            /// <param name="brush"></param>
            /// <param name="textMeasuringMode"></param>
            /// <param name="localeName"></param>
            /// <param name="glyphOrientation"></param>
            public void DrawStrikethrough(Vector2 point, float strikethroughWidth, float strikethroughThickness, float strikethroughOffset, CanvasTextDirection textDirection, object brush, CanvasTextMeasuringMode textMeasuringMode, string localeName, CanvasGlyphOrientation glyphOrientation)
            {
                this.context.DrawingSession.FillRectangle(new Rect(point.X, point.Y + strikethroughOffset, strikethroughWidth, strikethroughThickness), brush as ICanvasBrush ?? this.defaultBrush);
            }

            /// <summary>
            /// Draw an underline.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="underlineWidth"></param>
            /// <param name="underlineThickness"></param>
            /// <param name="underlineOffset"></param>
            /// <param name="runHeight"></param>
            /// <param name="textDirection"></param>
            /// <param name="brush"></param>
            /// <param name="textMeasuringMode"></param>
            /// <param name="localeName"></param>
            /// <param name="glyphOrientation"></param>
            public void DrawUnderline(Vector2 point, float underlineWidth, float underlineThickness, float underlineOffset, float runHeight, CanvasTextDirection textDirection, object brush, CanvasTextMeasuringMode textMeasuringMode, string localeName, CanvasGlyphOrientation glyphOrientation)
            {
                this.context.DrawingSession.FillRectangle(new Rect(point.X, point.Y + underlineOffset, underlineWidth, underlineThickness), brush as ICanvasBrush ?? this.defaultBrush);
            }
        }

        private void RenderInlines(DrawingContext context, IList<MarkdownInline> inlines, CanvasTextFormat textFormat,
            Action<CanvasTextLayout, int> initTextLayout = null)
        {
            var size = RenderOrMeasureInlines(context, inlines, textFormat, initTextLayout);
            context.ApplyBlockContent(size.Width, size.Height);
        }

        private Size RenderOrMeasureInlines(DrawingContext context, IList<MarkdownInline> inlines, CanvasTextFormat textFormat,
            Action<CanvasTextLayout, int> initTextLayout = null)
        {
            // Concatenate the inline text together into one string.
            var inlineTextBuilder = new StringBuilder();
            ConcatText(inlineTextBuilder, inlines);

            using (var textLayout = new CanvasTextLayout(context.Device, inlineTextBuilder.ToString(), textFormat, (float)context.AvailableWidth, 0))
            {
                if (initTextLayout != null)
                    initTextLayout(textLayout, inlineTextBuilder.Length);
                SetTextLayoutProperties(textLayout, inlines, 0);

                if (!context.MeasureOnly)
                {
                    textLayout.DrawToTextRenderer(new CustomTextRenderer(this, context), (float)context.X, (float)context.Y);
                }

                return new Size(textLayout.LayoutBounds.Width, textLayout.LayoutBounds.Height);
            }
        }

        private void ConcatText(StringBuilder result, IList<MarkdownInline> inlines)
        {
            foreach (var inline in inlines)
            {
                switch (inline.Type)
                {
                    case MarkdownInlineType.Bold:
                        ConcatText(result, ((BoldTextInline)inline).Inlines);
                        break;
                    case MarkdownInlineType.Code:
                        result.Append(((CodeInline)inline).Text);
                        break;
                    case MarkdownInlineType.Italic:
                        ConcatText(result, ((ItalicTextInline)inline).Inlines);
                        break;
                    case MarkdownInlineType.MarkdownLink:
                        ConcatText(result, ((MarkdownLinkInline)inline).Inlines);
                        break;
                    case MarkdownInlineType.RawHyperlink:
                        result.Append(((RawHyperlinkInline)inline).Url);
                        break;
                    case MarkdownInlineType.RawSubreddit:
                        result.Append(((RawSubredditInline)inline).Text);
                        break;
                    case MarkdownInlineType.Strikethrough:
                        ConcatText(result, ((StrikethroughTextInline)inline).Inlines);
                        break;
                    case MarkdownInlineType.Superscript:
                        ConcatText(result, ((SuperscriptTextInline)inline).Inlines);
                        break;
                    case MarkdownInlineType.TextRun:
                        result.Append(((TextRunInline)inline).Text);
                        break;
                }
            }

            // Implement whitespace rules similar to HTML: remove leading whitespace characters and
            // collapse inline whitespace characters to a single space character.  Instead of
            // removing excess whitespace characters we actually just replace them with zero-width
            // spaces (it's faster).
            int whitespaceCount = 0;
            for (int i = 0; i < result.Length; i ++)
            {
                if (Common.IsWhiteSpace(result[i]))
                {
                    whitespaceCount++;
                    if (i == 0 || whitespaceCount >= 2)
                        result[i] = (char)0x200B;
                }
                else
                    whitespaceCount = 0;
            }
        }

        private void SetTextLayoutProperties(CanvasTextLayout textLayout, IList<MarkdownInline> inlines, int startIndex)
        {
            foreach (var inline in inlines)
            {
                int textLength = 0;
                switch (inline.Type)
                {
                    case MarkdownInlineType.Bold:
                        textLength = ComputeTextLength(((BoldTextInline)inline).Inlines);
                        textLayout.SetFontWeight(startIndex, textLength, FontWeights.Bold);
                        SetTextLayoutProperties(textLayout, ((BoldTextInline)inline).Inlines, startIndex);
                        break;
                    case MarkdownInlineType.Code:
                        textLength = ((CodeInline)inline).Text.Length;
                        textLayout.SetFontFamily(startIndex, textLength, CodeFontFamily);
                        break;
                    case MarkdownInlineType.Italic:
                        textLength = ComputeTextLength(((ItalicTextInline)inline).Inlines);
                        textLayout.SetFontStyle(startIndex, textLength, FontStyle.Italic);
                        SetTextLayoutProperties(textLayout, ((ItalicTextInline)inline).Inlines, startIndex);
                        break;
                    case MarkdownInlineType.MarkdownLink:
                        textLength = ComputeTextLength(((MarkdownLinkInline)inline).Inlines);
                        textLayout.SetColor(startIndex, textLength, LinkColor);
                        SetTextLayoutProperties(textLayout, ((MarkdownLinkInline)inline).Inlines, startIndex);
                        break;
                    case MarkdownInlineType.RawHyperlink:
                        textLength = ((RawHyperlinkInline)inline).Url.Length;
                        textLayout.SetColor(startIndex, textLength, LinkColor);
                        break;
                    case MarkdownInlineType.RawSubreddit:
                        textLength = ((RawSubredditInline)inline).Text.Length;
                        textLayout.SetColor(startIndex, textLength, LinkColor);
                        break;
                    case MarkdownInlineType.Strikethrough:
                        textLength = ComputeTextLength(((StrikethroughTextInline)inline).Inlines);
                        textLayout.SetStrikethrough(startIndex, textLength, hasStrikethrough: true);
                        SetTextLayoutProperties(textLayout, ((StrikethroughTextInline)inline).Inlines, startIndex);
                        break;
                    case MarkdownInlineType.Superscript:
                        textLength = ComputeTextLength(((SuperscriptTextInline)inline).Inlines);
                        var superscriptBrush = new CustomBrush(textLayout.GetCustomBrush(startIndex) as CustomBrush);
                        superscriptBrush.SuperscriptLevel++;
                        textLayout.SetCustomBrush(startIndex, textLength, superscriptBrush);
                        textLayout.SetFontSize(startIndex, textLength, textLayout.GetFontSize(startIndex) * (float)this.SuperscriptSize);
                        SetTextLayoutProperties(textLayout, ((SuperscriptTextInline)inline).Inlines, startIndex);
                        break;
                    case MarkdownInlineType.TextRun:
                        textLength = ((TextRunInline)inline).Text.Length;
                        break;
                }
                startIndex += textLength;
            }
        }

        private int ComputeTextLength(IList<MarkdownInline> inlines)
        {
            int result = 0;
            foreach (var inline in inlines)
            {
                switch (inline.Type)
                {
                    case MarkdownInlineType.Bold:
                        result += ComputeTextLength(((BoldTextInline)inline).Inlines);
                        break;
                    case MarkdownInlineType.Code:
                        result += ((CodeInline)inline).Text.Length;
                        break;
                    case MarkdownInlineType.Italic:
                        result += ComputeTextLength(((ItalicTextInline)inline).Inlines);
                        break;
                    case MarkdownInlineType.MarkdownLink:
                        result += ComputeTextLength(((MarkdownLinkInline)inline).Inlines);
                        break;
                    case MarkdownInlineType.RawHyperlink:
                        result += ((RawHyperlinkInline)inline).Url.Length;
                        break;
                    case MarkdownInlineType.RawSubreddit:
                        result += ((RawSubredditInline)inline).Text.Length;
                        break;
                    case MarkdownInlineType.Strikethrough:
                        result += ComputeTextLength(((StrikethroughTextInline)inline).Inlines);
                        break;
                    case MarkdownInlineType.Superscript:
                        result += ComputeTextLength(((SuperscriptTextInline)inline).Inlines);
                        break;
                    case MarkdownInlineType.TextRun:
                        result += ((TextRunInline)inline).Text.Length;
                        break;
                }
            }
            return result;
        }
    }
}
