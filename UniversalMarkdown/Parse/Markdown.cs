using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Helpers;
using UniversalMarkdown.Parse.Elements;

namespace UniversalMarkdown.Parse
{
    /// <summary>
    /// A class used to represent abstract markdown.
    /// </summary>
    class Markdown : MarkdownBlock
    {
        public Markdown()
            : base(MarkdownBlockType.Root)
        { }

        public void Parse(string markdownText)
        {
            // Don't do anything if we don't have anything.
            if(String.IsNullOrWhiteSpace(markdownText))
            {
                return;
            }

            // We need to make sure that all text ends with a \r/n so everything is contained in at least something
            markdownText += "\r\n";

            // Parse us.
            Parse(ref markdownText, 0, markdownText.Length);
        }

        /// <summary>
        /// Called when we should parse our blocks
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="maxEndingPos"></param>
        /// <returns></returns>
        internal override int Parse(ref string markdown, int startingPos, int maxEndingPos)
        {
            // We are the only thing that can parse blocks, and we should be the only thing to hold blocks. 
            // So start off by parsing our block children.
            int currentParsePosition = 0;

            while (currentParsePosition < maxEndingPos)
            {
                int elementStartingPos = currentParsePosition;

                // Find the next element
                MarkdownBlock element = FindNextBlock(ref markdown, currentParsePosition, maxEndingPos);

                // Ask it to parse, it will return us the ending pos of itself.
                currentParsePosition = element.Parse(ref markdown, elementStartingPos, maxEndingPos);

                // Add it the the children
                Children.Add(element);
            }

            return maxEndingPos;
        }

        /// <summary>
        /// Called by all elements to find the next element to parse out of the markdown given a startingPos and an ending Pos
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="startingPos"></param>
        /// <param name="endingPost"></param>
        /// <returns></returns>
        public static MarkdownBlock FindNextBlock(ref string markdown, int startingPos, int endingPos)
        {
            MarkdownBlockType nextBlockType = MarkdownBlockType.Paragraph;

            // We need to look at the start of this current block and figure out what type it is.
            // Find the next char that isn't a \n, \r, or ' ', keep track of white space
            int nextCharPos = startingPos;
            int spaceCount = 0;
            while (markdown.Length > nextCharPos && endingPos > nextCharPos && (markdown[nextCharPos] == '\r' || markdown[nextCharPos] == '\n' || Char.IsWhiteSpace(markdown[nextCharPos])))
            {
                // If we find a space count it for the indent rules. If not reset the count.
                spaceCount = markdown[nextCharPos] == ' ' ? spaceCount + 1 : 0;
                nextCharPos++;
            }

            //If we found 4 spaces in a row we have 'code'
            if (spaceCount > 3)
            {
                nextBlockType = MarkdownBlockType.Code;
            }
            // We have a quote; remember to check for the end of the text
            else if (markdown.Length > nextCharPos && endingPos > nextCharPos && markdown[nextCharPos] == '>')
            {
                nextBlockType = MarkdownBlockType.Quote;
            }
            // We have a header;
            else if (markdown.Length > nextCharPos && endingPos > nextCharPos && markdown[nextCharPos] == '#')
            {
                nextBlockType = MarkdownBlockType.Header;
            }
            // We have a list element;
            else if (markdown.Length > nextCharPos + 1 && endingPos > nextCharPos + 1 && (markdown[nextCharPos] == '*' || markdown[nextCharPos] == '-') && markdown[nextCharPos + 1] == ' ')
            {
                nextBlockType = MarkdownBlockType.ListElement;
            }
            // We have a number or letter list
            else if (markdown.Length > nextCharPos + 1 && endingPos > nextCharPos + 1 && Char.IsLetterOrDigit(markdown[nextCharPos]) && markdown[nextCharPos + 1] == '.')
            {
                nextBlockType = MarkdownBlockType.ListElement;
            }
            else if (markdown.IndexOf("*****", nextCharPos) == nextCharPos)
            {
                nextBlockType = MarkdownBlockType.HorizontalRule;
            }
            else
            {
                nextBlockType = MarkdownBlockType.Paragraph;
            }    

            // Now that we have our winner, make the object
            switch (nextBlockType)
            {
                case MarkdownBlockType.Quote:
                    return new QuoteBlock();
                case MarkdownBlockType.Code:
                    return new CodeBlock();
                case MarkdownBlockType.Header:
                    return new HeaderBlock();
                case MarkdownBlockType.ListElement:
                    return new ListElementBlock();
                case MarkdownBlockType.HorizontalRule:
                    return new HorizontalRuleBlock();
                case MarkdownBlockType.Paragraph:
                default:
                    return new ParagraphBlock();
            }
        }
    }
}
