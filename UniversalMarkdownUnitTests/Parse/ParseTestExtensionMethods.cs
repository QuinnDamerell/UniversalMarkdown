using UniversalMarkdown.Parse;

namespace UniversalMarkdownUnitTests.Parse
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ParseTestExtensionMethods
    {
        /// <summary>
        /// Adds one or more child elements to the given parent object.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static T AddChildren<T>(this T parent, params MarkdownElement[] elements) where T : MarkdownElement
        {
            foreach (var child in elements)
                parent.Children.Add(child);
            return parent;
        }
    }
}
