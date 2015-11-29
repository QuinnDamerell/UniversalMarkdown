using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Helpers;

namespace UniversalMarkdown.Parse.Elements
{
    class ParagraphBlock : MarkdownBlock
    {
        public ParagraphBlock() 
            : base(MarkdownBlockType.Paragraph)
        { }

        internal override int Parse(ref string markdown, int startingPos, int maxEndingPos)
        {
            // Find the end of paragraph
            int endingPos = Common.FindNextNewLine(ref markdown, startingPos, maxEndingPos);

            // Make sure there is something to parse, and not just dead space
            if (endingPos > startingPos)
            {
                // Parse the children of this paragraph
                ParseInlineChildren(ref markdown, startingPos, endingPos);
            }

            // Trim off any extra line endings, except ' ' otherwise we can't do code blocks
            while (endingPos < markdown.Length && endingPos < maxEndingPos && Char.IsWhiteSpace(markdown[endingPos]) && markdown[endingPos] != ' ')
            {
                endingPos++;
            }

            // Return where we ended.
            return endingPos;
        }
    }
}
