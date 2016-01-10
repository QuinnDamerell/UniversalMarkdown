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
    public class ItalicTextInline : MarkdownInline
    {
        /// <summary>
        /// The contents of the inline.
        /// </summary>
        public IList<MarkdownInline> Inlines { get; set; }

        public ItalicTextInline()
            : base(MarkdownInlineType.Italic)
        { }


        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        internal static void AddTripChars(List<InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = '*', Type = MarkdownInlineType.Italic });
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = '_', Type = MarkdownInlineType.Italic });
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
        internal override int Parse(string markdown, int startingPos, int endingPos)
        {
            var innerStart = startingPos + 1;
            if (markdown[startingPos] != '*' && markdown[startingPos] != '_')
            {
                DebuggingReporter.ReportCriticalError("Parse didn't find * or _ in at the starting pos");
            }

            var innerEnd = endingPos - 1;
            if (markdown[innerEnd] != '*' && markdown[innerEnd] != '_')
            {
                DebuggingReporter.ReportCriticalError("Parse didn't find * or _ in at the end pos");
            }

            // Make sure there is something to parse, and not just dead space
            if (innerEnd > innerStart)
            {
                // Parse any children.
                Inlines = ParseInlineChildren(markdown, innerStart, innerEnd);
            }

            return endingPos;
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
        public static bool VerifyMatch(string markdown, int startingPos, int maxEndingPos, ref int elementStartingPos, ref int elementEndingPos)
        {
            // Do a sanity check.
            char startChar = markdown[startingPos];
            if (startChar != '*' && startChar != '_')
                return false;

            // Find the end of the span.  The end character (either '*' or '_') must be the same as
            // the start character.
            int innerEnd = Common.IndexOf(markdown, startChar, startingPos + 1, maxEndingPos);
            if (innerEnd == -1)
                return false;

            // The span must contain at least one character.
            var innerStart = startingPos + 1;
            if (innerStart == innerEnd)
                return false;

            // The first character inside the span must NOT be a space.
            if (Common.IsWhiteSpace(markdown[innerStart]))
                return false;

            // The last character inside the span must NOT be a space.
            if (Common.IsWhiteSpace(markdown[innerEnd - 1]))
                return false;

            elementStartingPos = startingPos;
            elementEndingPos = innerEnd + 1;
            return true;
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            if (Inlines == null)
                return base.ToString();
            return "*" + string.Join(string.Empty, Inlines) + "*";
        }
    }
}
