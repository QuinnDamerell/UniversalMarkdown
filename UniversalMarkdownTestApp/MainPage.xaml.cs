using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UniversalMarkdownTestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        // Fired when the text changes
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MarkdownTextBlock.Markdown = TextBox.Text;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox.Text = await FileIO.ReadTextAsync(await Package.Current.InstalledLocation.GetFileAsync("InitialContent.md"));
            }
            catch (Exception ex)
            {
                TextBox.Text = ex.Message;
            }
        }

        private async void BenchmarkButton_Click(object sender, RoutedEventArgs e)
        {
            // Read the text to use as a benchmark.
            var simpleMarkdownText = @"
                Xamarin Studio is significantly different to MonoDevelop these days. It's tightly focused
                on mobile dev rather than trying to be a general purpose IDE, and has a lot of additional
                functionality and integrations through the plugin architecture that MD doesn't have.
                Essentially MonoDevelop as most people use it is just a barebones text editor, solution pane etc.
                and provides a base level container for plugins. Xamarin Studio is what you get when all those
                plugins are added. It's like comparing two slices of bread to a club sandwich.

                I use XS in my job daily and it's very capable at what it's designed for.";
            var complexMarkdownText = await FileIO.ReadTextAsync(await Package.Current.InstalledLocation.GetFileAsync("InitialContent.md"));

            // Give the test as good a chance as possible
            // of avoiding garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var stopWatch1 = System.Diagnostics.Stopwatch.StartNew();
            int simpleIterations = 10000;
            for (int i = 0; i < simpleIterations; i++)
            {
                var parser = new UniversalMarkdown.Parse.Markdown();
                parser.Parse(simpleMarkdownText);
            }
            var simpleTimeMs = stopWatch1.ElapsedMilliseconds / (double)simpleIterations;

            var stopWatch2 = System.Diagnostics.Stopwatch.StartNew();
            int complexIterations = 1000;
            for (int i = 0; i < complexIterations; i++)
            {
                var parser = new UniversalMarkdown.Parse.Markdown();
                parser.Parse(complexMarkdownText);
            }
            var complexTimeMs = stopWatch2.ElapsedMilliseconds / (double)complexIterations;

            var dialog = new MessageDialog($"**Benchmark complete**\r\n\r\nTime to parse a simple comment: {simpleTimeMs}ms\r\n\r\nTime to parse a complex document: {complexTimeMs}ms");
            await dialog.ShowAsync();
        }

        private async void MarkdownTextBlock_OnMarkdownLinkTapped(object sender, UniversalMarkdown.OnMarkdownLinkTappedArgs e)
        {
            var dialog = new MessageDialog($"Link clicked: {e.Link}");
            await dialog.ShowAsync();
        }

        private void FontSizePlusButton_Click(object sender, RoutedEventArgs e)
        {
            MarkdownTextBlock.FontSize++;
        }

        private void FontSizeMinusButton_Click(object sender, RoutedEventArgs e)
        {
            MarkdownTextBlock.FontSize--;
        }
    }
}
