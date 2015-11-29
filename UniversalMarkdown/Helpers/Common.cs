// Copyright (c) 2015 Quinn Damerell
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Parse;
using UniversalMarkdown.Parse.Elements;

namespace UniversalMarkdown.Helpers
{
    class Common
    {
        /// <summary>
        /// Called by all elements to find the next element to parse out of the markdown given a startingPos and an ending Pos
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPost"></param>
        /// <returns></returns>
        public static MarkdownInline FindNextInlineElement(ref string markdown, int startingPos, int endingPos, ref int nextElementStart, ref int nextElementEnd)
        {
            // Hold what the next closest element is and where it is.
            nextElementStart = int.MaxValue;
            MarkdownInlineType nextClosestType = MarkdownInlineType.TextRun;

            // Go through all of the elements and try to find the next closest element. 
            // Note the order is important here, so don't reorder them.
            if (BoldTextElement.FindNextClosest(ref markdown, startingPos, endingPos, ref nextElementStart, ref nextElementEnd))
            {
                nextClosestType = MarkdownInlineType.Bold;
            }
            if (ItalicTextElement.FindNextClosest(ref markdown, startingPos, endingPos, ref nextElementStart, ref nextElementEnd))
            {
                nextClosestType = MarkdownInlineType.Italic;
            }
            if (MarkdownLinkInline.FindNextClosest(ref markdown, startingPos, endingPos, ref nextElementStart, ref nextElementEnd))
            {
                nextClosestType = MarkdownInlineType.MarkdownLink;
            }
            if (RawHyperlinkInline.FindNextClosest(ref markdown, startingPos, endingPos, ref nextElementStart, ref nextElementEnd))
            {
                nextClosestType = MarkdownInlineType.RawHyperlink;
            }
            if (RawSubredditInline.FindNextClosest(ref markdown, startingPos, endingPos, ref nextElementStart, ref nextElementEnd))
            {
                nextClosestType = MarkdownInlineType.RawSubreddit;
            }

            // If we didn't find any elements we have a normal text block.
            // Let is consume the entire range.
            if (nextElementStart == int.MaxValue)
            {
                nextElementStart = startingPos;
                nextElementEnd = endingPos;
                nextClosestType = MarkdownInlineType.TextRun;
            }

            // Now that we have our winner, make the object
            switch (nextClosestType)
            {
                case MarkdownInlineType.Bold:
                    return new BoldTextElement();
                case MarkdownInlineType.Italic:
                    return new ItalicTextElement();
                case MarkdownInlineType.MarkdownLink:
                    return new MarkdownLinkInline();
                case MarkdownInlineType.RawHyperlink:
                    return new RawHyperlinkInline();
                case MarkdownInlineType.RawSubreddit:
                    return new RawSubredditInline();
                case MarkdownInlineType.TextRun:
                default:
                    return new TextRunElement();
            }
        }

        public static int FindNextNewLine(ref string markdown, int startingPos, int endingPos)
        {
            // Find any line marker
            int newLinePos = markdown.IndexOf('\n', startingPos);
            int returnPos = markdown.IndexOf('\r', startingPos);

            if (newLinePos == -1 && returnPos == -1)
            {
                return -1;
            }

            // If either are -1 make them huge
            newLinePos = newLinePos == -1 ? int.MaxValue : newLinePos;
            returnPos = returnPos == -1 ? int.MaxValue : returnPos;
            int closestChar = Math.Min(newLinePos, returnPos);

            if (closestChar >= endingPos)
            {
                return -1;
            }

            return closestChar;
        }

        /// <summary>
        /// Helper function for index of with a start and an ending.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="search"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPos"></param>
        /// <returns></returns>
        public static int IndexOf(ref string markdown, string search, int startingPos, int endingPos, bool reverseSearch = false)
        {
            // Check the ending isn't out of bounds.
            if (endingPos > markdown.Length)
            {
                endingPos = markdown.Length;
                DebuggingReporter.ReportCriticalError("IndexOf endingPos > string length");
            }

            // Figure out how long to go
            int count = endingPos - startingPos;
            if (count < 0)
            {
                return -1;
            }

            // Make sure we don't go too far.
            int remainingCount = markdown.Length - startingPos;
            if (count > remainingCount)
            {
                DebuggingReporter.ReportCriticalError("IndexOf count > remaing count");
                count = remainingCount;
            }

            // Check the ending. Since we use inclusive ranges we need to -1 from this for
            // reverses searches.
            if (reverseSearch && endingPos > 0)
            {
                endingPos -= 1;
            }

            return reverseSearch ? markdown.LastIndexOf(search, endingPos, count) : markdown.IndexOf(search, startingPos, count);
        }

        /// <summary>
        /// Helper function for index of with a start and an ending.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="search"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPos"></param>
        /// <returns></returns>
        public static int IndexOf(ref string markdown, char search, int startingPos, int endingPos, bool reverseSearch = false)
        {
            // Check the ending isn't out of bounds.
            if (endingPos > markdown.Length)
            {
                endingPos = markdown.Length;
                DebuggingReporter.ReportCriticalError("IndexOf endingPos > string length");
            }

            // Figure out how long to go
            int count = endingPos - startingPos;
            if (count < 0)
            {
                return -1;
            }

            // Make sure we don't go too far.
            int remainingCount = markdown.Length - startingPos;
            if (count > remainingCount)
            {
                DebuggingReporter.ReportCriticalError("IndexOf count > remaing count");
                count = remainingCount;
            }

            // Check the ending. Since we use inclusive ranges we need to -1 from this for
            // reverses searches.
            if(reverseSearch && endingPos > 0)
            {
                endingPos -= 1;
            }

            return reverseSearch ? markdown.LastIndexOf(search, endingPos, count) : markdown.IndexOf(search, startingPos, count);
        }

        /// <summary>
        /// Finds the next whitespace in a range.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPos"></param>
        /// <returns></returns>
        public static int FindNextWhiteSpace(ref string markdown, int startingPos, int endingPos, bool ifNotFoundReturnLenght)
        {
            int currentPos = startingPos;
            while(currentPos < markdown.Length && currentPos < endingPos)
            {
                if(Char.IsWhiteSpace(markdown[currentPos]))
                {
                    return currentPos;
                }
                currentPos++;
            }
            return ifNotFoundReturnLenght ? endingPos : -1;
        }

        /// <summary>
        /// Finds the next char that is not a letter or digit in a range.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPos"></param>
        /// <returns></returns>
        public static int FindNextNonLetterDigitOrUnderscore(ref string markdown, int startingPos, int endingPos, bool ifNotFoundReturnLenght)
        {
            int currentPos = startingPos;
            while (currentPos < markdown.Length && currentPos < endingPos)
            {
                if (!Char.IsLetterOrDigit(markdown[currentPos]) && markdown[currentPos] != '_')
                {
                    return currentPos;
                }
                currentPos++;
            }
            return ifNotFoundReturnLenght ? endingPos : -1;
        }
    }
}
