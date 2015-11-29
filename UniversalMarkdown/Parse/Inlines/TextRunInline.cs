using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalMarkdown.Parse.Elements
{
    class TextRunElement : MarkdownInline
    {
        /// <summary>
        /// The text for this run.
        /// </summary>
        public string Text { get; set; }

        public TextRunElement() :
            base(MarkdownInlineType.TextRun)
        {   }
            
        // Called when this element should parse. This is the lowest level element, if parse gets called we take all of the text.
        internal override int Parse(ref string markdown, int startingPos, int maxEndingPos)
        {
            // Grab all of the text
            Text = markdown.Substring(startingPos, maxEndingPos - startingPos);

            // Return that we ate it all.
            return maxEndingPos;
        }
    }
}
