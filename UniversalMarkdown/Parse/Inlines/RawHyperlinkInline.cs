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
using UniversalMarkdown.Helpers;

namespace UniversalMarkdown.Parse.Elements
{
    public enum HyperlinkType
    {
        Url,
        Subreddit,
        User,
    }

    public class RawHyperlinkInline : MarkdownInline, IInlineLeaf, ILinkElement
    {
        /// <summary>
        /// The text to display.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The URL to link to.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Raw URLs do not have a tooltip.
        /// </summary>
        string ILinkElement.Tooltip => null;

        /// <summary>
        /// The type of hyperlink.
        /// </summary>
        public HyperlinkType LinkType { get; set; }

        /// <summary>
        /// Initializes a new markdown URL.
        /// </summary>
        public RawHyperlinkInline() : base(MarkdownInlineType.RawHyperlink)
        {
        }

        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        internal static void AddTripChars(List<Common.InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new Common.InlineTripCharHelper() { FirstChar = '<', Method = Common.InlineParseMethod.AngleBracketLink });
            tripCharHelpers.Add(new Common.InlineTripCharHelper() { FirstChar = ':', Method = Common.InlineParseMethod.Url });
            tripCharHelpers.Add(new Common.InlineTripCharHelper() { FirstChar = '/', Method = Common.InlineParseMethod.RedditLink });
        }

        /// <summary>
        /// Attempts to parse a URL within angle brackets e.g. "<http://www.reddit.com>".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <param name="actualEnd"> Set to the end of the span when the return value is non-null. </param>
        /// <returns> A parsed URL, or <c>null</c> if this is not a URL. </returns>
        internal static Common.InlineParseResult ParseAngleBracketLink(string markdown, int start, int maxEnd)
        {
            int innerStart = start + 1;
            int pos;
            string httpPrefix = "http://";
            string httpsPrefix = "https://";
            if (maxEnd - innerStart >= httpPrefix.Length && string.Equals(markdown.Substring(innerStart, httpPrefix.Length), httpPrefix, StringComparison.OrdinalIgnoreCase))
            {
                // HTTP hyperlink found.
                pos = innerStart + httpPrefix.Length;
            }
            else if (maxEnd - innerStart >= httpsPrefix.Length && string.Equals(markdown.Substring(innerStart, httpsPrefix.Length), httpsPrefix, StringComparison.OrdinalIgnoreCase))
            {
                // HTTPS hyperlink found.
                pos = innerStart + httpsPrefix.Length;
            }
            else
                return null;

            // Angle bracket links should not have any whitespace.
            int innerEnd = markdown.IndexOfAny(new char[] { ' ', '\t', '\r', '\n', '>' }, pos, maxEnd - pos);
            if (innerEnd == -1 || markdown[innerEnd] != '>')
                return null;

            // There should be at least one character after the http://.
            if (innerEnd == pos)
                return null;

            var url = markdown.Substring(innerStart, innerEnd - innerStart);
            return new Common.InlineParseResult(new RawHyperlinkInline { Url = url, Text = url, LinkType = HyperlinkType.Url }, start, innerEnd + 1);
        }

        /// <summary>
        /// Attempts to parse a URL e.g. "http://www.reddit.com".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <param name="actualEnd"> Set to the end of the span when the return value is non-null. </param>
        /// <returns> A parsed URL, or <c>null</c> if this is not a URL. </returns>
        internal static Common.InlineParseResult ParseUrl(string markdown, int tripPos, int maxEnd)
        {
            int start;
            string httpPrefix = "http://";
            string httpsPrefix = "https://";
            if (tripPos >= 4 && tripPos < maxEnd - 2 && string.Equals(markdown.Substring(tripPos - 4, httpPrefix.Length), httpPrefix, StringComparison.OrdinalIgnoreCase))
            {
                // HTTP hyperlink found.
                start = tripPos - 4;
            }
            else if (tripPos >= 5 && tripPos < maxEnd - 2 && string.Equals(markdown.Substring(tripPos - 5, httpsPrefix.Length), httpsPrefix, StringComparison.OrdinalIgnoreCase))
            {
                // HTTPS hyperlink found.
                start = tripPos - 5;
            }
            else
                return null;

            // The previous character must be non-alphanumeric i.e. "ahttp://t.co" is not a valid URL.
            if (start > 0 && char.IsLetter(markdown[start - 1]))
                return null;

            // The URL must have at least one character after the http:// and at least one dot.
            int pos = tripPos + 2;
            int dotIndex = markdown.IndexOf('.', pos, maxEnd - pos);
            if (dotIndex == -1 || dotIndex == pos)
                return null;

            // For some reason a less than character ends a URL...
            int end = markdown.IndexOfAny(new char[] { ' ', '\t', '\r', '\n', '<' }, dotIndex + 1, maxEnd - (dotIndex + 1));
            if (end == -1)
                end = maxEnd;

            // URLs can't end on a punctuation character.
            while (end - 1 > dotIndex)
            {
                if (Array.IndexOf(new char[] { ')', '}', ']', '!', ';', '.', '?', ',' }, markdown[end - 1]) < 0)
                    break;
                end--;
            }

            var url = markdown.Substring(start, end - start);
            return new Common.InlineParseResult(new RawHyperlinkInline { Url = url, Text = url, LinkType = HyperlinkType.Url }, start, end);
        }

