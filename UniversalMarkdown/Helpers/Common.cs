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
using System.Text;
using System.Threading.Tasks;
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
    public class InlineTripCharHelper
    {
        // Note! Everything in first char and suffix should be lower case!
        public char FirstChar;
        public string FirstCharSuffix = null;
        public MarkdownInlineType Type;
    }


    class Common
    {
        private static List<InlineTripCharHelper> s_tripCharList = new List<InlineTripCharHelper>();

        /// <summary>
        /// Returns a list of trip chars for all of the inlines. These are used to detect the
        /// possible beginning of an inline.
        /// </summary>
        /// <returns></returns>
        public static List<InlineTripCharHelper> GetTripCharsList()
        {
            lock(s_tripCharList)
            {
                if (s_tripCharList.Count == 0)
                {
                    s_tripCharList.Add(BoldTextElement.GetTripChars());
                    s_tripCharList.Add(ItalicTextElement.GetTripChars());
                    s_tripCharList.Add(MarkdownLinkInline.GetTripChars());
                    s_tripCharList.Add(RawHyperlinkInline.GetTripChars());
                    s_tripCharList.Add(RawSubredditInline.GetTripChars());
                    // Text run doesn't have one.
                }
            }
            return s_tripCharList;
        }


        /// <summary>
        /// Finds the next inline element by matching trip chars and verifying the match.
        /// </summary>
        /// <returns></returns>
        public static MarkdownInline FindNextInlineElement(ref string markdown, int startingPos, int endingPos, ref int nextElementStart, ref int nextElementEnd)
        {
            // Get the list of trip chars
            List<InlineTripCharHelper> tripChars = GetTripCharsList();

            // Loop though all of the chars in this run and look for a trip char.
            for(int i = startingPos; i < endingPos; i++)
            {
                char currentChar = Char.ToLower(markdown[i]);

                // Try to match each trip char to the char
                foreach(InlineTripCharHelper currentTripChar in tripChars)
                {
                    // Check if our current char matches the sufex char.
                    if(currentChar == currentTripChar.FirstChar)
                    {

                        // We have a match! See if there is a suffix and if so if it matches.
                        if(currentTripChar.FirstCharSuffix != null)
                        {
                            // We need to loop through the sufex and see if it matches the next n chars in the markdown.
                            int suffexCharCounter = i + 1;
                            bool suffexFound = true;
                            foreach (char suffexChar in currentTripChar.FirstCharSuffix)
                            {
                                char test = Char.ToLower(markdown[suffexCharCounter]);
                                if (suffexCharCounter >= endingPos || suffexChar != Char.ToLower(markdown[suffexCharCounter]))
                                {
                                    suffexFound = false;
                                    break;
                                }
                                suffexCharCounter++;
                            }
                            // If the suffex didn't match this isn't a possibility.
                            if(!suffexFound)
                            {
                                continue;
                            }
                        }

                        // If we are here we have a possible match. Call into the inline class to verify.
                        // Note! The order of bold and italic here is important because they both start with *
                        // otherwise italic will consume bold's opening tag.
                        switch(currentTripChar.Type)
                        {
                            case MarkdownInlineType.Bold:
                                if (BoldTextElement.VerifyMatch(ref markdown, i, endingPos, ref nextElementStart, ref nextElementEnd))
                                {
                                    return new BoldTextElement();
                                }
                                break;
                            case MarkdownInlineType.Italic:
                                if (ItalicTextElement.VerifyMatch(ref markdown, i, endingPos, ref nextElementStart, ref nextElementEnd))
                                {
                                    return new ItalicTextElement();
                                }
                                break;
                            case MarkdownInlineType.MarkdownLink:
                                if (MarkdownLinkInline.VerifyMatch(ref markdown, i, endingPos, ref nextElementStart, ref nextElementEnd))
                                {
                                    return new MarkdownLinkInline();
                                }
                                break;
                            case MarkdownInlineType.RawHyperlink:
                                if (RawHyperlinkInline.VerifyMatch(ref markdown, i, endingPos, ref nextElementStart, ref nextElementEnd))
                                {
                                    return new RawHyperlinkInline();
                                }
                                break;
                            case MarkdownInlineType.RawSubreddit:
                                if (RawSubredditInline.VerifyMatch(ref markdown, i, endingPos, ref nextElementStart, ref nextElementEnd))
                                {
                                    return new RawSubredditInline();
                                }
                                break;
                        }
                    }
                }
            }

            // If we didn't find any elements we have a normal text block.
            // Let is consume the entire range.
            nextElementStart = startingPos;
            nextElementEnd = endingPos;
            return new TextRunInline();
        }

        /// <summary>
        /// Returns the next \n\n or \r\n\r\n in the markdown.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPos"></param>
        /// <returns></returns>
        public static int FindNextDoubleNewLine(ref string markdown, int startingPos, int endingPos)
        {
            // Find any line marker
            int doubleNewLinePos = IndexOf(ref markdown, "\n\n", startingPos, endingPos);
            int returnNewLinePos = IndexOf(ref markdown, "\r\n\r\n", startingPos, endingPos);

            if (doubleNewLinePos == -1 && returnNewLinePos == -1)
            {
                return -1;
            }

            // If either are -1 make them huge
            doubleNewLinePos = doubleNewLinePos == -1 ? int.MaxValue : doubleNewLinePos;
            returnNewLinePos = returnNewLinePos == -1 ? int.MaxValue : returnNewLinePos;
            return Math.Min(doubleNewLinePos, returnNewLinePos);
        }

        /// <summary>
        /// Returns the next \n or \r\n in the markdown.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPos"></param>
        /// <returns></returns>
        public static int FindNextSingleNewLine(ref string markdown, int startingPos, int endingPos)
        {
            // Find any line marker
            int newLinePos = IndexOf(ref markdown, "\n", startingPos, endingPos);
            int returnLinePos = IndexOf(ref markdown, "\r\n", startingPos, endingPos);

            if (newLinePos == -1 && returnLinePos == -1)
            {
                return -1;
            }

            // If either are -1 make them huge
            newLinePos = newLinePos == -1 ? int.MaxValue : newLinePos;
            returnLinePos = returnLinePos == -1 ? int.MaxValue : returnLinePos;
            return Math.Min(newLinePos, returnLinePos);
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
