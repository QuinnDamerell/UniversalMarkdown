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

namespace UniversalMarkdown.Parse.Elements
{
    class HorizontalRuleBlock : MarkdownBlock
    {
        public HorizontalRuleBlock()
            : base(MarkdownBlockType.HorizontalRule)
        { }

        /// <summary>
        /// Called when this block type should parse out the goods. Given the markdown, a starting point, and a max ending point
        /// the block should find the start of the block, find the end and parse out the middle. The end most of the time will not be
        /// the max ending pos, but it sometimes can be. The function will return where it ended parsing the block in the markdown.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="maxEndingPos"></param>
        /// <returns></returns>
        internal override int Parse(ref string markdown, int startingPos, int maxEndingPos)
        {
            // Figure out what char we are processing.
            int horzStart = startingPos;
            char ruleChar = '*';
            if (markdown[horzStart] == '*')
            {
                ruleChar = '*';
            }
            else if(markdown[horzStart] == '-')
            {
                ruleChar = '-';
            }
            else if(markdown[horzStart] == '=')
            {
                ruleChar = '=';
            }
            else if (markdown[horzStart] == '_')
            {
                ruleChar = '_';
            }
            else
            {
                DebuggingReporter.ReportCriticalError("Tried parse horizontal rule but didn't find a * or -");
                return maxEndingPos;
            }

            // Find the end of the line
            int horzEnd = horzStart;
            while (horzEnd < markdown.Length && horzEnd < maxEndingPos)
            {
                if (markdown[horzEnd] != ruleChar)
                {
                    break;
                }
                horzEnd++;
            }

            // Trim off any extra line endings, except ' ' otherwise we can't do code blocks
            while (horzEnd < markdown.Length && horzEnd < maxEndingPos && Char.IsWhiteSpace(markdown[horzEnd]) && markdown[horzEnd] != ' ')
            {
                horzEnd++;
            }

            // Return where we ended.
            return horzEnd;
        }

        /// <summary>
        /// Called to determine if this block type can handle the next block.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="nextCharPos"></param>
        /// <param name="endingPos"></param>
        /// <returns></returns>
        public static bool CanHandleBlock(ref string markdown, int nextCharPos, int endingPos)
        {
            return markdown.IndexOf("***", nextCharPos) == nextCharPos || markdown.IndexOf("---", nextCharPos) == nextCharPos || markdown.IndexOf("===", nextCharPos) == nextCharPos || markdown.IndexOf("___", nextCharPos) == nextCharPos;
        }
    }
}
