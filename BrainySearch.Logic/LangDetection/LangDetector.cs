using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.LangDetection
{
    public class LangDetector
    {
        public static string Detect(string text)
        {
            var detector = new LanguageDetection.LanguageDetector();
            //detector.AddAllLanguages();
            detector.AddLanguages("ru", "en", "uk");
            return detector.Detect(text);
        }

        public static List<string> DetectAll(string text)
        {
            var detector = new LanguageDetection.LanguageDetector();
            //detector.AddAllLanguages();
            detector.AddLanguages("ru", "en", "uk");
            return detector.DetectAll(text).OrderByDescending(item => item.Probability).Select(item => item.Language).ToList();
        }
    }
}
