﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    }
}