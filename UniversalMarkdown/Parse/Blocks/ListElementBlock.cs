using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Helpers;

namespace UniversalMarkdown.Parse.Elements
{
    class ListElementBlock : MarkdownBlock
    {
        public int ListIndent = 0;

        public string ListBullet = "•";

        public ListElementBlock() 
            : base(MarkdownBlockType.ListElement)
        { }

        internal override int Parse(ref string markdown, int startingPos, int maxEndingPos)
        {
            // Find where the list begins
            int listStart = startingPos;
            while (listStart < markdown.Length && listStart < maxEndingPos)
            {
                if (markdown[listStart] == ' ')
                {
                    ListIndent++;
                }
                // We have a bullet list
                else if (markdown[listStart] == '*' || markdown[listStart] == '-')
                {
                    break;
                }
                // We have a number or letter list
                else if(Char.IsLetterOrDigit(markdown[listStart]))
                {
                    // Grab the list letter
                    ListBullet = markdown[listStart] + ".";

                    // +1 to move past the .
                    listStart++;
                    break;
                }
                listStart++;
            }

            // Find the end of the list
            int listEnd = Common.FindNextNewLine(ref markdown, listStart, maxEndingPos);
            if (listEnd == -1)
            {
                DebuggingReporter.ReportCriticalError("Tried to parse list that didn't have an end");
                listEnd = maxEndingPos;
            }

            // Remove one indent from the list. This doesn't work exactly like reddit's
            // but it is close enough
            ListIndent = Math.Max(1, ListIndent - 1);

            // Jump past the *
            listStart++;

            // Make sure there is something to parse, and not just dead space
            if (listEnd > listStart)
            {
                // Parse the children of this list
                ParseInlineChildren(ref markdown, listStart, listEnd);
            }

            // Trim off any extra line endings, except ' ' otherwise we can't do code blocks
            while (listEnd < markdown.Length && listEnd < maxEndingPos && Char.IsWhiteSpace(markdown[listEnd]) && markdown[listEnd] != ' ')
            {
                listEnd++;
            }

            // Return where we ended.
            return listEnd;
        }
    }
}
