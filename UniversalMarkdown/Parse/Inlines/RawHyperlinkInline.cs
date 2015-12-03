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
    class RawHyperlinkInline : MarkdownInline
    {
        public string Url { get; set; }

        public RawHyperlinkInline()
            : base(MarkdownInlineType.RawHyperlink)
        { }


        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        public static InlineTripCharHelper GetTripChars()
        {
            return new InlineTripCharHelper() { FirstChar = 'h', FirstCharSuffix = "ttp", Type = MarkdownInlineType.RawHyperlink};
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
            int httpStart = Common.IndexOf(ref markdown, "http://", startingPos, endingPos);
            int httpsStart = Common.IndexOf(ref markdown, "https://", startingPos, endingPos);

            // Make -1 huge.
            httpStart = httpStart == -1 ? int.MaxValue : httpStart;
            httpsStart = httpsStart == -1 ? int.MaxValue : httpsStart;

            // Figure out the pos of the link
            int linkStart = Math.Min(httpStart, httpsStart);
            int linkEnd = Common.FindNextWhiteSpace(ref markdown, linkStart, endingPos, true);

            // These should always be =
            if (linkStart != startingPos)
            {
                DebuggingReporter.ReportCriticalError("raw link parse didn't find http in at the starting pos");
            }
            if (linkEnd != endingPos)
            {
                DebuggingReporter.ReportCriticalError("raw link parse didn't find the same ending pos");
            }

            // Grab the link text
            Url = markdown.Substring(linkStart, linkEnd - linkStart);

            // Return the point after the end
            return linkEnd + 1;
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
            if(markdown[startingPos] == 'h')
            {
                int httpStart = Common.IndexOf(ref markdown, "http://", startingPos, maxEndingPos);
                int httpsStart = Common.IndexOf(ref markdown, "https://", startingPos, maxEndingPos);

                if (httpsStart != -1 || httpStart != -1)
                {
                    // Make -1 huge.
                    httpStart = httpStart == -1 ? int.MaxValue : httpStart;
                    httpsStart = httpsStart == -1 ? int.MaxValue : httpsStart;

                    // Figure out the pos of the link
                    int foundLinkStart = Math.Min(httpStart, httpsStart);

                    // Set the start and end
                    elementStartingPos = startingPos;
                    elementEndingPos = Common.FindNextWhiteSpace(ref markdown, foundLinkStart, maxEndingPos, true);
                    return true;
                }
            }
            return false;
        }
    }
}
