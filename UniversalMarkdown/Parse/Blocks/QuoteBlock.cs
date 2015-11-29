using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Helpers;

namespace UniversalMarkdown.Parse.Elements
{
    class QuoteBlock : MarkdownBlock
    {
        public int QuoteIndent = 0;

        public QuoteBlock() 
            : base(MarkdownBlockType.Quote)
        { }

        internal override int Parse(ref string markdown, int startingPos, int maxEndingPos)
        {
            // Find the start of the quote
            int quoteStart = Common.IndexOf(ref markdown, '>', startingPos, maxEndingPos);
            if(quoteStart == -1)
            {
                DebuggingReporter.ReportCriticalError("Tried to parse quote that didn't exist");
                return maxEndingPos;
            }

            // Find the end of quote
            int quoteEnd = Common.FindNextNewLine(ref markdown, quoteStart, maxEndingPos);
            if(quoteEnd == -1)
            {
                DebuggingReporter.ReportCriticalError("Tried to parse quote that didn't have an end");
                quoteEnd = maxEndingPos;
            }

            // Find how many indents we have
            QuoteIndent = 0;
            while (quoteStart < markdown.Length && quoteStart < quoteEnd && markdown[quoteStart] == '>')
            {
                QuoteIndent++;
                quoteStart++;
            }

            // Make sure there is something to parse, and not just dead space
            if (quoteEnd > quoteStart)
            {
                // Parse the children of this quote
                ParseInlineChildren(ref markdown, quoteStart, quoteEnd);
            }

            // Trim off any extra line endings, except ' ' otherwise we can't do code blocks
            while (quoteEnd < markdown.Length && quoteEnd < maxEndingPos && Char.IsWhiteSpace(markdown[quoteEnd]) && markdown[quoteEnd] != ' ')
            {
                quoteEnd++;
            }

            // Return where we ended.
            return quoteEnd;
        }
    }
}
