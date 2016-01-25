using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace UniversalMarkdownTestApp.Code
{
    public class BenchmarkRunner
    {
        public static async Task<double> Run(TimeSpan duration)
        {
            // Read comments out of the zip file.
            var comments = new List<string>();
            var zipFile = await Package.Current.InstalledLocation.GetFileAsync("Comments.zip");
            using (var stream = await zipFile.OpenReadAsync())
            using (var decompressor = new ZipArchive(stream.AsStream()))
            {
                foreach (var entry in decompressor.Entries)
                {
                    using (var zipEntryStream = entry.Open())
                    {
                        comments.Add(new StreamReader(zipEntryStream).ReadToEnd());
                    }
                }
            }

            // Give the test as good a chance as possible
            // of avoiding garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();


            // Warm up the cache.
            var stopWatch = Stopwatch.StartNew();
            do
            {
                RunTest(comments);
            } while (stopWatch.Elapsed < TimeSpan.FromSeconds(0.2));
            
            // Start measuring for real.
            stopWatch = Stopwatch.StartNew();
            int iterationCount = 0;
            do
            {
                RunTest(comments);
                iterationCount++;
            } while (stopWatch.Elapsed < duration);

            return stopWatch.Elapsed.TotalMilliseconds / iterationCount;
        }

        private static void RunTest(List<string> comments)
        {
            // Parse each comment.
            foreach (var commentText in comments)
            {
                var parser = new UniversalMarkdown.Parse.MarkdownDocument();
                parser.Parse(commentText);
            }
        }
    }
}
