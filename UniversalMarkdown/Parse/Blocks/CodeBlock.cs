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
    class CodeBlock : MarkdownBlock
    {
        public int CodeIndent = 0;

        public CodeBlock()
            : base(MarkdownBlockType.Code)
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
            // Find where the code begins, since we are given the line after the last space we actually need
            // to go backwards.
            int spaceCountStart = startingPos - 1;
            int spaceCount = 0;
            while (spaceCountStart >= 0)
            {
                // If we found a space count it
                if (markdown[spaceCountStart] == ' ')
                {
                    spaceCount++;
                }
                else
                {
                    if (spaceCount > 3)
                    {
                        // We found the next char after the code begin
                        break;
                    }
                    else
                    {
                        // We found a char that broke the space count
                        spaceCount = 0;
                    }
                }
                spaceCountStart--;
            }

            if (spaceCount == 0)
            {
                DebuggingReporter.ReportCriticalError("Tried to code but found no space row > 3");
            }

            // Find the end of code, note code breaks with a single new line no matter what.
            int codeEnd = Common.FindNextSingleNewLine(ref markdown, startingPos, maxEndingPos);
            if(codeEnd == -1)
            {
                DebuggingReporter.ReportCriticalError("Tried to code quote that didn't have an end");
                codeEnd = maxEndingPos;
            }

            // For every 4 spaces we want to add an indent
            CodeIndent = (int)Math.Floor(spaceCount / 4.0);

            // Make sure there is something to parse, and not just dead space
            if (codeEnd > startingPos)
            {
                // Parse the children of this quote
                ParseInlineChildren(ref markdown, startingPos, codeEnd);
            }

            // Trim off any extra line endings, except ' ' otherwise we can't do code blocks
            while (codeEnd < markdown.Length && codeEnd < maxEndingPos && Char.IsWhiteSpace(markdown[codeEnd]) && markdown[codeEnd] != ' ')
            {
                codeEnd++;
            }

            // Return where we ended.
            return codeEnd;
        }

        /// <summary>
        /// Called to see if the code block can handle this next block
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="nextCharPos"></param>
        /// <param name="endingPos"></param>
        /// <param name="spaceCount"></param>
        /// <returns></returns>
        public static bool CanHandleBlock(ref string markdown, int nextCharPos, int endingPos, int spaceCount)
        {
            // Check the spaces and ensure the next char isn't a list element or quote.
            return spaceCount > 3 && markdown[nextCharPos] != '*' && markdown[nextCharPos] != '-' && markdown[nextCharPos] != '>';
        }
    }
}
