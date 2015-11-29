using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Parse.Elements;

namespace UniversalMarkdown.Parse
{
    public enum MarkdownBlockType
    {
        Root,
        Paragraph,
        Quote,
        Code,
        Header,
        ListElement,
        HorizontalRule
    };

    abstract class MarkdownBlock : MarkdownElement
    {
        /// <summary>
        /// Tells us what type this element is.
        /// </summary>
        public MarkdownBlockType Type { get; set; }

        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        public MarkdownBlock(MarkdownBlockType type)
        {
            Type = type;
        }      
    }
}
