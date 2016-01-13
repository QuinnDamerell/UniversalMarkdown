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
using System.Text;
using UniversalMarkdown.Helpers;

namespace UniversalMarkdown.Parse.Elements
{
    public enum ListStyle
    {
        Bulleted,
        Numbered,
    }

    public class ListBlock : MarkdownBlock
    {
        /// <summary>
        /// The list items.
        /// </summary>
        public IList<ListItemBlock> Items { get; set; }

        /// <summary>
        /// The style of the list, either numbered or bulleted.
        /// </summary>
        public ListStyle Style;

        /// <summary>
        /// Initializes a new list block.
        /// </summary>
        public ListBlock() : base(MarkdownBlockType.List)
        {
        }

        /// <summary>
        /// Parses a list block.
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location of the first character in the block. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <param name="quoteDepth"> The current nesting level for block quoting. </param>
        /// <param name="actualEnd"> Set to the end of the block when the return value is non-null. </param>
        /// <returns> A parsed list block, or <c>null</c> if this is not a list block. </returns>
        internal static ListBlock Parse(string markdown, int start, int maxEnd, int quoteDepth, out int actualEnd)
        {
            // Attempt to parse the first list item.
            var firstItem = ParseItemPreamble(markdown, start, maxEnd);
            if (firstItem == null)
            {
                actualEnd = start;
                return null;
            }

            // This is definitely a valid list.
            var result = new ListBlock { Style = firstItem.Style };
            result.Items = new List<ListItemBlock>();
            var listItem = new ListItemBlock();
            int startOfLine = firstItem.ContentStartPos;
            int endOfLine;
            int itemContentStart = firstItem.ContentStartPos;

            do
            {
                // Find the end of the line.
                endOfLine = Common.FindNextSingleNewLine(markdown, startOfLine, maxEnd, out startOfLine);

                // Attempt to parse the next line as a new list item.
                var nextListItem = ParseItemPreamble(markdown, startOfLine, maxEnd);
                if (nextListItem != null)
                {
                    // Add the previous list item to the result.
                    int actualEnd1;
                    listItem.Blocks = Markdown.Parse(markdown, itemContentStart, endOfLine, quoteDepth, out actualEnd1);
                    result.Items.Add(listItem);

                    // Start a new list item.
                    listItem = new ListItemBlock();
                    itemContentStart = nextListItem.ContentStartPos;
                    startOfLine = nextListItem.ContentStartPos;
                }
                else
                {
                    // Okay, so the next line is not a list item.  Is the line blank?
                    bool containsNonSpaceChar = false;
                    int pos = startOfLine;
                    while (pos < maxEnd)
                    {
                        char c = markdown[pos];
                        if (c == '\n')
                            break;
                        if (!Common.IsWhiteSpace(c))
                        {
                            containsNonSpaceChar = true;
                            break;
                        }
                        pos++;
                    }
                    if (containsNonSpaceChar == false)
                    {
                        // The line is blank, which means this is the end of the list.
                        break;
                    }
                }
            } while (startOfLine < maxEnd);

            // Close off the unfinished list item.
            int actualEnd2;
            listItem.Blocks = Markdown.Parse(markdown, itemContentStart, endOfLine, quoteDepth, out actualEnd2);
            result.Items.Add(listItem);

            // Return the result.
            actualEnd = startOfLine;
            return result;
        }

        private class ListItemPreamble
        {
            public ListStyle Style;
            public int ContentStartPos;
        }

        /// <summary>
        /// Parsing helper method.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="start"></param>
        /// <param name="maxEnd"></param>
        /// <returns></returns>
        private static ListItemPreamble ParseItemPreamble(string markdown, int start, int maxEnd)
        {
            // Skip any whitespace characters.
            while (start < maxEnd)
            {
                char c = markdown[start];
                if (c != ' ' && c != '\t')
                    break;
                start++;
            }
            if (start == maxEnd)
                return null;

            // There are two types of lists.
            // A numbered list starts with a number, then a period ('.'), then a space.
            // A bulleted list starts with a star ('*'), dash ('-') or plus ('+'), then a period, then a space.
            ListStyle style;
            if (markdown[start] == '*' || markdown[start] == '-' || markdown[start] == '+')
            {
                style = ListStyle.Bulleted;
                start++;
            }
            else if (markdown[start] >= '0' && markdown[start] <= '9')
            {
                style = ListStyle.Numbered;
                start++;

                // Skip any other digits.
                while (start < maxEnd)
                {
                    char c = markdown[start];
                    if (c < '0' || c > '9')
                        break;
                    start++;
                }

                // Next should be a period ('.').
                if (start == maxEnd || markdown[start] != '.')
                    return null;
                start++;
            }
            else
                return null;

            // Next should be a space.
            if (start == maxEnd || (markdown[start] != ' ' && markdown[start] != '\t'))
                return null;
            start++;

            // This is a valid list item.
            return new ListItemPreamble { Style = style, ContentStartPos = start };
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            if (Items == null)
                return base.ToString();
            var result = new StringBuilder();
            for (int i = 0; i < Items.Count; i ++)
            {
                if (result.Length > 0)
                    result.AppendLine();
                switch (Style)
                {
                    case ListStyle.Bulleted:
                        result.Append("* ");
                        break;
                    case ListStyle.Numbered:
                        result.Append(i + 1);
                        result.Append(".");
                        break;
                }
                result.Append(" ");
                result.Append(string.Join("\r\n", Items[i].Blocks));
            }
            return result.ToString();
        }
    }

    public class ListItemBlock
    {
        /// <summary>
        /// The contents of the list item.
        /// </summary>
        public IList<MarkdownBlock> Blocks { get; set; }

        /// <summary>
        /// Initializes a new list item.
        /// </summary>
        public ListItemBlock()
        {
        }
    }
}
