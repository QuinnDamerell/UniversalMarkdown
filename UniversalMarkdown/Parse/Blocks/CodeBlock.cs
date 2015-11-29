using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Helpers;

namespace UniversalMarkdown.Parse.Elements
{
    class CodeBlock : MarkdownBlock
    {
        public int CodeIndent = 0;

        public CodeBlock() 
            : base(MarkdownBlockType.Code)
        { }

        internal override int Parse(ref string markdown, int startingPos, int maxEndingPos)
        {
            // Find where the code begins
            int codeStart = startingPos;
            int spaceCount = 0;
            while (codeStart < markdown.Length && codeStart < maxEndingPos)
            {
                // If we found a space count it
                if (markdown[codeStart] == ' ')
                {
                    spaceCount++;
                }
                else
                {
                    if (spaceCount > 3)
                    {
                        // We found the next char after the code begin
                        break;
                    }
                    else
                    {
                        // We found a char that broke the space count
                        spaceCount = 0;
                    }
                }
                codeStart++;
            }

            if (spaceCount == 0)
            {
                DebuggingReporter.ReportCriticalError("Tried to code but found no space row > 3");
            } 

            // Find the end of code
            int codeEnd = Common.FindNextNewLine(ref markdown, codeStart, maxEndingPos);
            if(codeEnd == -1)
            {
                DebuggingReporter.ReportCriticalError("Tried to code quote that didn't have an end");
                codeEnd = maxEndingPos;
            }

            // For every 4 spaces we want to add an indent
            CodeIndent = (int)Math.Floor(spaceCount / 4.0);

            // Make sure there is something to parse, and not just dead space
            if (codeEnd > codeStart)
            {
                // Parse the children of this quote
                ParseInlineChildren(ref markdown, codeStart, codeEnd);
            }

            // Trim off any extra line endings, except ' ' otherwise we can't do code blocks
            while (codeEnd < markdown.Length && codeEnd < maxEndingPos && Char.IsWhiteSpace(markdown[codeEnd]) && markdown[codeEnd] != ' ')
            {
                codeEnd++;
            }

            // Return where we ended.
            return codeEnd;
        }
    }
}
