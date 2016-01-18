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
    public enum RedditLinkType
    {
        Subreddit,
        User,
    }

    public class RedditLinkInline : MarkdownInline, ILinkElement
    {
        /// <summary>
        /// The subreddit link text e.g. "r/news" or "/u/quinbd".
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The link URL.  This is the same as <see cref="Text"/> except that it always has the
        /// leading slash (i.e. the Url will be "/r/baconit" even if the text is "r/baconit").
        /// </summary>
        public string Url => Text.StartsWith("/") ? Text : "/" + Text;

        /// <summary>
        /// Subreddit links do not have a tooltip.
        /// </summary>
        string ILinkElement.Tooltip => null;

        /// <summary>
        /// The type of reddit link - currently can be a subreddit or a user.
        /// </summary>
        public RedditLinkType LinkType { get; set; }

        /// <summary>
        /// Initializes a new subreddit link.
        /// </summary>
        public RedditLinkInline() : base(MarkdownInlineType.RawSubreddit)
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
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = 'u', Type = MarkdownInlineType.RawSubreddit, IgnoreEscapeChar = true });
        }

        /// <summary>
        /// Attempts to parse a subreddit link e.g. "/r/news" or "r/news".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <param name="actualEnd"> Set to the end of the span when the return value is non-null. </param>
        /// <returns> A parsed subreddit link, or <c>null</c> if this is not a subreddit link. </returns>
        internal static RedditLinkInline Parse(string markdown, int start, int maxEnd, out int actualEnd)
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

            // Expect 'r' or 'u'.  Use this to determine the type of link.
            var linkType = RedditLinkType.Subreddit;
            if (pos == maxEnd || (markdown[pos] != 'r' && markdown[pos] != 'u'))
                return null;
            if (markdown[pos] == 'u')
                linkType = RedditLinkType.User;
            pos++;

            // Expect '/'
            if (pos == maxEnd || markdown[pos] != '/')
                return null;
            pos++;

            // Find the end of the link.
            actualEnd = Common.FindNextNonLetterDigitOrUnderscore(markdown, pos, maxEnd, true);

            // Subreddit names must be at least two characters long, users at least one.
            if (actualEnd - pos < (linkType == RedditLinkType.User ? 1 : 2))
                return null;

            // We found something!
            return new RedditLinkInline { Text = markdown.Substring(start, actualEnd - start), LinkType = linkType };
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
