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
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

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
        /// Holds a list of hyperlinks we are listening to.
        /// </summary>
        private List<Hyperlink> m_listeningHyperlinks = new List<Hyperlink>();

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
            if (newMarkdown != null)
            {
                try
                {
                    // Try to parse the markdown.
                    Markdown markdown = new Markdown();
                    markdown.Parse(newMarkdown);

                    // Now try to display it
                    var renderer = new XamlRenderer(this);
                    renderer.Background = Background;
                    renderer.BorderBrush = BorderBrush;
                    renderer.BorderThickness = BorderThickness;
                    renderer.CharacterSpacing = CharacterSpacing;
                    renderer.FontFamily = FontFamily;
                    renderer.FontSize = FontSize;
                    renderer.FontStretch = FontStretch;
                    renderer.FontStyle = FontStyle;
                    renderer.FontWeight = FontWeight;
                    renderer.Foreground = Foreground;
                    renderer.HorizontalAlignment = HorizontalAlignment;
                    renderer.IsTextSelectionEnabled = true;
                    renderer.Padding = Padding;
                    renderer.CodeBackground = new SolidColorBrush(Color.FromArgb(8, 255, 255, 255));
                    renderer.CodeBorderBrush = new SolidColorBrush(Color.FromArgb(32, 255, 255, 255));
                    renderer.CodeBorderThickness = new Thickness(1);
                    renderer.HorizontalRuleBrush = new SolidColorBrush(Color.FromArgb(48, 255, 255, 255));
                    renderer.QuoteBorderBrush = Application.Current.Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush;
                    renderer.TableBorderBrush = new SolidColorBrush(Color.FromArgb(16, 255, 255, 255));
                    Content = renderer.Render(markdown);
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

            // Clear everything that exists.
            m_listeningHyperlinks.Clear();
        }

        // Used to attach the URL to hyperlinks.
        private static readonly DependencyProperty HyperlinkUrlProperty =
            DependencyProperty.RegisterAttached("HyperlinkUrl", typeof(string), typeof(MarkdownTextBlock), new PropertyMetadata(null));

        /// <summary>
        /// Called when the render has a link we need to listen to.
        /// </summary>
        /// <param name="newHyperlink"></param>
        /// <param name="linkUrl"></param>
        public void RegisterNewHyperLink(Hyperlink newHyperlink, string linkUrl)
        {
            // Setup a listener for clicks.
            newHyperlink.Click += Hyperlink_Click;

            // Associate the URL with the hyperlink.
            newHyperlink.SetValue(HyperlinkUrlProperty, linkUrl);

            // Add it to our list
            m_listeningHyperlinks.Add(newHyperlink);
        }

        /// <summary>
        /// Fired when a user taps one of the link elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Hyperlink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            // Get the hyperlink URL.
            var url = (string)sender.GetValue(HyperlinkUrlProperty);
            if (url != null)
            {
                OnMarkdownLinkTappedArgs eventArgs = new OnMarkdownLinkTappedArgs()
                {
                    Link = url
                };

                m_onMarkdownLinkTapped.Raise(this, eventArgs);
            }
        }

        #endregion
    }
}
