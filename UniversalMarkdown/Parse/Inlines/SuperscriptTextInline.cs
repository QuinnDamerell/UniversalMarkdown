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
    public class SuperscriptTextInline : MarkdownInline, IInlineContainer
    {
        /// <summary>
        /// The contents of the inline.
        /// </summary>
        public IList<MarkdownInline> Inlines { get; set; }

        /// <summary>
        /// Initializes a new superscript text span.
        /// </summary>
        public SuperscriptTextInline() : base(MarkdownInlineType.Superscript)
        {
        }

        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        internal static void AddTripChars(List<InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = '^', Type = MarkdownInlineType.Superscript });
        }

        /// <summary>
        /// Attempts to parse a superscript text span.
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <param name="actualEnd"> Set to the end of the span when the return value is non-null. </param>
        /// <returns> A parsed superscript text span, or <c>null</c> if this is not a superscript text span. </returns>
        internal static SuperscriptTextInline Parse(string markdown, int start, int maxEnd, out int actualEnd)
        {
            actualEnd = start;

            // Check the first character.
            if (start == maxEnd || markdown[start] != '^')
                return null;

            // The content might be enclosed in parentheses.
            int innerStart = start + 1;
            int innerEnd;
            if (innerStart < maxEnd && markdown[innerStart] == '(')
            {
                // Find the end parenthesis.
                innerStart++;
                innerEnd = Common.IndexOf(markdown, ')', innerStart, maxEnd);
                if (innerEnd == -1)
                    return null;
                actualEnd = innerEnd + 1;
            }
            else
            {
                // Search for the next whitespace character.
                innerEnd = Common.FindNextWhiteSpace(markdown, innerStart, maxEnd, ifNotFoundReturnLength: true);
                if (innerEnd == innerStart)
                    return null;   // No match if the character after the caret is a space.
                actualEnd = innerEnd;
            }

            // We found something!
            var result = new SuperscriptTextInline();
            result.Inlines = Common.ParseInlineChildren(markdown, innerStart, innerEnd);
            return result;
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            if (Inlines == null)
                return base.ToString();
            return "^(" + string.Join(string.Empty, Inlines) + ")";
        }
    }
}
