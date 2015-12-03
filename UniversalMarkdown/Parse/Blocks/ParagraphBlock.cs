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
    class ParagraphBlock : MarkdownBlock
    {
        public ParagraphBlock()
            : base(MarkdownBlockType.Paragraph)
        { }

        /// <summary>
        /// Called when this block type should parse out the goods. Given the markdown, a starting point, and a max ending point
        /// the block should find the start of the block, find the end and parse out the middle. The end most of the time will not be
        /// the max ending pos, but it sometimes can be. The function will return where it ended parsing the block in the markdown.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="maxEndingPos"></param>
        /// <returns></returns>
        internal override int Parse(ref string markdown, int startingPos, int maxEndingPos)
        {
            // Find the end of paragraph, read the summary of the function for details.
            int endingPos = Common.FindNextParagraphLineBreak(ref markdown, startingPos, maxEndingPos);

            // Make sure there is something to parse, and not just dead space
            if (endingPos > startingPos)
            {
                // Parse the children of this paragraph
                ParseInlineChildren(ref markdown, startingPos, endingPos);
            }

            // Trim off any extra line endings, except ' ' otherwise we can't do code blocks
            while (endingPos < markdown.Length && endingPos < maxEndingPos && Char.IsWhiteSpace(markdown[endingPos]) && markdown[endingPos] != ' ')
            {
                endingPos++;
            }

            // Return where we ended.
            return endingPos;
        }
    }
}
