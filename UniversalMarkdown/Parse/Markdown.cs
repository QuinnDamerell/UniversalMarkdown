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
using UniversalMarkdown.Helpers;
using UniversalMarkdown.Parse.Elements;

namespace UniversalMarkdown.Parse
{
    /// <summary>
    /// A class used to represent abstract markdown.
    /// </summary>
    public class Markdown : MarkdownBlock
    {
        /// <summary>
        /// Holds the list of block elements.
        /// </summary>
        public IList<MarkdownBlock> Blocks { get; set; }

        /// <summary>
        /// Initializes a new markdown document.
        /// </summary>
        public Markdown() : base(MarkdownBlockType.Root)
        {
        }

        /// <summary>
        /// Parses markdown document text.
        /// </summary>
        public void Parse(string markdownText)
        {
            Blocks = Parse(markdownText, 0, markdownText.Length);
        }

        /// <summary>
        /// Parses a markdown document.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns> A list of parsed blocks. </returns>
        public static List<MarkdownBlock> Parse(string markdown, int start, int end)
        {
            // We need to parse out the list of blocks.
            // Some blocks need to start on a new paragraph (code, lists and tables) while other
            // blocks can start on any line (headers, horizontal rules and quotes).
            // Text that is outside of any other block becomes a paragraph.

            var blocks = new List<MarkdownBlock>();
            int startOfLine = start;
            bool lineStartsNewParagraph = true;
            int startOfParagraphText = -1;

            // These are needed to parse underline-style header blocks.
            int previousStartOfLine = start;
            int previousEndOfLine = start;

            // Go line by line.
            while (startOfLine < end)
            {
                // Find the first non-whitespace character.
                char nonSpaceChar = '\0';
                int nonSpacePos = startOfLine;
                while (nonSpacePos < end)
                {
                    char c = markdown[nonSpacePos];
                    if (c == '\r' || c == '\n')
                    {
                        // The line is either entirely whitespace, or is empty.
                        break;
                    }
                    if (c != ' ' && c != '\t')
                    {
                        // The line has content.
                        nonSpaceChar = c;
                        break;
                    }
                    nonSpacePos++;
                }

                // Find the end of the current line.
                int startOfNextLine;
                int endOfLine = Common.FindNextSingleNewLine(markdown, nonSpacePos, end, out startOfNextLine);


                if (nonSpaceChar == '\0')
                {
                    // The line is empty or nothing but whitespace.
                    lineStartsNewParagraph = true;

                    if (startOfParagraphText >= 0)
                    {
                        // End the current paragraph.
                        blocks.Add(ParagraphBlock.Parse(markdown, startOfParagraphText, previousEndOfLine));
                        startOfParagraphText = -1;
                    }
                }
                else
                {

                    // This is a header if the line starts with a hash character,
                    // or if the line starts with '-' or a '=' character and has no other characters.
                    // Or a quote if the line starts with a greater than character (optionally preceded by whitespace).
                    // Or a horizontal rule if the line contains nothing but 3 '*', '-' or '_' characters (with optional whitespace).
                    MarkdownBlock newBlockElement = null;
                    if (nonSpaceChar == '#' && nonSpacePos == startOfLine)
                    {
                        // Hash-prefixed header.
                        newBlockElement = HeaderBlock.ParseHashPrefixedHeader(markdown, startOfLine, endOfLine);
                    }
                    else if ((nonSpaceChar == '-' || nonSpaceChar == '=') && nonSpacePos == startOfLine && startOfParagraphText >= 0)
                    {
                        // Underline style header. These are weird because you don't know you've
                        // got one until you've gone past it.
                        // Note: we intentionally deviate from reddit here in that we only
                        // recognise this type of header if the previous line is part of a
                        // paragraph.  For example if you have this, the header at the bottom is
                        // ignored:
                        //   a|b
                        //   -|-
                        //   1|2
                        //   ===
                        newBlockElement = HeaderBlock.ParseUnderlineStyleHeader(markdown, previousStartOfLine, previousEndOfLine, startOfLine, endOfLine);

                        if (newBlockElement != null)
                        {
                            // We're going to have to remove the header text from the pending
                            // paragraph by prematurely ending the current paragraph.
                            // We already made sure that there is a paragraph in progress.
                            if (previousStartOfLine > startOfParagraphText)
                                blocks.Add(ParagraphBlock.Parse(markdown, startOfParagraphText, previousStartOfLine));
                            startOfParagraphText = -1;
                        }
                    }
                    else if (nonSpaceChar == '>')
                    {
                        newBlockElement = new QuoteBlock();
                    }

                    // These characters overlap with the underline-style header so check for this LAST.
                    if (newBlockElement == null && (nonSpaceChar == '*' || nonSpaceChar == '-' || nonSpaceChar == '_'))
                    {
                        newBlockElement = HorizontalRuleBlock.Parse(markdown, startOfLine, endOfLine);
                    }

                    if (newBlockElement == null && lineStartsNewParagraph)
                    {
                        // Some block elements must start on a new paragraph (tables, lists and code).
                        int endOfBlock = startOfNextLine;
                        if (nonSpaceChar == '*' || nonSpaceChar == '+' || nonSpaceChar == '-' || (nonSpaceChar >= '0' && nonSpaceChar <= '9'))
                            newBlockElement = ListBlock.Parse(markdown, startOfLine, end, out endOfBlock);
                        if (newBlockElement == null && nonSpacePos > startOfLine)
                            newBlockElement = CodeBlock.Parse(markdown, startOfLine, end, out endOfBlock);
                        if (newBlockElement == null)
                            newBlockElement = TableBlock.Parse(markdown, startOfLine, endOfLine, end, out endOfBlock);
                        if (newBlockElement != null)
                            startOfNextLine = endOfBlock;
                    }

                    // Block elements start new paragraphs.
                    lineStartsNewParagraph = newBlockElement != null;

                    if (newBlockElement == null)
                    {
                        // The line contains paragraph text.
                        if (startOfParagraphText < 0)
                            startOfParagraphText = startOfLine;

                        // Add the last paragraph if we are at the end of the input text.
                        if (startOfNextLine >= end)
                        {
                            // End the current paragraph.
                            blocks.Add(ParagraphBlock.Parse(markdown, startOfParagraphText, endOfLine));
                            startOfParagraphText = -1;
                        }
                    }
                    else
                    {
                        // The line contained a block.
                        if (startOfParagraphText >= 0)
                        {
                            // End the current paragraph.
                            blocks.Add(ParagraphBlock.Parse(markdown, startOfParagraphText, previousEndOfLine));
                            startOfParagraphText = -1;
                        }
                        blocks.Add(newBlockElement);
                    }
                }

                // Repeat.
                previousStartOfLine = startOfLine;
                previousEndOfLine = endOfLine;
                startOfLine = startOfNextLine;
            }
            return blocks;
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            if (Blocks == null)
                return base.ToString();
            return string.Join("\r\n", Blocks);
        }
    }
}
