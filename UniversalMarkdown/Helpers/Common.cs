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
using System.Linq;
using UniversalMarkdown.Parse;
using UniversalMarkdown.Parse.Elements;

namespace UniversalMarkdown.Helpers
{
    /// <summary>
    /// A helper class for the trip chars. This is an optimization. If we ask each class to go through the rage and look for it's self
    /// we end up looping through the range n times, once for each inline. This class represent the first char that an inline needs to find
    /// to has a possible match. We will go through the range once and look for everyone's trip chars, and if they can make a match from the trip
    /// char then we will commit to them.
    /// </summary>
    internal class InlineTripCharHelper
    {
        // Note! Everything in first char and suffix should be lower case!
        public char FirstChar;
        public MarkdownInlineType Type;
        public bool IgnoreEscapeChar;
    }


    internal class Common
    {
        private static List<InlineTripCharHelper> s_triggerList = new List<InlineTripCharHelper>();
        private static char[] s_tripCharacters;

        /// <summary>
        /// Returns a list of trip chars for all of the inlines. These are used to detect the
        /// possible beginning of an inline.
        /// </summary>
        /// <returns></returns>
        public static List<InlineTripCharHelper> GetInlineTriggersList()
        {
            lock (s_triggerList)
            {
                if (s_triggerList.Count == 0)
                {
                    BoldTextInline.AddTripChars(s_triggerList);
                    ItalicTextInline.AddTripChars(s_triggerList);
                    MarkdownLinkInline.AddTripChars(s_triggerList);
                    RawHyperlinkInline.AddTripChars(s_triggerList);
                    RawSubredditInline.AddTripChars(s_triggerList);
                    StrikethroughTextInline.AddTripChars(s_triggerList);
                    SuperscriptTextInline.AddTripChars(s_triggerList);
                    CodeInline.AddTripChars(s_triggerList);
                    // Text run doesn't have one.

                    // Create an array of characters to search against using IndexOfAny.
                    s_tripCharacters = s_triggerList.Select(trigger => trigger.FirstChar).Distinct().ToArray();
                }
            }
            return s_triggerList;
        }

        /// <summary>
        /// This function can be called by any element parsing. Given a start and stopping point this will
        /// parse all found elements out of the range.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="maxEndingPos"></param>
        /// <returns> A list of parsed inlines. </returns>
        public static List<MarkdownInline> ParseInlineChildren(string markdown, int startingPos, int maxEndingPos)
        {
            int currentParsePosition = startingPos;

            var inlines = new List<MarkdownInline>();
            while (currentParsePosition < maxEndingPos)
            {
                int nextElementStart;
                int nextElementEnd;

                // Find the next element
                MarkdownInline element = Common.FindNextInlineElement(markdown, currentParsePosition, maxEndingPos, out nextElementStart, out nextElementEnd);

                // If the element we found doesn't start at the position we are looking for there is text between the element and
                // the start. We need to wrap it into a Text Run
                if (nextElementStart != currentParsePosition)
                {
                    var textRun = TextRunInline.Parse(markdown, currentParsePosition, nextElementStart);
                    inlines.Add(textRun);
                }

                // Add the parsed element.
                inlines.Add(element);

                // Update the current position.
                currentParsePosition = nextElementEnd;
            }
            return inlines;
        }

