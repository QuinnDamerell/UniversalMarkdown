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


using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.UI;
using Windows.UI;
using UniversalMarkdown.Parse;
using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using UniversalMarkdown.Parse.Elements;
using System.Numerics;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.Text;
using System.Text;
using Windows.UI.Text;
using System;
using UniversalMarkdown.Display;

namespace UniversalMarkdown
{
    public sealed partial class MarkdownTextBlock : UserControl
    {
        private MarkdownRenderer renderer;

        /// <summary>
        /// Creates a new markdown text block.
        /// </summary>
        public MarkdownTextBlock()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The markdown text to display.
        /// </summary>
        public string Markdown
        {
            get { return (string)GetValue(MarkdownProperty); }
            set { SetValue(MarkdownProperty, value); }
        }

        public static readonly DependencyProperty MarkdownProperty =
            DependencyProperty.Register(
                "Markdown",
                typeof(string),
                typeof(MarkdownTextBlock),
                new PropertyMetadata("", new PropertyChangedCallback(OnMarkdownChangedStatic)
                ));

        private static void OnMarkdownChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as MarkdownTextBlock;
            if (instance != null)
            {
                // Send the post to the class.
                instance.OnMarkdownChanged((string)e.NewValue);
            }
        }


        /// <summary>
        /// Gets or sets the amount of space between paragraphs.
        /// </summary>
        public double ParagraphSpacing
        {
            get { return (double)GetValue(ParagraphSpacingProperty); }
            set { SetValue(ParagraphSpacingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParagraphSpacing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParagraphSpacingProperty =
            DependencyProperty.Register("ParagraphSpacing", typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(5.0));




        /// <summary>
        /// Fired when the markdown is changed. 
        /// </summary>
        /// <param name="newMarkdown"></param>
        private void OnMarkdownChanged(string newMarkdown)
        {
            var markdownTree = new Parse.Markdown();
            markdownTree.Parse(Markdown);

            this.renderer = new MarkdownRenderer(markdownTree);
            this.renderer.ParagraphFontSize = FontSize;
            this.renderer.ParagraphSpacing = ParagraphSpacing;

            InvalidateMeasure();
            Canvas.Invalidate();
        }

        /// <summary>
        /// Provides the behavior for the "Measure" pass of the layout cycle. Classes can override
        /// this method to define their own "Measure" pass behavior.
        /// </summary>
        /// <param name="availableSize"> The available size that this object can give to child
        /// objects. Infinity can be specified as a value to indicate that the object will size
        /// to whatever content is available. </param>
        /// <returns> The size that this object determines it needs during layout, based on its
        /// calculations of the allocated sizes for child objects or based on other considerations
        /// such as a fixed container size. </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.renderer == null)
                return new Size(0, 0);
            return this.renderer.Measure(CanvasDevice.GetSharedDevice(), availableSize);
        }

        private void Canvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {

        }

        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.renderer.Draw(args.DrawingSession, new Rect(Padding.Left, Padding.Top,
                ActualWidth - Padding.Left - Padding.Right,
                ActualHeight - Padding.Top - Padding.Bottom));
            //args.DrawingSession.DrawGlyphRun(new Vector2(0, 16), )
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Canvas.RemoveFromVisualTree();
            Canvas = null;
        }
    }
}
