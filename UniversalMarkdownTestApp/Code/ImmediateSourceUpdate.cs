using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UniversalMarkdownTestApp.Code
{
    public class ImmediateSourceUpdate : DependencyObject
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool),
            typeof(ImmediateSourceUpdate),
            new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        private static void OnIsEnabledChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var txt = d as TextBox;
            if (txt != null)
            {
                if ((bool)e.NewValue)
                    txt.TextChanged += txt_TextChanged;
                else
                    txt.TextChanged -= txt_TextChanged;
            }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(string),
            typeof(ImmediateSourceUpdate),
            new PropertyMetadata(default(string)));

        public static string GetSource(DependencyObject d)
        {
            return (string)d.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject d, string value)
        {
            d.SetValue(SourceProperty, value);
        }

        private static void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            txt.SetValue(ImmediateSourceUpdate.SourceProperty, txt.Text);
        }
    }
}
