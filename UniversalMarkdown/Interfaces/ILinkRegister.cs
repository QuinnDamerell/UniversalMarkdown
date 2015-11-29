using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;

namespace UniversalMarkdown.Interfaces
{
    public interface ILinkRegister
    {
        void RegisterNewHyperLink(Hyperlink newHyperlink, string linkUrl);
    }
}
