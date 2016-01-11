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


using System.Collections.Generic;
using UniversalMarkdown.Helpers;

namespace UniversalMarkdown.Parse.Elements
{
    public class RawSubredditInline : MarkdownInline
    {
        /// <summary>
        /// The subreddit link text e.g. "r/news" or "/r/worldnews".
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Initializes a new subreddit link.
        /// </summary>
        public RawSubredditInline() : base(MarkdownInlineType.RawSubreddit)
        {
        }

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
        /// Attempts to parse a subreddit link e.g. "/r/news" or "r/news".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <param name="actualEnd"> Set to the end of the span when the return value is non-null. </param>
        /// <returns> A parsed subreddit link, or <c>null</c> if this is not a subreddit link. </returns>
        internal static RawSubredditInline Parse(string markdown, int start, int maxEnd, out int actualEnd)
        {
            actualEnd = start;
            if (start == maxEnd)
                return null;

            // The link may or may not start with '/'.
            int pos = start;
            if (markdown[pos] == '/')
            {
                pos++;
                if (pos == maxEnd)
                    return null;
            }
            else
            {
                // If the link doesn't start with '/', then the previous character must be
                // non-alphanumeric i.e. "bear/trap" is not a valid subreddit link.
                if (pos > 0 && char.IsLetterOrDigit(markdown[pos - 1]))
                    return null;
            }

            // Expect 'r'.
            if (pos == maxEnd || markdown[pos] != 'r')
                return null;
            pos++;

            // Expect '/'
            if (pos == maxEnd || markdown[pos] != '/')
                return null;
            pos++;

            // Find the end of the link.
            actualEnd = Common.FindNextNonLetterDigitOrUnderscore(markdown, pos, maxEnd, true);

            // The subreddit name must be at least 2 characters long.
            if (actualEnd - pos < 2)
                return null;

            // We found something!
            return new RawSubredditInline { Text = markdown.Substring(start, actualEnd - start) };
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
