using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using UniversalMarkdown;
using UniversalMarkdownTestApp.Code;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
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
            InitEditableProperties();
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

        // Fired when the text changes
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MarkdownTextBlock.Markdown = TextBox.Text;
        }

        private void MarkdownTextBlock_OnMarkdownReady(object sender, OnMarkdownReadyArgs e)
        {
            ErrorContainer.Visibility = e.Exception != null ? Visibility.Visible : Visibility.Collapsed;
            if (e.Exception != null)
                ErrorText.Text = e.Exception.ToString();
        }

        private async void MarkdownTextBlock_OnMarkdownLinkTapped(object sender, UniversalMarkdown.OnMarkdownLinkTappedArgs e)
        {
            var dialog = new MessageDialog($"Link clicked: {e.Link}");
            await dialog.ShowAsync();
        }

        private async void BenchmarkButton_Click(object sender, RoutedEventArgs e)
        {
            //// Read the text to use as a benchmark.
            //var simpleMarkdownText = @"
            //    Xamarin Studio is significantly different to MonoDevelop these days. It's tightly focused
            //    on mobile dev rather than trying to be a general purpose IDE, and has a lot of additional
            //    functionality and integrations through the plugin architecture that MD doesn't have.
            //    Essentially MonoDevelop as most people use it is just a barebones text editor, solution pane etc.
            //    and provides a base level container for plugins. Xamarin Studio is what you get when all those
            //    plugins are added. It's like comparing two slices of bread to a club sandwich.

            //    I use XS in my job daily and it's very capable at what it's designed for.";
            //var complexMarkdownText = await FileIO.ReadTextAsync(await Package.Current.InstalledLocation.GetFileAsync("InitialContent.md"));

            //// Give the test as good a chance as possible
            //// of avoiding garbage collection
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();

            var elapsedTimeInMs = await BenchmarkRunner.Run(TimeSpan.FromSeconds(5));

            
            var dialog = new MessageDialog($"**Benchmark complete**\r\n\r\nTime to parse 2,807 comments: {elapsedTimeInMs:f2}ms");
            await dialog.ShowAsync();
        }




        public class EditableProperty : INotifyPropertyChanged
        {
            private MarkdownTextBlock instance;
            private PropertyInfo property;
            private bool CanRead;
            private bool CanWrite;
            private MethodInfo GetMethod;

            public EditableProperty(MarkdownTextBlock instance, PropertyInfo property)
            {
                this.instance = instance;
                this.property = property;
                this.CanRead = property.CanRead;
                this.CanWrite = property.CanWrite;
                this.GetMethod = property.GetGetMethod();
                this.Name = property.Name;
                this.value = ConvertToString(property.GetValue(instance));
            }

            /// <summary>
            /// The name of the editable property.
            /// </summary>
            public string Name { get; private set; }

            private string value;

            /// <summary>
            /// The textual value of the editable property.
            /// </summary>
            public string Value
            {
                get { return this.value; }
                set
                {
                    try
                    {
                        this.value = value;
                        this.property.SetValue(this.instance, ConvertFromString(this.property.PropertyType, value));
                        HasBindingError = false;
                    }
                    catch
                    {
                        HasBindingError = true;
                    }
                }
            }

            private static string ConvertToString(object value)
            {
                if (value == null)
                    return "(null)";
                else if (value is FontFamily)
                {
                    return ((FontFamily)value).Source;
                }
                else if (value is SolidColorBrush)
                {
                    var color = ((SolidColorBrush)value).Color;
                    return $"{color.A},{color.R},{color.G},{color.B}";
                }
                else if (value is FontWeight)
                {
                    return ((FontWeight)value).Weight.ToString();
                }
                else
                    return value.ToString();
            }

            private static object ConvertFromString(Type propertyType, string value)
            {
                if (value == "(null)")
                    return null;
                else if (propertyType == typeof(string))
                {
                    return value;
                }
                else if (propertyType == typeof(bool))
                {
                    return bool.Parse(value);
                }
                else if (propertyType == typeof(int))
                {
                    return int.Parse(value);
                }
                else if (propertyType == typeof(double))
                {
                    return double.Parse(value);
                }
                else if (propertyType == typeof(Thickness))
                {
                    var components = value.Split(',');
                    if (components.Length == 1)
                    {
                        return new Thickness(double.Parse(components[0]));
                    }
                    else if (components.Length == 4)
                    {
                        return new Thickness(
                            double.Parse(components[0]),
                            double.Parse(components[1]),
                            double.Parse(components[2]),
                            double.Parse(components[3]));
                    }
                    else
                        throw new FormatException("Please enter one or four numbers, separated by commas.");
                }
                else if (propertyType == typeof(FontFamily))
                {
                    return new FontFamily(value);
                }
                else if (propertyType == typeof(FontWeight))
                {
                    return new FontWeight { Weight = ushort.Parse(value) };
                }
                else if (propertyType == typeof(Brush))
                {
                    var components = value.TrimStart('#').Split(',');
                    if (components.Length != 4)
                        throw new FormatException("Please enter one or four integers, separated by commas.");
                    return new SolidColorBrush(Color.FromArgb(
                            byte.Parse(components[0]),
                            byte.Parse(components[1]),
                            byte.Parse(components[2]),
                            byte.Parse(components[3])));
                }
                else if (typeof(Enum).IsAssignableFrom(propertyType))
                {
                    return Enum.Parse(propertyType, value);
                }
                else
                    throw new NotSupportedException("Unsupported property type.");
            }

            private bool hasBindingError;

            /// <summary>
            /// Indicates whether there is a problem with the value.
            /// </summary>
            public bool HasBindingError
            {
                get { return this.hasBindingError; }
                set
                {
                    this.hasBindingError = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BorderBrush)));
                }
            }

            public Brush BorderBrush
            {
                get { return HasBindingError ? new SolidColorBrush(Colors.Red) : null; }
            }

            /// <summary>
            /// Notifies subscribers that a property has changed.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;
        }

        public List<EditableProperty> EditableProperties { get; set; }

        private void InitEditableProperties()
        {
            EditableProperties = new List<EditableProperty>();
            foreach (var property in MarkdownTextBlock.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).OrderBy(p => p.Name))
            {
                var propertyType = property.PropertyType;
                if (property.CanRead == false || property.CanWrite == false)
                    continue;
                if (!(propertyType == typeof(string)) && !(propertyType == typeof(double)) &&
                    !(propertyType == typeof(bool)) && !(propertyType == typeof(int)) &&
                    !(propertyType == typeof(Thickness)) && !(propertyType == typeof(FontFamily)) &&
                    !(propertyType == typeof(FontWeight)) && !(propertyType == typeof(Brush)) &&
                    !(typeof(Enum).IsAssignableFrom(propertyType)))
                    continue;
                if (property.Name == "Markdown" || property.Name == "Name")
                    continue;
                EditableProperties.Add(new EditableProperty(MarkdownTextBlock, property));
            }
        }
    }
}
