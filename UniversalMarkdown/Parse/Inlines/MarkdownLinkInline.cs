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
    class MarkdownLinkInline : MarkdownInline
    {
        public string Url { get; set; }

        public MarkdownLinkInline()
            : base(MarkdownInlineType.MarkdownLink)
        { }


        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <returns></returns>
        public static InlineTripCharHelper GetTripChars()
        {
            return new InlineTripCharHelper() { FirstChar = '[', Type = MarkdownInlineType.MarkdownLink };
        }

        /// <summary>
        /// Called when the object should parse it's goods out of the markdown. The markdown, start, and stop are given.
        /// The start and stop are what is returned from the FindNext function below. The object should do it's parsing and
        /// return up to the last pos it used. This can be shorter than what is given to the function in endingPos.
        /// </summary>
        /// <param name="markdown">The markdown</param>
        /// <param name="startingPos">Where the parse should start</param>
        /// <param name="endingPos">Where the parse should end</param>
        /// <returns></returns>
        internal override int Parse(ref string markdown, int startingPos, int endingPos)
        {
            // Find all of the link parts
            int linkTextOpen = Common.IndexOf(ref markdown, '[', startingPos, endingPos);
            int linkTextClose = Common.IndexOf(ref markdown, ']', linkTextOpen, endingPos);
            int linkOpen = Common.IndexOf(ref markdown, '(', linkTextClose, endingPos);
            int linkClose = Common.IndexOf(ref markdown, ')', linkOpen, endingPos);

            // These should always be =
            if (linkTextOpen != startingPos)
            {
                DebuggingReporter.ReportCriticalError("link parse didn't find [ in at the starting pos");
            }
            if (linkClose + 1 != endingPos)
            {
                DebuggingReporter.ReportCriticalError("link parse didn't find ] in at the end pos");
            }

            // Make sure there is something to parse, and not just dead space
            linkTextOpen++;
            if (linkTextClose > linkTextOpen)
            {
                // Parse any children of this link element
                ParseInlineChildren(ref markdown, linkTextOpen, linkTextClose);
            }

            // We can't render links in links. So if anything in the children of this is a link
            // we have to remove it
            for(int count = 0; count < Children.Count; count++)
            {
                // Look through the children for a link, if found grab the text
                MarkdownInlineType type = ((MarkdownInline)Children[count]).Type;
                string replaceText = null;
                if (type == MarkdownInlineType.MarkdownLink)
                {
                    // If it is a link just grab the URL. Ideally we would grab the text
                    // but that is too hard and this will never happen.
                    replaceText = ((MarkdownLinkInline)Children[count]).Url;
                }
                else if (type == MarkdownInlineType.RawHyperlink)
                {
                    replaceText = ((RawHyperlinkInline)Children[count]).Url;
                }
                else if (type == MarkdownInlineType.RawSubreddit)
                {
                    replaceText = ((RawSubredditInline)Children[count]).Text;
                }

                // If we found text to replace add a new text element as the text.
                if(replaceText != null)
                {
                    TextRunInline textRun = new TextRunInline();
                    textRun.Text = replaceText;
                    Children[count] = textRun;
                }
            }

            // Grab the link
            linkOpen++;
            Url = markdown.Substring(linkOpen, linkClose - linkOpen);

            // Return the point after the )
            return linkClose + 1;
        }

        /// <summary>
        /// Verify a match that is found in the markdown. If the match is good and the rest of the element exits the function should
        /// return true and the element will be matched. If if is a false positive return false and we will keep looking.
        /// </summary>
        /// <param name="markdown">The markdown to match</param>
        /// <param name="startingPos">Where the first trip char should be found</param>
        /// <param name="maxEndingPos">The max length to look in.</param>
        /// <param name="elementEndingPos">If found, the ending pos of the element found.</param>
        /// <returns></returns>
        public static bool VerifyMatch(ref string markdown, int startingPos, int maxEndingPos, ref int elementStartingPos, ref int elementEndingPos)
        {
            // Sanity check
            if(markdown[startingPos] == '[')
            {
                int linkTextOpen = startingPos;
                // Ensure we have a link
                int linkTextClose = Common.IndexOf(ref markdown, ']', linkTextOpen, maxEndingPos);
                if (linkTextClose != -1)
                {
                    int linkOpen = Common.IndexOf(ref markdown, '(', linkTextClose, maxEndingPos);
                    if (linkOpen != -1)
                    {
                        char test = markdown[maxEndingPos];
                        int linkClose = Common.IndexOf(ref markdown, ')', linkOpen, maxEndingPos);
                        if (linkClose != -1)
                        {
                            // Make sure the order is correct
                            if (linkTextOpen < linkTextClose && linkTextClose < linkOpen && linkOpen < linkClose)
                            {
                                elementStartingPos = startingPos;
                                elementEndingPos = linkClose + 1;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
