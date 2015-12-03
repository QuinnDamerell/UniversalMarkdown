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
    class ItalicTextElement : MarkdownInline
    {
        public ItalicTextElement()
            : base(MarkdownInlineType.Italic)
        { }


        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        public static InlineTripCharHelper GetTripChars()
        {
            return new InlineTripCharHelper() { FirstChar = '*', Type = MarkdownInlineType.Italic };
        }

        /// <summary>
        /// Called when the object should parse it's goods out of the markdown. The markdown, start, and stop are given.
        /// The start and stop are what is returned from the FindNext function below. The object should do it's parsing and
        /// return up to the last pos it used. This can be shorter than what is given to the function in endingPos.
        /// </summary>
        /// <param name="markdown">The markdown</param>
        /// <param name="startingPos">Where the parse should start</param>
        /// <param name="endingPos">Where the parse should end</param>
        /// <returns></returns>
        internal override int Parse(ref string markdown, int startingPos, int endingPos)
        {
            int italicStart = Common.IndexOf(ref markdown, '*', startingPos, endingPos);
            // These should always be =
            if(italicStart != startingPos)
            {
                DebuggingReporter.ReportCriticalError("italic parse didn't find * in at the starting pos");
            }
            italicStart++;

            // Find the ending
            int italicEnd = Common.IndexOf(ref markdown, '*', italicStart, endingPos, true);
            if (italicEnd + 1 != endingPos)
            {
                DebuggingReporter.ReportCriticalError("italic parse didn't find * in at the end pos");
            }

            // Make sure there is something to parse, and not just dead space
            if (italicEnd > italicStart)
            {
                // Parse any children of this bold element
                ParseInlineChildren(ref markdown, italicStart, italicEnd);
            }

            // Return the point after the *
            return italicEnd + 1;
        }

        /// <summary>
        /// Verify a match that is found in the markdown. If the match is good and the rest of the element exits the function should
        /// return true and the element will be matched. If if is a false positive return false and we will keep looking.
        /// </summary>
        /// <param name="markdown">The markdown to match</param>
        /// <param name="startingPos">Where the first trip char should be found</param>
        /// <param name="maxEndingPos">The max length to look in.</param>
        /// <param name="elementEndingPos">If found, the ending pos of the element found.</param>
        /// <returns></returns>
        public static bool VerifyMatch(ref string markdown, int startingPos, int maxEndingPos, ref int elementStartingPos, ref int elementEndingPos)
        {
            // Do an sanity check
            if (markdown[startingPos] == '*')
            {
                // We might have one, try to find the ending that is in the current endingPos
                // We need to loop and keep looking for * not followed by another * (bold)
                int italicEndPos = Common.IndexOf(ref markdown, '*', startingPos + 1, maxEndingPos);
                while (italicEndPos != -1 && italicEndPos + 1 < markdown.Length && markdown[italicEndPos + 1] == '*')
                {
                    italicEndPos = Common.IndexOf(ref markdown, '*', italicEndPos + 2, maxEndingPos);
                }

                // If we found it and it is the next closest ending pos use it!
                if (italicEndPos != -1)
                {
                    elementStartingPos = startingPos;
                    elementEndingPos = italicEndPos + 1;
                    return true;
                }
            }
            return false;
        }
    }
}
