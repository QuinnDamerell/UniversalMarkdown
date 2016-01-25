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
    /// <summary>
    /// Represents a type of hyperlink where the text can be different from the target URL.
    /// </summary>
    public class MarkdownLinkInline : MarkdownInline, IInlineContainer, ILinkElement
    {
        /// <summary>
        /// Initializes a new markdown link.
        /// </summary>
        public MarkdownLinkInline() : base(MarkdownInlineType.MarkdownLink)
        {
        }

        /// <summary>
        /// The contents of the inline.
        /// </summary>
        public IList<MarkdownInline> Inlines { get; set; }

        /// <summary>
        /// The link URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// A tooltip to display on hover.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        internal static void AddTripChars(List<Common.InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new Common.InlineTripCharHelper() { FirstChar = '[', Method = Common.InlineParseMethod.MarkdownLink });
        }

        /// <summary>
        /// Attempts to parse a markdown link e.g. "[](http://www.reddit.com)".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <returns> A parsed markdown link, or <c>null</c> if this is not a markdown link. </returns>
        internal static Common.InlineParseResult Parse(string markdown, int start, int maxEnd)
        {
            // Expect a '[' character.
            int linkTextOpen = start;
            if (linkTextOpen == maxEnd || markdown[linkTextOpen] != '[')
                return null;

            // Find the ']' character, keeping in mind that [test [0-9]](http://www.test.com) is allowed.
            int pos = linkTextOpen + 1;
            int linkTextClose;
            int openSquareBracketCount = 0;
            while (true)
            {
                linkTextClose = markdown.IndexOfAny(new char[] { '[', ']' }, pos, maxEnd - pos);
                if (linkTextClose == -1)
                    return null;
                if (markdown[linkTextClose] == '[')
                    openSquareBracketCount++;
                else if (openSquareBracketCount > 0)
                    openSquareBracketCount--;
                else
                    break;
                pos = linkTextClose + 1;
            }

            // Find the '(' character.
            int linkOpen = Common.IndexOf(markdown, '(', linkTextClose, maxEnd);
            if (linkOpen == -1)
                return null;

            // Skip whitespace.
            linkOpen++;
            while (linkOpen < maxEnd && Common.IsWhiteSpace(markdown[linkOpen]))
                linkOpen++;

            // Find the ')' character.
            pos = linkOpen;
            int linkClose = -1;
            while (pos < maxEnd)
            {
                linkClose = Common.IndexOf(markdown, ')', pos, maxEnd);
                if (linkClose == -1)
                    return null;
                if (markdown[linkClose - 1] != '\\')
                    break;
                pos = linkClose + 1;
            }
            if (pos >= maxEnd)
                return null;
            int end = linkClose + 1;

            // Skip whitespace backwards.
            while (linkClose > linkOpen && Common.IsWhiteSpace(markdown[linkClose - 1]))
                linkClose--;

            // If there is no text whatsoever, then this is not a valid link.
            if (linkOpen == linkClose)
                return null;

            // Check if there is tooltip text.
            
            string url;
            string tooltip = null;
            bool lastUrlCharIsDoubleQuote = markdown[linkClose - 1] == '"';
            int tooltipStart = Common.IndexOf(markdown, " \"", linkOpen + 1, linkClose - 1, reverseSearch: true);
            if (lastUrlCharIsDoubleQuote && tooltipStart != -1)
            {
                // Extract the URL (resolving any escape sequences).
                url = TextRunInline.ResolveEscapeSequences(markdown, linkOpen, tooltipStart).TrimEnd(' ', '\t', '\r', '\n');
                tooltip = markdown.Substring(tooltipStart + 2, (linkClose - 1) - (tooltipStart + 2));
            }
            else
            {
                // Extract the URL (resolving any escape sequences).
                url = TextRunInline.ResolveEscapeSequences(markdown, linkOpen, linkClose);
            }

            // Relative links are allowed.
            if (!url.StartsWith("/"))
            {
                // Check the scheme is allowed.
                bool schemeIsAllowed = false;
                foreach (var scheme in HyperlinkInline.KnownSchemes)
                {
                    if (url.StartsWith(scheme))
                    {
                        schemeIsAllowed = true;
                        break;
                    }
                }
                if (schemeIsAllowed == false)
                    return null;
            }

            // We found something!
            var result = new MarkdownLinkInline();
            result.Inlines = Common.ParseInlineChildren(markdown, linkTextOpen + 1, linkTextClose, ignoreLinks: true);
            result.Url = url.Replace(" ", "%20");
            result.Tooltip = tooltip;
            return new Common.InlineParseResult(result, start, end);
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            if (Inlines == null || Url == null)
                return base.ToString();
            return string.Format("[{0}]({1})", string.Join(string.Empty, Inlines), Url);
        }
    }
}
