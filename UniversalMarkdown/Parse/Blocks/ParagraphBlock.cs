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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Helpers;

namespace UniversalMarkdown.Parse.Elements
{
    public class ParagraphBlock : MarkdownBlock
    {
        /// <summary>
        /// The contents of the block.
        /// </summary>
        public IList<MarkdownInline> Inlines { get; set; }

        public ParagraphBlock()
            : base(MarkdownBlockType.Paragraph)
        { }

        /// <summary>
        /// Parses paragraph text.
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location of the start of the paragraph. </param>
        /// <param name="end"> The location of the end of the paragraph. </param>
        /// <returns> A parsed paragraph. </returns>
        internal static ParagraphBlock Parse(string markdown, int start, int end)
        {
            var result = new ParagraphBlock();
            result.Inlines = ParseInlineChildren(markdown, start, end);
            return result;
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            if (Inlines == null)
                return base.ToString();
            return string.Join(string.Empty, Inlines);
        }
    }
}
