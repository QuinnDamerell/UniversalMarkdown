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
using System.Collections.Generic;
using UniversalMarkdown.Parse.Elements;
using Windows.Foundation;
using Windows.UI;
using System.Numerics;
using Microsoft.Graphics.Canvas.Brushes;
using System;

namespace UniversalMarkdown.Display
{

    /// <summary>
    /// Represents a link element that has been rendered.
    /// </summary>
    public class RenderedLink
    {
        internal class GlyphRun
        {
            public Rect Rect;
            public Vector2 Point;
            public CanvasFontFace FontFace;
            public float FontSize;
            public CanvasGlyph[] Glyphs;
            public bool IsSideways;
            public uint BidiLevel;
            public ICanvasBrush Brush;
            public CanvasTextMeasuringMode MeasuringMode;
            public string LocaleName;
            public string TextString;
            public int[] ClusterMapIndices;
            public uint CharacterIndex;

            /// <summary>
            /// Draws the rendered link again, using the given color.
            /// </summary>
            /// <param name="drawingSession"> The drawing session to draw with. </param>
            /// <param name="linkColor"> The color of the link text. </param>
            public void Draw(CanvasDrawingSession drawingSession, Color linkColor)
            {
                using (var brush = new CanvasSolidColorBrush(drawingSession, linkColor))
                {
                    drawingSession.DrawGlyphRun(this.Point, this.FontFace, this.FontSize, this.Glyphs,
                        this.IsSideways, this.BidiLevel, brush, this.MeasuringMode, this.LocaleName,
                        this.TextString, this.ClusterMapIndices, this.CharacterIndex);
                }
            }
        }

        /// <summary>
        /// Creates a new RenderedLink instance.
        /// </summary>
        /// <param name="linkElement"></param>
        public RenderedLink(ILinkElement linkElement)
        {
            this.LinkElement = linkElement;
            this.GlyphRuns = new List<GlyphRun>();
        }

        /// <summary>
        /// A reference to the parsed link.
        /// </summary>
        public ILinkElement LinkElement { get; set; }

        /// <summary>
        /// A list of glyph runs that can be used to draw the element.
        /// </summary>
        internal IList<GlyphRun> GlyphRuns { get; private set; }
    }

}
