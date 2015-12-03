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
    class QuoteBlock : MarkdownBlock
    {
        public int QuoteIndent = 0;

        public QuoteBlock()
            : base(MarkdownBlockType.Quote)
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
            // Do a quick check.
            int quoteStart = startingPos + 1;
            if(markdown[startingPos] != '>')
            {
                DebuggingReporter.ReportCriticalError("Tried to parse quote that didn't exist");
            }

            // Find the end of quote, we always break on a double return no matter what.
            int quoteEnd = Common.FindNextDoubleNewLine(ref markdown, quoteStart, maxEndingPos);
            if(quoteEnd == -1)
            {
                DebuggingReporter.ReportCriticalError("Tried to parse quote that didn't have an end");
                quoteEnd = maxEndingPos;
            }

            // Find how many indents we have, we have to count backwards from the starting pos. Start with one
            // so if we have no spaces we at least get that.
            QuoteIndent = 1;
            int currentBackCount = startingPos - 1;
            while (currentBackCount >= 0 && markdown[currentBackCount] != '\n' && markdown[currentBackCount] != '\r')
            {
                QuoteIndent++;
                currentBackCount--;
            }

            // Make sure there is something to parse, and not just dead space
            if (quoteEnd > quoteStart)
            {
                // Parse the children of this quote
                ParseInlineChildren(ref markdown, quoteStart, quoteEnd);
            }

            // Trim off any extra line endings, except ' ' otherwise we can't do code blocks
            while (quoteEnd < markdown.Length && quoteEnd < maxEndingPos && Char.IsWhiteSpace(markdown[quoteEnd]) && markdown[quoteEnd] != ' ')
            {
                quoteEnd++;
            }

            // Return where we ended.
            return quoteEnd;
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
            return markdown.Length > nextCharPos && endingPos > nextCharPos && markdown[nextCharPos] == '>';
        }
    }
}
