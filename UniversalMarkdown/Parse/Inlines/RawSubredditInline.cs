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
    class RawSubredditInline : MarkdownInline
    {
        public string Text { get; set; }

        public RawSubredditInline()
            : base(MarkdownInlineType.RawSubreddit)
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
            // Find the start of the subreddit link
            int subredditStart = Common.IndexOf(ref markdown, "r/", startingPos, endingPos);
            if(subredditStart != startingPos && (subredditStart - 1) != startingPos)
            {
                DebuggingReporter.ReportCriticalError("Trying to parse a subreddit link but didn't find a subreddit");
                return endingPos;
            }

            // Grab where to begin looking for the end.
            int subredditEnd = subredditStart + 2;
            int subredditTextStart = subredditStart + 2;

            // Check if there is a / before it, if so include it
            if (subredditStart != 0 && markdown[subredditStart - 1] == '/')
            {
                subredditStart--;
            }

            // While we didn't hit the end && (it is a char or digit or _ )
            subredditEnd = Common.FindNextNonLetterDigitOrUnderscore(ref markdown, subredditEnd, endingPos, true);

            // Validate
            if(subredditEnd != endingPos)
            {
                DebuggingReporter.ReportCriticalError("Raw subreddit ending didn't match endingPos");
            }

            // Grab the text
            Text = markdown.Substring(subredditStart, subredditEnd - subredditStart);     

            // Return what we consumed
            return subredditEnd;
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
            // Test for raw subreddit links. We need to loop here so if we find a false positive
            // we can keep checking before the current closest. Note this logic must match the logic
            // in the subreddit link parser below.
            int subredditStart = Common.IndexOf(ref markdown, "r/", startingPos, endingPos);
            while (subredditStart != -1 && subredditStart < currentNextElementStart)
            {
                // Make sure the char before the r/ is not a letter
                if (subredditStart == 0 || !Char.IsLetterOrDigit(markdown[subredditStart - 1]))
                {
                    // Make sure there is something after the r/
                    if (subredditStart + 2 < markdown.Length && subredditStart + 2 < endingPos && Char.IsLetterOrDigit(markdown[subredditStart + 2]))
                    {
                        // Check if there is a / before it, if so include it
                        int beginEndSearchOffset = 2;
                        if (subredditStart != 0 && markdown[subredditStart - 1] == '/')
                        {
                            subredditStart--;
                            beginEndSearchOffset++;
                        }

                        // Send the info off!
                        currentNextElementStart = subredditStart;
                        elementEndingPos = Common.FindNextNonLetterDigitOrUnderscore(ref markdown, currentNextElementStart + beginEndSearchOffset, endingPos, true);
                        return true;
                    }
                }
                subredditStart += 2;
                subredditStart = Common.IndexOf(ref markdown, "r/", subredditStart, endingPos);
            }
            return false;
        }
    }
}
