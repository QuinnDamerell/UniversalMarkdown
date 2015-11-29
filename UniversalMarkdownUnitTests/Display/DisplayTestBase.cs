using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UniversalMarkdown.Display;
using UniversalMarkdown.Interfaces;
using UniversalMarkdown.Parse;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace UniversalMarkdownUnitTests.Display
{
    /// <summary>
    /// The base class for our display unit tests.
    /// </summary>
    public abstract class DisplayTestBase : TestBase
    {
        /// <summary>
        /// Parses the given markdown into an AST.
        /// </summary>
        /// <param name="markdown"></param>
        /// <returns></returns>
        protected string RenderMarkdown(string markdown)
        {
            var parser = new Markdown();
            parser.Parse(markdown);

            var richTextBlock = new RichTextBlock();
            var renderer = new RenderToRichTextBlock(richTextBlock, new DummyLinkRegister());
            renderer.Render(parser);

            var result = new StringBuilder();
            foreach (var block in richTextBlock.Blocks)
            {
                SerializeElement(result, block, indentLevel: 0);
            }
            return result.ToString();
        }

        private class DummyLinkRegister : ILinkRegister
        {
            public void RegisterNewHyperLink(Hyperlink newHyperlink, string linkUrl)
            {
            }
        }
    }
}