        /// <summary>
        /// Finds the next inline element by matching trip chars and verifying the match.
        /// </summary>
        /// <returns></returns>
        public static MarkdownInline FindNextInlineElement(string markdown, int startingPos, int endingPos, out int nextElementStart, out int nextElementEnd)
        {
            // Get the list of triggers.
            List<InlineTripCharHelper> tripChars = GetInlineTriggersList();

            // Search for the next inline sequence.
            for (int pos = startingPos; pos < endingPos; pos++)
            {
                // IndexOfAny should be the fastest way to skip characters we don't care about.
                pos = markdown.IndexOfAny(s_tripCharacters, pos, endingPos - pos);
                if (pos < 0)
                    break;

                // Find the trigger(s) that matched.
                char currentChar = Char.ToLower(markdown[pos]);
                foreach (InlineTripCharHelper currentTripChar in tripChars)
                {
                    // Check if our current char matches the suffix char.
                    if (currentChar == currentTripChar.FirstChar)
                    {
                        // Don't match if the previous character was a backslash.
                        if (pos > startingPos && markdown[pos - 1] == '\\' && currentTripChar.IgnoreEscapeChar == false)
                            continue;

                        // If we are here we have a possible match. Call into the inline class to verify.
                        MarkdownInline parsedElement = null;
                        int parsedElementEnd = pos;
                        switch (currentTripChar.Type)
                        {
                            case MarkdownInlineType.Bold:
                                parsedElement = BoldTextInline.Parse(markdown, pos, endingPos, out parsedElementEnd);
                                break;
                            case MarkdownInlineType.Italic:
                                parsedElement = ItalicTextInline.Parse(markdown, pos, endingPos, out parsedElementEnd);
                                break;
                            case MarkdownInlineType.MarkdownLink:
                                parsedElement = MarkdownLinkInline.Parse(markdown, pos, endingPos, out parsedElementEnd);
                                break;
                            case MarkdownInlineType.RawHyperlink:
                                parsedElement = RawHyperlinkInline.Parse(markdown, pos, endingPos, out parsedElementEnd);
                                break;
                            case MarkdownInlineType.RawSubreddit:
                                parsedElement = RawSubredditInline.Parse(markdown, pos, endingPos, out parsedElementEnd);
                                break;
                            case MarkdownInlineType.Strikethrough:
                                parsedElement = StrikethroughTextInline.Parse(markdown, pos, endingPos, out parsedElementEnd);
                                break;
                            case MarkdownInlineType.Superscript:
                                parsedElement = SuperscriptTextInline.Parse(markdown, pos, endingPos, out parsedElementEnd);
                                break;
                            case MarkdownInlineType.Code:
                                parsedElement = CodeInline.Parse(markdown, pos, endingPos, out parsedElementEnd);
                                break;
                        }

                        if (parsedElement != null)
                        {
                            nextElementStart = pos;
                            nextElementEnd = parsedElementEnd;
                            return parsedElement;
                        }
                    }
                }
            }

            // If we didn't find any elements we have a normal text block.
            // Let us consume the entire range.
            nextElementStart = startingPos;
            nextElementEnd = endingPos;
            return TextRunInline.Parse(markdown, startingPos, endingPos);
        }


        /// <summary>
        /// Returns the next \n or \r\n in the markdown.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPos"></param>
        /// <param name="startOfNextLine"></param>
        /// <returns></returns>
        public static int FindNextSingleNewLine(string markdown, int startingPos, int endingPos, out int startOfNextLine)
        {
            // A line can end with CRLF (\r\n) or just LF (\n).
            int lineFeedPos = markdown.IndexOf('\n', startingPos);
            if (lineFeedPos == -1)
            {
                startOfNextLine = endingPos;
                return endingPos;
            }
            startOfNextLine = lineFeedPos + 1;

            // Check if it was a CRLF.
            if (lineFeedPos > startingPos && markdown[lineFeedPos - 1] == '\r')
                return lineFeedPos - 1;
            return lineFeedPos;
        }

        /// <summary>
        /// Helper function for index of with a start and an ending.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="search"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPos"></param>
        /// <returns></returns>
        public static int IndexOf(string markdown, string search, int startingPos, int endingPos, bool reverseSearch = false)
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
        public static int IndexOf(string markdown, char search, int startingPos, int endingPos, bool reverseSearch = false)
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
        /// Finds the next whitespace in a range.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPos"></param>
        /// <returns></returns>
        public static int FindNextWhiteSpace(string markdown, int startingPos, int endingPos, bool ifNotFoundReturnLength)
        {
            int currentPos = startingPos;
            while (currentPos < markdown.Length && currentPos < endingPos)
            {
                if (Char.IsWhiteSpace(markdown[currentPos]))
                {
                    return currentPos;
                }
                currentPos++;
            }
            return ifNotFoundReturnLength ? endingPos : -1;
        }

        /// <summary>
        /// Finds the next char that is not a letter or digit in a range.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPos"></param>
        /// <returns></returns>
        public static int FindNextNonLetterDigitOrUnderscore(string markdown, int startingPos, int endingPos, bool ifNotFoundReturnLenght)
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

        /// <summary>
        /// Determines if a character is a whitespace character.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n';
        }
    }
}
