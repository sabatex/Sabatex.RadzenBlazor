using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sabatex.Blazor
{
    public static class Globalization
    {
        static string DefaultCultureName = "en-US";
        static List<string> SupportedCultures = new List<string>() {DefaultCultureName};

        
        static IEnumerable<string> GetLanguagesFromAcceptLanguage(string? acceptLanguageHeader)
        {
            if (string.IsNullOrWhiteSpace(acceptLanguageHeader))
                yield break;

        
            int start = 0;
            int length = acceptLanguageHeader.Length;
            while (start < length)
            {
                int end = start;
                while (end < length && acceptLanguageHeader[end] != ',' && acceptLanguageHeader[end] != ';')
                {
                    end++;
                }
                string languageRange = acceptLanguageHeader[start..end].Trim();
                if (!string.IsNullOrEmpty(languageRange))
                {
                    yield return languageRange;
                }

                // Пропускаємо до наступного елемента після коми або крапки з комою
                while (end < length && (acceptLanguageHeader[end] == ',' || acceptLanguageHeader[end] == ';'))
                {
                    end++;
                }
                start = end;
            }
        }

        /// <summary>
        /// Get  Culture by Accept-Language or default en-US
        /// </summary>
        /// <param name="acceptLanguageHeader"></param>
        /// <returns></returns>
        public static string GetSupportedCultureByAcceptLanguage(string? acceptLanguageHeader)
        {
            foreach (var language in GetLanguagesFromAcceptLanguage(acceptLanguageHeader))
            {
                var c = SupportedCultures.FirstOrDefault(c => c == language);
                if (c != null)
                    return c;
            }
            return DefaultCultureName;
        }

        /// <summary>
        /// Add culture to supported cultures
        /// </summary>
        /// <param name="cultureName"></param>
        public static void AddCulture(string cultureName)
        {
            SupportedCultures.Add(cultureName);
        }
    }
}
