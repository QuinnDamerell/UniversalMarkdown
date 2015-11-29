using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalMarkdown.Helpers
{
    class DebuggingReporter
    {
        /// <summary>
        /// Reports a critical error.
        /// </summary>
        /// <param name="errorText"></param>
        public static void ReportCriticalError(string errorText)
        {
            Debug.WriteLine(errorText);
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }
    }
}
