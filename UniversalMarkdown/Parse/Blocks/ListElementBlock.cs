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
    public class ListElementBlock : MarkdownBlock
    {
        public int ListIndent = 0;

        public string ListBullet = "•";

        public ListElementBlock() 
            : base(MarkdownBlockType.ListElement)
        { }

        /// <summary>
        /// Called when this block type should parse out the goods. Given the markdown, a starting point, and a max ending point
        /// the block should find the start of the block, find the end and parse out the middle. The end most of the time will not be
        /// the max ending pos, but it sometimes can be. The funciton will return where it ended parsing the block in the markdown.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="maxEndingPos"></param>
        /// <returns></returns>
        internal override int Parse(ref string markdown, int startingPos, int maxEndingPos)
        {
            // Find where the list begins
            int listStart = startingPos;
            while (listStart < markdown.Length && listStart < maxEndingPos)
            {
                if (markdown[listStart] == ' ')
                {
                    ListIndent++;
                }
                // We have a bullet list
                else if (markdown[listStart] == '*' || markdown[listStart] == '-')
                {
                    break;
                }
                // We have a number or letter list
                else if(Char.IsLetterOrDigit(markdown[listStart]))
                {
                    // Grab the list letter
                    ListBullet = markdown[listStart] + ".";

                    // +1 to move past the .
                    listStart++;
                    break;
                }
                listStart++;
            }

            // Find the end of the list
            int listEnd = Common.FindNextNewLine(ref markdown, listStart, maxEndingPos);
            if (listEnd == -1)
            {
                DebuggingReporter.ReportCriticalError("Tried to parse list that didn't have an end");
                listEnd = maxEndingPos;
            }

            // Remove one indent from the list. This doesn't work exactly like reddit's
            // but it is close enough
            ListIndent = Math.Max(1, ListIndent - 1);

            // Jump past the *
            listStart++;

            // Make sure there is something to parse, and not just dead space
            if (listEnd > listStart)
            {
                // Parse the children of this list
                ParseInlineChildren(ref markdown, listStart, listEnd);
            }

            // Trim off any extra line endings, except ' ' otherwise we can't do code blocks
            while (listEnd < markdown.Length && listEnd < maxEndingPos && Char.IsWhiteSpace(markdown[listEnd]) && markdown[listEnd] != ' ')
            {
                listEnd++;
            }

            // Return where we ended.
            return listEnd;
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
            if (markdown.Length > nextCharPos + 1 && endingPos > nextCharPos + 1)
            {
                // Check for * and - followed by space
                char test = markdown[nextCharPos + 1];
                if ((markdown[nextCharPos] == '*' || markdown[nextCharPos] == '-') && markdown[nextCharPos + 1] == ' ')
                {
                    return true;
                }
                // Check for a ltter or digit followed by a .
                if(Char.IsLetterOrDigit(markdown[nextCharPos]) && markdown[nextCharPos + 1] == '.')
                {
                    return true;
                }
            }
            return false;
        }
    }
}
