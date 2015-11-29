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
    class LineBreakBlock : MarkdownBlock
    {
        public LineBreakBlock() 
            : base(MarkdownBlockType.LineBreak)
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
            // Get the end.
            int nbspEnd = TryToFindNbsp(ref markdown, startingPos, maxEndingPos);

            // Sanity check
            if (nbspEnd == -1)
            {
                DebuggingReporter.ReportCriticalError("Tried parse line break find a &nbps;");
                return maxEndingPos;
            }      

            // Trim off any extra line endings, except ' ' otherwise we can't do code blocks
            while (nbspEnd < markdown.Length && nbspEnd < maxEndingPos && Char.IsWhiteSpace(markdown[nbspEnd]) && markdown[nbspEnd] != ' ')
            {
                nbspEnd++;
            }       

            // Return where we ended.
            return nbspEnd;
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
            return TryToFindNbsp(ref markdown, nextCharPos, endingPos) != -1;
        }

        /// <summary>
        /// attempts to find a nbsp or multiple and if found returns the index where they end.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPos"></param>
        /// <returns></returns>
        private static int TryToFindNbsp(ref string markdown, int startingPos, int endingPos)
        {
            int currentPos = startingPos;
            bool nonBreakingSpaceFound = false;

            // We need to loop though and find all of the nbsp; if one or more are found with a \n \r after it we have a line break.
            while (currentPos < markdown.Length && currentPos < endingPos)
            {
                // If we found one iterate and see if we find another.
                if (markdown.IndexOf("&nbsp;", currentPos) == currentPos)
                {
                    currentPos += 6;
                    nonBreakingSpaceFound = true;
                }
                // If we found a \n or \r figure out if we are good.
                else if (markdown[currentPos] == '\n' || markdown[currentPos] == '\r')
                {
                    return nonBreakingSpaceFound ? currentPos : -1;
                }
                // Anything else just quit
                else
                {
                    return -1;
                }
            }

            return -1;
        }
    }
}
