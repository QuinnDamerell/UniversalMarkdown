using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Helpers;

namespace UniversalMarkdown.Parse.Elements
{
    class HeaderBlock : MarkdownBlock
    {
        public int HeaderLevel = 0;

        public HeaderBlock()
            : base(MarkdownBlockType.Header)
        { }

        internal override int Parse(ref string markdown, int startingPos, int maxEndingPos)
        {
            // Find the # 
            int headerStart = Common.IndexOf(ref markdown, '#', startingPos, maxEndingPos);
            if (headerStart == -1)
            {
                DebuggingReporter.ReportCriticalError("Tried to header but # wasn't found");
                return maxEndingPos;
            }

            // Find the end of header
            int headerEnd = Common.FindNextNewLine(ref markdown, headerStart, maxEndingPos);
            if (headerEnd == -1)
            {
                DebuggingReporter.ReportCriticalError("Tried to parse header that didn't have an end");
                headerEnd = maxEndingPos;
            }

            // Find how many are in a row
            while (headerStart < markdown.Length && headerStart < maxEndingPos && markdown[headerStart] == '#')
            {
                HeaderLevel++;
                headerStart++;

                // To match reddit's formatting if there are more than 6 we should start showing them.
                if (HeaderLevel > 5)
                {
                    break;
                }
            }

            // Make sure there is something to parse, and not just dead space
            if (headerEnd > headerStart)
            {
                // Parse the children of this quote
                ParseInlineChildren(ref markdown, headerStart, headerEnd);
            }

            // Trim off any extra line endings, except ' ' otherwise we can't do code blocks
            while (headerEnd < markdown.Length && headerEnd < maxEndingPos && Char.IsWhiteSpace(markdown[headerEnd]) && markdown[headerEnd] != ' ')
            {
                headerEnd++;
            }

            // Return where we ended.
            return headerEnd;
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
            return markdown.Length > nextCharPos && endingPos > nextCharPos && markdown[nextCharPos] == '#';
        }
    }
}
