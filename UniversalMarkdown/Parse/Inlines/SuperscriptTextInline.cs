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
    public class SuperscriptTextInline : MarkdownInline
    {
        public SuperscriptTextInline()
            : base(MarkdownInlineType.Superscript)
        { }

        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        internal static InlineTripCharHelper GetTripChars()
        {
            return new InlineTripCharHelper() { FirstChar = '^', FirstCharSuffix = null, Type = MarkdownInlineType.Superscript};
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
            // Sanity check
            if (markdown[startingPos] == '^')
            {
                // The content might be enclosed in parentheses.
                int contentStart = startingPos + 1;
                if (contentStart < maxEndingPos && markdown[contentStart] == '(')
                {
                    // Find the end parenthesis.
                    contentStart++;
                    int contentEnd = Common.IndexOf(ref markdown, ')', contentStart, maxEndingPos);
                    if (contentEnd == -1)
                        return false;

                    // Okay, found it.
                    elementStartingPos = startingPos;
                    elementEndingPos = contentEnd + 1;
                    return true;
                }

                // We might have one, try to find the ending that is in the current endingPos
                elementStartingPos = startingPos;
                elementEndingPos = Common.FindNextWhiteSpace(ref markdown, contentStart, maxEndingPos, ifNotFoundReturnLength: true);
                return true;
            }
            return false;
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
            int contentStart = Common.IndexOf(ref markdown, '^', startingPos, endingPos);
            // These should always be =
            if (contentStart != startingPos)
            {
                DebuggingReporter.ReportCriticalError("superscript parse didn't find ^ in at the starting pos");
            }
            contentStart++;

            // The content might be enclosed in parentheses.
            int contentEnd;
            if (contentStart < endingPos && markdown[contentStart] == '(')
            {
                // Find the end parenthesis.
                contentStart++;
                contentEnd = Common.IndexOf(ref markdown, ')', contentStart, endingPos);
                if (contentEnd == -1)
                {
                    DebuggingReporter.ReportCriticalError("superscript parse didn't find ending )");
                }
            }
            else
            {
                contentEnd = Common.FindNextWhiteSpace(ref markdown, contentStart, endingPos, ifNotFoundReturnLength: true);
            }

            // Make sure there is something to parse, and not just dead space
            if (contentEnd > contentStart)
            {
                // Parse any children of this superscript element
                ParseInlineChildren(ref markdown, contentStart, contentEnd);
            }

            return endingPos;
        }
    }
}
