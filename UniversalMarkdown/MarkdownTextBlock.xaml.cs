using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UniversalMarkdown.Display;
using UniversalMarkdown.Helpers;
using UniversalMarkdown.Parse;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UniversalMarkdown
{
    public sealed partial class MarkdownTextBlock : UserControl
    {
        /// <summary>
        /// Fired when the text is done parsing and formatting.
        /// </summary>
        public event EventHandler<EventArgs> OnMarkdownReady
        {
            add { m_onMarkdownReady.Add(value); }
            remove { m_onMarkdownReady.Remove(value); }
        }
        SmartWeakEvent<EventHandler<EventArgs>> m_onMarkdownReady = new SmartWeakEvent<EventHandler<EventArgs>>();

        public MarkdownTextBlock()
        {
            this.InitializeComponent();
        }

        #region Markdown Logic

        /// <summary>
        /// This it how we get the post form the xmal binding.
        /// </summary>
        public string Markdown
        {
            get { return (string)GetValue(MarkdownProperty); }
            set { SetValue(MarkdownProperty, value); }
        }

        public static readonly DependencyProperty MarkdownProperty =
            DependencyProperty.Register(
                "Markdown",                     // The name of the DependencyProperty
                typeof(string),                   // The type of the DependencyProperty
                typeof(MarkdownTextBlock), // The type of the owner of the DependencyProperty
                new PropertyMetadata(           // OnBlinkChanged will be called when Blink changes
                    false,                      // The default value of the DependencyProperty
                    new PropertyChangedCallback(OnMarkdownChangedStatic)
                ));

        private static void OnMarkdownChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as MarkdownTextBlock;
            if (instance != null)
            {
                // Send the post to the class.
                instance.OnMarkdownChanged(e.NewValue.GetType() == typeof(string) ? (string)e.NewValue : "");
            }
        }

        #endregion

        private void OnMarkdownChanged(string newMarkdown)
        {
            try
            {
                Markdown markdown = new Markdown();
                markdown.Parse(newMarkdown);

                RenderToRichTextBlock rendner = new RenderToRichTextBlock(ui_richTextBox);
                rendner.Render(markdown);
            }
            catch(Exception e)
            {
                // #todo
            }   

            // When done if we set something fire the event.
            if (!String.IsNullOrWhiteSpace(newMarkdown))
            {
                // #todo indicate if ready
                m_onMarkdownReady.Raise(this, new EventArgs());
            }
        }

        private void CleanUpTextBlock()
        {
            //// Clear any hyperlink events if we have any
            //foreach (Hyperlink link in m_hyperLinks)
            //{
            //    link.Click -= HyperLink_Click;
            //}

            //// Clear what exists
            //ui_richTextBox.Blocks.Clear();
            //m_hyperLinkToUrl.Clear();
            //m_hyperLinks.Clear();
        }
    }
}
