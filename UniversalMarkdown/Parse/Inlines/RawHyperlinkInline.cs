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
        /// Attempts to find a element in the range given. If an element is found we must check if the starting is less than currentNextElementStart,
        /// and if so update that value to be the start and update the elementEndPos to be the end of the element. These two vales will be passed back to us
        /// when we are asked to parse. We then return true or false to indicate if we are the new candidate. 
        /// </summary>
        /// <param name="markdown">mark down to parse</param>
        /// <param name="currentPos">the starting point to search</param>
        /// <param name="maxEndingPos">the ending point to search</param>
        /// <param name="elementStartingPos">the current starting element, if this element is < we will update this to be our starting pos</param>
        /// <param name="elementEndingPos">The ending pos of this element if it is interesting.</param>
        /// <returns>true if we are the next element candidate, false otherwise.</returns>
        public static bool FindNextClosest(ref string markdown, int startingPos, int endingPos, ref int currentNextElementStart, ref int elementEndingPos)
        {
            // Test for raw hyper links
            int linkStart = Common.IndexOf(ref markdown, "http", startingPos, endingPos);
            if (linkStart != -1 && linkStart < currentNextElementStart)
            {
                int httpStart = Common.IndexOf(ref markdown, "http://", startingPos, endingPos);
                int httpsStart = Common.IndexOf(ref markdown, "https://", startingPos, endingPos);

                if (httpsStart != -1 || httpStart != -1)
                {
                    // Make -1 huge.
                    httpStart = httpStart == -1 ? int.MaxValue : httpStart;
                    httpsStart = httpsStart == -1 ? int.MaxValue : httpsStart;

                    // Figure out the pos of the link
                    int foundLinkStart = Math.Min(httpStart, httpsStart);

                    // Set the start and end
                    currentNextElementStart = foundLinkStart;
                    elementEndingPos = Common.FindNextWhiteSpace(ref markdown, foundLinkStart, endingPos, true);
                    return true;
                }
            }
            return false;
        }
    }
}
