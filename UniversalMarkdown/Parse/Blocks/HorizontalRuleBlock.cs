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

        internal override int Parse(ref string markdown, int startingPos, int maxEndingPos)
        {
            // Find where the list begins
            int horzStart = Common.IndexOf(ref markdown, '*', startingPos, maxEndingPos);
            if (horzStart == -1)
            {
                DebuggingReporter.ReportCriticalError("Tried parse horizontal rule but didn't find a *");
            }

            // Find the end of the * line
            int horzEnd = horzStart;
            while (horzEnd < markdown.Length && horzEnd < maxEndingPos)
            {
                if (markdown[horzEnd] != '*')
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
    }
}
