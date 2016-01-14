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
using System.Text;

namespace UniversalMarkdown.Parse.Elements
{
    public class TextRunInline : MarkdownInline
    {
        /// <summary>
        /// The text for this run.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Initializes a new text span.
        /// </summary>
        public TextRunInline() : base(MarkdownInlineType.TextRun)
        {
        }

        /// <summary>
        /// Parses unformatted text.
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="end"> The location to stop parsing. </param>
        /// <returns> A parsed text span. </returns>
        internal static TextRunInline Parse(string markdown, int start, int end)
        {
            // Handle escape sequences.
            // Note: this code is designed to be as fast as possible in the case where there are no
            // escape sequences (expected to be the common case).
            StringBuilder result = null;
            int pos = start;
            while (pos < end)
            {
                // Look for the next backslash.
                int backslashIndex = markdown.IndexOf('\\', pos, end - pos);
                if (backslashIndex == -1 || backslashIndex >= end - 1)   // There's another character to come.
                    break;

                // Check if the character after the backslash can be escaped.
                char c = markdown[backslashIndex + 1];
                if (Array.IndexOf(new char[] { '\\', '`', '*', '_', '{', '}', '[', ']', '(', ')', '#', '+', '-', '.', '!', '|', '~', '^', '&', ':', '<', '>', '/' }, c) < 0)
                {
                    // This character cannot be escaped.
                    result.Append(markdown.Substring(pos, backslashIndex + 2 - pos));
                    pos = backslashIndex + 2;
                    continue;
                }

                // This here's an escape sequence!
                if (result == null)
                    result = new StringBuilder(end - start);
                result.Append(markdown.Substring(pos, backslashIndex - pos));
                result.Append(c);
                pos = backslashIndex + 2;
            }

            if (result != null)
            {
                result.Append(markdown.Substring(pos, end - pos));
                return new TextRunInline { Text = result.ToString() };
            }
            return new TextRunInline { Text = markdown.Substring(start, end - start) };
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
