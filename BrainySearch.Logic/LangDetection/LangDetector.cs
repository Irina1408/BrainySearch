using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.LangDetection
{
    public class LangDetector
    {
        public static string DetectLanguage(string text)
        {
            var detector = new LanguageDetection.LanguageDetector();
            detector.AddAllLanguages();
            return detector.Detect(text);
        }
    }
}