        /// <summary>
        /// Attempts to parse a subreddit link e.g. "/r/news" or "r/news".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <param name="actualEnd"> Set to the end of the span when the return value is non-null. </param>
        /// <returns> A parsed subreddit link, or <c>null</c> if this is not a subreddit link. </returns>
        internal static Common.InlineParseResult ParseRedditLink(string markdown, int start, int maxEnd)
        {
            var result = ParseDoubleSlashLink(markdown, start, maxEnd);
            if (result != null)
                return result;
            return ParseSingleSlashLink(markdown, start, maxEnd);
        }

        /// <summary>
        /// Parse a link of the form "/r/news" or "/u/quinbd".
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="start"></param>
        /// <param name="maxEnd"></param>
        /// <returns></returns>
        private static Common.InlineParseResult ParseDoubleSlashLink(string markdown, int start, int maxEnd)
        {
            // The minimum length is 4 characters ("/u/u").
            if (start > maxEnd - 4)
                return null;

            // Determine the type of link (subreddit or user).
            HyperlinkType linkType;
            if (markdown[start + 1] == 'r')
                linkType = HyperlinkType.Subreddit;
            else if (markdown[start + 1] == 'u')
                linkType = HyperlinkType.User;
            else
                return null;

            // Check that there is another slash.
            if (markdown[start + 2] != '/')
                return null;

            // Find the end of the link.
            int end = Common.FindNextNonLetterDigitOrUnderscore(markdown, start + 3, maxEnd, true);

            // Subreddit names must be at least two characters long, users at least one.
            if (end - start < (linkType == HyperlinkType.User ? 4 : 5))
                return null;

            // We found something!
            var text = markdown.Substring(start, end - start);
            return new Common.InlineParseResult(new RawHyperlinkInline { Text = text, Url = text, LinkType = linkType }, start, end);
        }

        /// <summary>
        /// Parse a link of the form "r/news" or "u/quinbd".
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="start"></param>
        /// <param name="maxEnd"></param>
        /// <returns></returns>
        private static Common.InlineParseResult ParseSingleSlashLink(string markdown, int start, int maxEnd)
        {
            // The minimum length is 3 characters ("u/u").
            start--;
            if (start < 0 || start > maxEnd - 3)
                return null;

            // Determine the type of link (subreddit or user).
            HyperlinkType linkType;
            if (markdown[start] == 'r')
                linkType = HyperlinkType.Subreddit;
            else if (markdown[start] == 'u')
                linkType = HyperlinkType.User;
            else
                return null;

            // If the link doesn't start with '/', then the previous character must be
            // non-alphanumeric i.e. "bear/trap" is not a valid subreddit link.
            if (start >= 1 && (char.IsLetterOrDigit(markdown[start - 1]) || markdown[start - 1] == '/'))
                return null;

            // Find the end of the link.
            int end = Common.FindNextNonLetterDigitOrUnderscore(markdown, start + 2, maxEnd, true);

            // Subreddit names must be at least two characters long, users at least one.
            if (end - start < (linkType == HyperlinkType.User ? 3 : 4))
                return null;

            // We found something!
            var text = markdown.Substring(start, end - start);
            return new Common.InlineParseResult(new RawHyperlinkInline { Text = text, Url = "/" + text, LinkType = linkType }, start, end);
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
