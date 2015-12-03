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
    public abstract class MarkdownElement
    {
        /// <summary>
        /// Holds the list of children for this element
        /// </summary>
        public List<MarkdownElement> Children = new List<MarkdownElement>();

        /// <summary>
        /// A function that all elements must implement to parse what they own.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="maxEndingPos"></param>
        /// <returns>Returns the ending position of the parse</returns>
        abstract internal int Parse(ref string markdown, int startingPos, int maxEndingPos);

        /// <summary>
        /// This function can be called by any element parsing. Given a start and stopping point this will
        /// parse all found elements out of the range.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="maxEndingPos"></param>
        protected void ParseInlineChildren(ref string markdown, int startingPos, int maxEndingPos)
        {
            int currentParsePosition = startingPos;


            while (currentParsePosition < maxEndingPos)
            {
                int nextElemntStart = 0;
                int nextElementEnd = 0;

                // Find the next element
                MarkdownInline element = Common.FindNextInlineElement(ref markdown, currentParsePosition, maxEndingPos, ref nextElemntStart, ref nextElementEnd);

                // If the element we found doesn't start at the position we are looking for there is text between the element and
                // the start. We need to wrap it into a Text Run
                if (nextElemntStart != currentParsePosition)
                {
                    TextRunInline textRun = new TextRunInline();
                    textRun.Parse(ref markdown, currentParsePosition, nextElemntStart);
                    Children.Add(textRun);
                }

                // Ask it to parse, it will return us the ending pos of itself.
                currentParsePosition = element.Parse(ref markdown, nextElemntStart, nextElementEnd);

                // Add it the the children
                Children.Add(element);
            }
        }
    }
}
