﻿// Copyright (c) 2016 Quinn Damerell
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
        /// <summary>
        /// A hyperlink surrounded by angle brackets (e.g. "<http://www.reddit.com>").
        /// </summary>
        BracketedUrl,

        /// <summary>
        /// A fully qualified hyperlink (e.g. "http://www.reddit.com").
        /// </summary>
        FullUrl,

        /// <summary>
        /// A URL without a scheme (e.g. "www.reddit.com").
        /// </summary>
        PartialUrl,

        /// <summary>
        /// An email address (e.g. "test@reddit.com").
        /// </summary>
        Email,

        /// <summary>
        /// A subreddit link (e.g. "/r/news").
        /// </summary>
        Subreddit,

        /// <summary>
        /// A user link (e.g. "/u/quinbd").
        /// </summary>
        User,
    }

    /// <summary>
    /// Represents a type of hyperlink where the text and the target URL cannot be controlled
    /// independently.
    /// </summary>
    public class HyperlinkInline : MarkdownInline, IInlineLeaf, ILinkElement
    {
        /// <summary>
        /// Initializes a new markdown URL.
        /// </summary>
        public HyperlinkInline() : base(MarkdownInlineType.RawHyperlink)
        {
        }

        /// <summary>
        /// The text to display.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The URL to link to.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// This type of hyperlink does not have a tooltip.
        /// </summary>
        string ILinkElement.Tooltip => null;

        /// <summary>
        /// The type of hyperlink.
        /// </summary>
        public HyperlinkType LinkType { get; set; }


        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        internal static void AddTripChars(List<Common.InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new Common.InlineTripCharHelper() { FirstChar = '<', Method = Common.InlineParseMethod.AngleBracketLink });
            tripCharHelpers.Add(new Common.InlineTripCharHelper() { FirstChar = ':', Method = Common.InlineParseMethod.Url });
            tripCharHelpers.Add(new Common.InlineTripCharHelper() { FirstChar = '/', Method = Common.InlineParseMethod.RedditLink });
            tripCharHelpers.Add(new Common.InlineTripCharHelper() { FirstChar = '.', Method = Common.InlineParseMethod.PartialLink });
        }

        /// <summary>
        /// A list of URL schemes.
        /// </summary>
        internal readonly static string[] KnownSchemes = new string[]
        {
            "http://",
            "https://",
            "ftp://",
            "steam://",
            "irc://",
            "news://",
            "mumble://",
            "ssh://",
        };

        /// <summary>
        /// Attempts to parse a URL within angle brackets e.g. "<http://www.reddit.com>".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <returns> A parsed URL, or <c>null</c> if this is not a URL. </returns>
        internal static Common.InlineParseResult ParseAngleBracketLink(string markdown, int start, int maxEnd)
        {
            int innerStart = start + 1;

            // Check for a known scheme e.g. "https://".
            int pos = -1;
            foreach (var scheme in KnownSchemes)
            {
                if (maxEnd - innerStart >= scheme.Length && string.Equals(markdown.Substring(innerStart, scheme.Length), scheme, StringComparison.OrdinalIgnoreCase))
                {
                    // URL scheme found.
                    pos = innerStart + scheme.Length;
                    break;
                }
            }
            if (pos == -1)
                return null;

            // Angle bracket links should not have any whitespace.
            int innerEnd = markdown.IndexOfAny(new char[] { ' ', '\t', '\r', '\n', '>' }, pos, maxEnd - pos);
            if (innerEnd == -1 || markdown[innerEnd] != '>')
                return null;

            // There should be at least one character after the http://.
            if (innerEnd == pos)
                return null;

            var url = markdown.Substring(innerStart, innerEnd - innerStart);
            return new Common.InlineParseResult(new HyperlinkInline { Url = url, Text = url, LinkType = HyperlinkType.BracketedUrl }, start, innerEnd + 1);
        }

        /// <summary>
        /// Attempts to parse a URL e.g. "http://www.reddit.com".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="tripPos"> The location of the colon character. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <returns> A parsed URL, or <c>null</c> if this is not a URL. </returns>
        internal static Common.InlineParseResult ParseUrl(string markdown, int tripPos, int maxEnd)
        {
            int start = -1;

            // Check for a known scheme e.g. "https://".
            foreach (var scheme in KnownSchemes)
            {
                int schemeStart = tripPos - (scheme.Length - 3);
                if (schemeStart >= 0 && schemeStart <= maxEnd - scheme.Length && string.Equals(markdown.Substring(schemeStart, scheme.Length), scheme, StringComparison.OrdinalIgnoreCase))
                {
                    // URL scheme found.
                    start = schemeStart;
                    break;
                }
            }
            if (start == -1)
                return null;

            // The previous character must be non-alphanumeric i.e. "ahttp://t.co" is not a valid URL.
            if (start > 0 && char.IsLetter(markdown[start - 1]))
                return null;

            // The URL must have at least one character after the http:// and at least one dot.
            int pos = tripPos + 2;
            int dotIndex = markdown.IndexOf('.', pos, maxEnd - pos);
            if (dotIndex == -1 || dotIndex == pos)
                return null;

            // Find the end of the URL.
            int end = FindUrlEnd(markdown, dotIndex + 1, maxEnd);

            var url = markdown.Substring(start, end - start);
            return new Common.InlineParseResult(new HyperlinkInline { Url = url, Text = url, LinkType = HyperlinkType.FullUrl }, start, end);
        }

        /// <summary>
        /// Attempts to parse a subreddit link e.g. "/r/news" or "r/news".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <returns> A parsed subreddit or user link, or <c>null</c> if this is not a subreddit link. </returns>
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
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <returns> A parsed subreddit or user link, or <c>null</c> if this is not a subreddit link. </returns>
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
            int end = FindNextNonLetterDigitOrUnderscore(markdown, start + 3, maxEnd);

            // Subreddit names must be at least two characters long, users at least one.
            if (end - start < (linkType == HyperlinkType.User ? 4 : 5))
                return null;

            // We found something!
            var text = markdown.Substring(start, end - start);
            return new Common.InlineParseResult(new HyperlinkInline { Text = text, Url = text, LinkType = linkType }, start, end);
        }

        /// <summary>
        /// Parse a link of the form "r/news" or "u/quinbd".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <returns> A parsed subreddit or user link, or <c>null</c> if this is not a subreddit link. </returns>
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
            int end = FindNextNonLetterDigitOrUnderscore(markdown, start + 2, maxEnd);

            // Subreddit names must be at least two characters long, users at least one.
            if (end - start < (linkType == HyperlinkType.User ? 3 : 4))
                return null;

            // We found something!
            var text = markdown.Substring(start, end - start);
            return new Common.InlineParseResult(new HyperlinkInline { Text = text, Url = "/" + text, LinkType = linkType }, start, end);
        }

        /// <summary>
        /// Attempts to parse a URL without a scheme e.g. "www.reddit.com".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="tripPos"> The location of the dot character. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <returns> A parsed URL, or <c>null</c> if this is not a URL. </returns>
        internal static Common.InlineParseResult ParsePartialLink(string markdown, int tripPos, int maxEnd)
        {
            int start = tripPos - 3;
            if (start < 0 || markdown.Substring(start, 3) != "www")
                return null;

            // The character before the "www" must be non-alphanumeric i.e. "bwww.reddit.com" is not a valid URL.
            if (start >= 1 && (char.IsLetterOrDigit(markdown[start - 1]) || markdown[start - 1] == '<'))
                return null;

            // The URL must have at least one character after the www.
            if (start >= maxEnd - 4)
                return null;

            // Find the end of the URL.
            int end = FindUrlEnd(markdown, start + 4, maxEnd);

            var url = markdown.Substring(start, end - start);
            return new Common.InlineParseResult(new HyperlinkInline { Url = "http://" + url, Text = url, LinkType = HyperlinkType.PartialUrl }, start, end);
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



        /// <summary>
        /// Finds the next character that is not a letter, digit or underscore in a range.
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start searching. </param>
        /// <param name="end"> The location to stop searching. </param>
        /// <returns> The location of the next character that is not a letter, digit or underscore. </returns>
        private static int FindNextNonLetterDigitOrUnderscore(string markdown, int start, int end)
        {
            int pos = start;
            while (pos < markdown.Length && pos < end)
            {
                if (!char.IsLetterOrDigit(markdown[pos]) && markdown[pos] != '_')
                {
                    return pos;
                }
                pos++;
            }
            return end;
        }

        /// <summary>
        /// Finds the end of a URL.
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start searching. </param>
        /// <param name="end"> The location to stop searching. </param>
        /// <returns> The location of the end of the URL. </returns>
        private static int FindUrlEnd(string markdown, int start, int maxEnd)
        {
            // For some reason a less than character ends a URL...
            int end = markdown.IndexOfAny(new char[] { ' ', '\t', '\r', '\n', '<' }, start, maxEnd - start);
            if (end == -1)
                end = maxEnd;

            // URLs can't end on a punctuation character.
            while (end - 1 > start)
            {
                if (Array.IndexOf(new char[] { ')', '}', ']', '!', ';', '.', '?', ',' }, markdown[end - 1]) < 0)
                    break;
                end--;
            }
            return end;
        }
    }
}
