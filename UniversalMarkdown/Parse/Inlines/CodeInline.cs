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
    public class CodeInline : MarkdownInline
    {
        /// <summary>
        /// The text to display as code.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Initializes a new inline code span.
        /// </summary>
        public CodeInline() : base(MarkdownInlineType.Code)
        {
        }

        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        internal static void AddTripChars(List<InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = '`', Type = MarkdownInlineType.Code });
        }

        /// <summary>
        /// Attempts to parse an inline code span.
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <param name="actualEnd"> Set to the end of the span when the return value is non-null. </param>
        /// <returns> A parsed inline code span, or <c>null</c> if this is not an inline code span. </returns>
        internal static CodeInline Parse(string markdown, int start, int maxEnd, out int actualEnd)
        {
            actualEnd = start;

            // Check the first char.
            if (start == maxEnd || markdown[start] != '`')
                return null;

            // Find the end of the span.
            var innerStart = start + 1;
            int innerEnd = Common.IndexOf(markdown, '`', innerStart, maxEnd);
            if (innerEnd == -1)
                return null;

            // The span must contain at least one character.
            if (innerStart == innerEnd)
                return null;

            // We found something!
            actualEnd = innerEnd + 1;
            var result = new CodeInline();
            result.Text = markdown.Substring(innerStart, innerEnd - innerStart);
            return result;
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            if (Text == null)
                return base.ToString();
            return "`" + Text + "`";
        }
    }
}
