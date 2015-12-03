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
    class HeaderBlock : MarkdownBlock
    {
        public int HeaderLevel = 0;

        public HeaderBlock()
            : base(MarkdownBlockType.Header)
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
            // Do a quick check
            int headerStart = startingPos;
            if (markdown[headerStart] != '#')
            {
                DebuggingReporter.ReportCriticalError("Tried to parse a header but # wasn't found");
            }

            // Find the end of header, note that headers break with a single new line no matter what.
            int headerEnd = Common.FindNextSingleNewLine(ref markdown, headerStart, maxEndingPos);
            if (headerEnd == -1)
            {
                DebuggingReporter.ReportCriticalError("Tried to parse header that didn't have an end");
                headerEnd = maxEndingPos;
            }

            // Find how many are in a row
            while (headerStart < markdown.Length && headerStart < maxEndingPos && markdown[headerStart] == '#')
            {
                HeaderLevel++;
                headerStart++;

                // To match reddit's formatting if there are more than 6 we should start showing them.
                if (HeaderLevel > 5)
                {
                    break;
                }
            }

            // Make sure there is something to parse, and not just dead space
            if (headerEnd > headerStart)
            {
                // Parse the children of this quote
                ParseInlineChildren(ref markdown, headerStart, headerEnd);
            }

            // Trim off any extra line endings, except ' ' otherwise we can't do code blocks
            while (headerEnd < markdown.Length && headerEnd < maxEndingPos && Char.IsWhiteSpace(markdown[headerEnd]) && markdown[headerEnd] != ' ')
            {
                headerEnd++;
            }

            // Return where we ended.
            return headerEnd;
        }


        /// <summary>
        /// Called to determine if this block type can handle the next block.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="nextCharPos"></param>
        /// <param name="endingPos"></param>
        /// <returns></returns>
        public static bool CanHandleBlock(ref string markdown, int nextCharPos, int endingPos)
        {
            return markdown.Length > nextCharPos && endingPos > nextCharPos && markdown[nextCharPos] == '#';
        }
    }
}
