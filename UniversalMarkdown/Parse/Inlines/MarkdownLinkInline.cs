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
    public class MarkdownLinkInline : MarkdownInline, ILinkElement
    {
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
        /// Initializes a new markdown link.
        /// </summary>
        public MarkdownLinkInline() : base(MarkdownInlineType.MarkdownLink)
        {
        }

        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        internal static void AddTripChars(List<InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = '[', Type = MarkdownInlineType.MarkdownLink });
        }

        /// <summary>
        /// Attempts to parse a markdown link e.g. "[](http://www.reddit.com)".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <param name="actualEnd"> Set to the end of the span when the return value is non-null. </param>
        /// <returns> A parsed markdown link, or <c>null</c> if this is not a markdown link. </returns>
        internal static MarkdownLinkInline Parse(string markdown, int start, int maxEnd, out int actualEnd)
        {
            actualEnd = start;

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

            // Find the '(' character.
            int linkClose = Common.IndexOf(markdown, ')', linkOpen, maxEnd);
            if (linkClose == -1)
                return null;

            // We found something!
            actualEnd = linkClose + 1;
            var result = new MarkdownLinkInline();
            result.Inlines = Common.ParseInlineChildren(markdown, linkTextOpen + 1, linkTextClose, ignoreLinks: true);
            result.Url = markdown.Substring(linkOpen + 1, linkClose - linkOpen - 1).Trim();
            return result;
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
