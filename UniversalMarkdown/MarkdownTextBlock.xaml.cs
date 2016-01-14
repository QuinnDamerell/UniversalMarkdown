// Copyright (c) 2016 Quinn Damerell
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using UniversalMarkdown.Display;
using UniversalMarkdown.Helpers;
using UniversalMarkdown.Interfaces;
using UniversalMarkdown.Parse;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace UniversalMarkdown
{
    public class OnMarkdownReadyArgs : EventArgs        
    {
        public bool WasError = false;
        public Exception Exception = null;
    }

    public class OnMarkdownLinkTappedArgs : EventArgs
    {
        public string Link;
    }

    public sealed partial class MarkdownTextBlock : UserControl, ILinkRegister
    {
        /// <summary>
        /// Fired when the text is done parsing and formatting.
        /// </summary>
        public event EventHandler<OnMarkdownReadyArgs> OnMarkdownReady
        {
            add { m_onMarkdownReady.Add(value); }
            remove { m_onMarkdownReady.Remove(value); }
        }
        SmartWeakEvent<EventHandler<OnMarkdownReadyArgs>> m_onMarkdownReady = new SmartWeakEvent<EventHandler<OnMarkdownReadyArgs>>();

        /// <summary>
        /// Fired when a link element in the markdown was tapped.
        /// </summary>
        public event EventHandler<OnMarkdownLinkTappedArgs> OnMarkdownLinkTapped
        {
            add { m_onMarkdownLinkTapped.Add(value); }
            remove { m_onMarkdownLinkTapped.Remove(value); }
        }
        SmartWeakEvent<EventHandler<OnMarkdownLinkTappedArgs>> m_onMarkdownLinkTapped = new SmartWeakEvent<EventHandler<OnMarkdownLinkTappedArgs>>();

        /// <summary>
        /// Holds a list of hyper links we are listening to
        /// </summary>
        private List<Hyperlink> m_listeningHyperlinks = new List<Hyperlink>();

        /// <summary>
        /// This is annoying. When a user clicks a link there is no way to associate the url to the
        /// event listener unless we set it as the URI. If we do then it will open the browser.
        /// So we use this map to associate link text with a url.
        /// </summary>
        private Dictionary<Hyperlink, string> m_hyperLinkToUrl = new Dictionary<Hyperlink, string>();

        /// <summary>
        /// Creates a new markdown text block
        /// </summary>
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
                "Markdown",  
                typeof(string),      
                typeof(MarkdownTextBlock),
                new PropertyMetadata("", new PropertyChangedCallback(OnMarkdownChangedStatic)
                ));

        private static void OnMarkdownChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as MarkdownTextBlock;
            if (instance != null)
            {
                // Send the post to the class.
                instance.OnMarkdownChanged((string)e.NewValue);
            }
        }

        /// <summary>
        /// Fired when the markdown is changed. 
        /// </summary>
        /// <param name="newMarkdown"></param>
        private void OnMarkdownChanged(string newMarkdown)
        {
            OnMarkdownReadyArgs args = new OnMarkdownReadyArgs();

            // Clear the current content
            CleanUpTextBlock();

            // Make sure we have something to parse.
            if (!String.IsNullOrWhiteSpace(newMarkdown))
            {
                try
                {
                    // Try to parse the markdown.
                    Markdown markdown = new Markdown();
                    markdown.Parse(newMarkdown);

                    // Now try to display it
                    RenderToRichTextBlock rendner = new RenderToRichTextBlock(ui_richTextBox, this);
                    rendner.Render(markdown);
                }
                catch (Exception e)
                {
                    DebuggingReporter.ReportCriticalError("Error while parsing and rendering: " + e.Message);
                    args.WasError = true;
                    args.Exception = e;
                }
            }

            // #todo indicate if ready
            m_onMarkdownReady.Raise(this, args);            
        }

        #endregion

        #region Link Logic

        private void CleanUpTextBlock()
        {
            // Clear any hyper link events if we have any
            foreach (Hyperlink link in m_listeningHyperlinks)
            {
                link.Click -= Hyperlink_Click;
            }

            // Clear everything that exists
            ui_richTextBox.Blocks.Clear();
            m_hyperLinkToUrl.Clear();
            m_listeningHyperlinks.Clear();
        }

        /// <summary>
        /// Called when the render has a link we need to listen to.
        /// </summary>
        /// <param name="newHyperlink"></param>
        /// <param name="linkUrl"></param>
        public void RegisterNewHyperLink(Hyperlink newHyperlink, string linkUrl)
        {
            // Setup a listener for clicks
            newHyperlink.Click += Hyperlink_Click;

            // Add it to our list
            m_listeningHyperlinks.Add(newHyperlink);

            // Add the url and hyper link to the map
            m_hyperLinkToUrl.Add(newHyperlink, linkUrl);
        }

        /// <summary>
        /// Fired when a user taps one of the link elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Hyperlink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            if (m_hyperLinkToUrl.ContainsKey(sender))
            {
                string link = m_hyperLinkToUrl[sender];

                OnMarkdownLinkTappedArgs eventArgs = new OnMarkdownLinkTappedArgs()
                {
                    Link = link
                };

                m_onMarkdownLinkTapped.Raise(this, eventArgs);
            }
        }

        #endregion
    }
}
