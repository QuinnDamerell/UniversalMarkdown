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
    public class RawHyperlinkInline : MarkdownInline
    {
        /// <summary>
        /// The raw URL.
        /// </summary>
        public string Url { get; set; }

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
        internal static void AddTripChars(List<InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = '<', Type = MarkdownInlineType.RawHyperlink });
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = 'h', Type = MarkdownInlineType.RawHyperlink, IgnoreEscapeChar = true });
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = 'H', Type = MarkdownInlineType.RawHyperlink, IgnoreEscapeChar = true });
        }

        /// <summary>
        /// Attempts to parse a URL e.g. "http://www.reddit.com".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <param name="actualEnd"> Set to the end of the span when the return value is non-null. </param>
        /// <returns> A parsed URL, or <c>null</c> if this is not a URL. </returns>
        internal static RawHyperlinkInline Parse(string markdown, int start, int maxEnd, out int actualEnd)
        {
            actualEnd = start;
            if (start == maxEnd)
                return null;

            // Links can be inside angle brackets.
            bool insideAngleBrackets = false;
            if (markdown[start] == '<')
            {
                insideAngleBrackets = true;
                start++;
            }
            else
            {
                // The previous character must be non-alphanumeric i.e. "ahttp://t.co" is not a valid URL.
                if (start > 0 && char.IsLetter(markdown[start - 1]))
                    return null;
            }

            int pos = start;
            string httpPrefix = "http://";
            string httpsPrefix = "https://";
            if (maxEnd - start >= httpPrefix.Length && string.Equals(markdown.Substring(start, httpPrefix.Length), httpPrefix, StringComparison.OrdinalIgnoreCase))
            {
                // HTTP hyperlink found.
                pos += httpPrefix.Length;
            }
            else if (maxEnd - start >= httpsPrefix.Length && string.Equals(markdown.Substring(start, httpsPrefix.Length), httpsPrefix, StringComparison.OrdinalIgnoreCase))
            {
                // HTTPS hyperlink found.
                pos += httpsPrefix.Length;
            }
            else
                return null;

            // Look for the end of the link.
            if (insideAngleBrackets)
            {
                // Angle bracket links should not have any whitespace.
                int innerEnd = markdown.IndexOfAny(new char[] { ' ', '\t', '\r', '\n', '>' }, pos, maxEnd - pos);
                if (innerEnd == -1 || markdown[innerEnd] != '>')
                    return null;

                // There should be at least one character after the http://.
                if (innerEnd == pos)
                    return null;

                actualEnd = innerEnd + 1;
                return new RawHyperlinkInline { Url = markdown.Substring(start, innerEnd - start) };
        }
            else
            {
                // The URL must have at least one character after the http:// and at least one dot.
                int dotIndex = markdown.IndexOf('.', pos, maxEnd - pos);
                if (dotIndex == -1 || dotIndex == pos)
                    return null;

                // For some reason a less than character ends a URL...
                actualEnd = markdown.IndexOfAny(new char[] { ' ', '\t', '\r', '\n', '<' }, dotIndex + 1, maxEnd - (dotIndex + 1));
                if (actualEnd == -1)
                    actualEnd = maxEnd;

                // URLs can't end on a punctuation character.
                if (Array.IndexOf(new char[] { ')', '}', ']', '!', ';', '.', '?', ',' }, markdown[actualEnd - 1]) >= 0)
                    actualEnd--;

                return new RawHyperlinkInline { Url = markdown.Substring(start, actualEnd - start) };
                }
            }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            if (Url == null)
                return base.ToString();
            return Url;
        }
    }
}
