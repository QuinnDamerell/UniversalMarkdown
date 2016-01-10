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
    public class RawSubredditInline : MarkdownInline
    {
        public string Text { get; set; }

        public RawSubredditInline()
            : base(MarkdownInlineType.RawSubreddit)
        { }

        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        internal static void AddTripChars(List<InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = '/', Type = MarkdownInlineType.RawSubreddit });
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = 'r', Type = MarkdownInlineType.RawSubreddit, IgnoreEscapeChar = true });
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
            Text = markdown.Substring(startingPos, endingPos - startingPos);
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
            // The link may or may not start with '/'.
            int pos = startingPos;
            if (pos == maxEndingPos)
                return false;
            if (markdown[pos] == '/')
            {
                pos++;
                if (pos == maxEndingPos)
                    return false;
            }

            // Expect 'r'.
            if (pos == maxEndingPos || markdown[pos] != 'r')
                return false;
            pos++;

            // Expect '/'
            if (pos == maxEndingPos || markdown[pos] != '/')
                return false;
            pos++;

            // Expect at least one other letter or digit.
            if (pos == maxEndingPos || !char.IsLetterOrDigit(markdown[pos]))
                return false;
            pos++;

            // Good enough.
            elementStartingPos = startingPos;
            elementEndingPos = Common.FindNextNonLetterDigitOrUnderscore(markdown, pos, maxEndingPos, true);
            return true;
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            if (Text == null)
                return base.ToString();
            return Text;
        }
    }
}
