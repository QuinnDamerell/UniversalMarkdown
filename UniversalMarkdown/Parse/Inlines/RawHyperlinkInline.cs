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
    public class RawHyperlinkInline : MarkdownInline
    {
        public string Url { get; set; }

        public RawHyperlinkInline()
            : base(MarkdownInlineType.RawHyperlink)
        { }


        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        internal static void AddTripChars(List<InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = '<', Type = MarkdownInlineType.RawHyperlink });
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = 'h', Type = MarkdownInlineType.RawHyperlink, IgnoreEscapeChar = true });
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = 'H', Type = MarkdownInlineType.RawHyperlink, IgnoreEscapeChar = true });
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
            int linkEnd = Common.FindNextWhiteSpace(markdown, startingPos, endingPos, true);
            
            // Grab the link text
            Url = markdown.Substring(startingPos, linkEnd - startingPos);

            // Return the point after the end
            return linkEnd;
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
            // HTTP links.
            if (maxEndingPos - startingPos >= "http://".Length)
            {
                if (string.Equals(markdown.Substring(startingPos, "http://".Length), "http://", StringComparison.OrdinalIgnoreCase))
                {
                    elementStartingPos = startingPos;
                    elementEndingPos = Common.FindNextWhiteSpace(markdown, startingPos, maxEndingPos, true);
                    return true;
                }
            }

            // HTTPS links.
            if (maxEndingPos - startingPos >= "https://".Length)
            {
                if (string.Equals(markdown.Substring(startingPos, "https://".Length), "https://", StringComparison.OrdinalIgnoreCase))
                {
                    elementStartingPos = startingPos;
                    elementEndingPos = Common.FindNextWhiteSpace(markdown, startingPos, maxEndingPos, true);
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            if (Url == null)
                return base.ToString();
            return Url;
        }
    }
}
