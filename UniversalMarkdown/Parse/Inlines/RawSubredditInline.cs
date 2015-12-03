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
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        public static InlineTripCharHelper GetTripChars()
        {
            return new InlineTripCharHelper() { FirstChar = 'r', FirstCharSuffix = "/", Type = MarkdownInlineType.RawSubreddit };
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
            // Do a sanity check.
            if((Char.ToLower(markdown[startingPos]) != 'r' || markdown[startingPos + 1] != '/') &&
               (markdown[startingPos] != '/' || Char.ToLower(markdown[startingPos + 1]) != 'r' || markdown[startingPos + 2] != '/'))
            {
                DebuggingReporter.ReportCriticalError("Trying to parse a subreddit link but didn't find a subreddit");
                return endingPos;
            }
            int subredditStart = startingPos;

            // Grab where to begin looking for the end.
            int subredditEnd = subredditStart + 2;
            int subredditTextStart = subredditStart + 2;

            // If we start with a / we need to +1 to the end.
            if (markdown[subredditStart] == '/')
            {
                subredditEnd++;
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
            // Sanity Check
            if(Char.ToLower(markdown[startingPos]) == 'r')
            {
                int subredditStart = startingPos;
                // Make sure the char before the r/ is not a letter
                if (subredditStart == 0 || !Char.IsLetterOrDigit(markdown[subredditStart - 1]))
                {
                    // Make sure there is something after the r/
                    if (subredditStart + 2 < markdown.Length && subredditStart + 2 < maxEndingPos && Char.IsLetterOrDigit(markdown[subredditStart + 2]))
                    {
                        // Check if there is a / before it, if so include it
                        int beginEndSearchOffset = 2;
                        if (subredditStart != 0 && markdown[subredditStart - 1] == '/')
                        {
                            subredditStart--;
                            beginEndSearchOffset++;
                        }

                        // Send the info off!
                        elementStartingPos = subredditStart;
                        elementEndingPos = Common.FindNextNonLetterDigitOrUnderscore(ref markdown, subredditStart + beginEndSearchOffset, maxEndingPos, true);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
