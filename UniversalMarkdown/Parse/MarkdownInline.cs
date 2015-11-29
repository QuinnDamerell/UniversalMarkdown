using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Parse.Elements;

namespace UniversalMarkdown.Parse
{
    public enum MarkdownInlineType
    {
        TextRun,
        Bold,
        Italic,
        MarkdownLink,
        RawHyperlink,
        RawSubreddit
    };

    abstract class MarkdownInline : MarkdownElement
    {
        /// <summary>
        /// Tells us what type this element is.
        /// </summary>
        public MarkdownInlineType Type { get; set; }

        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        public MarkdownInline(MarkdownInlineType type)
        {
            Type = type;
        }
    }
}
