using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UniversalMarkdownUnitTests.Display
{
    [TestClass]
    public class HorizontalRuleTests : DisplayTestBase
    {
        private static string hrText = @"
                Paragraph Margin: '0,12,0,12'
                    InlineUIContainer
                        Grid BorderBrush: null, CacheMode: null, ChildrenTransitions: null, Clip: null, DataContext: null, Height: 2, Margin: '0,0,0,0', Name: '', PointerCaptures: null, Projection: null, Style: null, Tag: null, Transform3D: null, Transitions: null
                            SolidColorBrush
                                Color A: 255, B: 153, G: 153, R: 153
                                MatrixTransform
                                MatrixTransform
                            TextBlock CacheMode: null, Clip: null, DataContext: null, Margin: '0,0,0,0', Name: '', PointerCaptures: null, Projection: null, Style: null, Tag: null, Text: 'This is Quinn writing magic text. You will never see this. Like a ghost! I love Marilyn Welniak! This needs to be really long! RRRRREEEEEAAAAALLLLYYYYY LLLOOOONNNGGGG. This is Quinn writing magic text. You will never see this. Like a ghost! I love Marilyn Welniak! This needs to be really long! RRRRREEEEEAAAAALLLLYYYYY LLLOOOONNNGGGG', TextReadingOrder: DetectFromContent, Transform3D: null, Transitions: null
                                SolidColorBrush
                                    Color A: 255, B: 153, G: 153, R: 153
                                    MatrixTransform
                                    MatrixTransform
                                Run Text: 'This is Quinn writing magic text. You will never see this. Like a ghost! I love Marilyn Welniak! This needs to be really long! RRRRREEEEEAAAAALLLLYYYYY LLLOOOONNNGGGG. This is Quinn writing magic text. You will never see this. Like a ghost! I love Marilyn Welniak! This needs to be really long! RRRRREEEEEAAAAALLLLYYYYY LLLOOOONNNGGGG'
                                    SolidColorBrush
                                        Color A: 255, B: 153, G: 153, R: 153
                                        MatrixTransform
                                        MatrixTransform
                                MatrixTransform
                                ResourceDictionary Source: null
                                SolidColorBrush
                                    Color A: 255, B: 41, G: 135, R: 204
                                    MatrixTransform
                                    MatrixTransform
                            MatrixTransform
                            ResourceDictionary Source: null";

        [UITestMethod]
        public void HorizontalRule_Simple()
        {
            string result = RenderMarkdown("*****");
            Assert.AreEqual(CollapseWhitespace(hrText), result);    // TODO
        }

        [UITestMethod]
        public void HorizontalRule_SixStars()
        {
            string result = RenderMarkdown("******");
            Assert.AreEqual(CollapseWhitespace(hrText), result);    // TODO
        }

        [UITestMethod]
        public void HorizontalRule_BeforeAfter()
        {
            // Text on other lines is okay.
            string result = RenderMarkdown(CollapseWhitespace(@"
                before
                *****
                after"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'" +
                    hrText + @"
                Paragraph
                    Run Text: 'after'"), result);    // TODO
        }

        [UITestMethod]
        public void HorizontalRule_Negative()
        {
            // Text on the same line is not.
            string result = RenderMarkdown(CollapseWhitespace(@"
                before
                *****d
                after"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'
                    Run Text: '*****d'
                    Run Text: 'after'"), result);
        }

        [UITestMethod]
        public void HorizontalRule_Negative_FourStars()
        {
            // Also, must be at least 5 stars.
            string result = RenderMarkdown(CollapseWhitespace(@"
                before
                ****
                after"));
            Assert.AreEqual(CollapseWhitespace(@"
                Paragraph
                    Run Text: 'before'
                    Run Text: '****'
                    Run Text: 'after'"), result);
        }
    }
}
              