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
        public Markdown()
            : base(MarkdownBlockType.Root)
        { }

        public void Parse(string markdownText)
        {
            // Don't do anything if we don't have anything.
            if(String.IsNullOrWhiteSpace(markdownText))
            {
                return;
            }

            // We need to make sure that all text ends with a \n\n so everything is contained in at least something
            markdownText += "\n\n";

            // Parse us.
            Parse(ref markdownText, 0, markdownText.Length);
        }

        /// <summary>
        /// Called when we should parse our blocks
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="maxEndingPos"></param>
        /// <returns></returns>
        internal override int Parse(ref string markdown, int startingPos, int maxEndingPos)
        {
            // We are the only thing that can parse blocks, and we should be the only thing to hold blocks.
            // So start off by parsing our block children.
            int currentParsePosition = 0;

            while (currentParsePosition < maxEndingPos)
            {
                int elementStartingPos = currentParsePosition;

                // Find the next element, note this can change the elementStartingPos param
                MarkdownBlock element = FindNextBlock(ref markdown, ref elementStartingPos, maxEndingPos);

                // If our next start is our end then we are done.
                if(maxEndingPos == elementStartingPos)
                {
                    return maxEndingPos;
                }

                // Ask it to parse, it will return us the ending pos of itself.
                currentParsePosition = element.Parse(ref markdown, elementStartingPos, maxEndingPos);

                // Add it the the children
                Children.Add(element);
            }

            return maxEndingPos;
        }

        /// <summary>
        /// Called by all elements to find the next element to parse out of the markdown given a startingPos and an ending Pos
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPost"></param>
        /// <returns></returns>
        public static MarkdownBlock FindNextBlock(ref string markdown, ref int startingPos, int endingPos)
        {
            // We need to look at the start of this current block and figure out what type it is.
            // Find the next char that isn't a \n, \r, or ' ', keep track of white space
            int spaceCount = 0;
            while (markdown.Length > startingPos && endingPos > startingPos && (markdown[startingPos] == '\r' || markdown[startingPos] == '\n' || Char.IsWhiteSpace(markdown[startingPos])))
            {
                // If we find a space count it for the indent rules. If not reset the count.
                spaceCount = markdown[startingPos] == ' ' ? spaceCount + 1 : 0;
                startingPos++;
            }

            if(CodeBlock.CanHandleBlock(ref markdown, startingPos, endingPos, spaceCount))
            {
                return new CodeBlock();
            }
            if(QuoteBlock.CanHandleBlock(ref markdown, startingPos, endingPos))
            {
                return new QuoteBlock();
            }
            if (HeaderBlock.CanHandleBlock(ref markdown, startingPos, endingPos))
            {
                return new HeaderBlock();
            }
            if (ListElementBlock.CanHandleBlock(ref markdown, startingPos, endingPos))
            {
                return new ListElementBlock();
            }
            if (HorizontalRuleBlock.CanHandleBlock(ref markdown, startingPos, endingPos))
            {
                return new HorizontalRuleBlock();
            }
            if (LineBreakBlock.CanHandleBlock(ref markdown, startingPos, endingPos))
            {
                return new LineBreakBlock();
            }

            // If we can't match any of these just make a new paragraph.
            return new ParagraphBlock();
        }
    }
}
