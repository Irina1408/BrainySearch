using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.TextProcessing
{
    public class TextProcessingUtils
    {
        /// <summary>
        /// Gets list of stop words
        /// </summary>
        public static List<string> GetStopWords()
        {
            // get executing assembly
            var assembly = Assembly.GetExecutingAssembly();
            // get file
            var sr = new StreamReader(assembly.GetManifestResourceStream("BrainySearch.Logic.TextProcessing.stop-words.txt"));
            // stop words
            var stopWords = new List<string>();
            // read all stop words
            var line = sr.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                stopWords.Add(line);
                line = sr.ReadLine();
            }
            // cleanup
            sr.Close();
            sr.Dispose();

            return stopWords;
        }
    }
}
