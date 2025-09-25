using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Common.Helpers
{
    public class StringHelper
    {
        public static string ToFriendlyUrl(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.ToLowerInvariant();
            text = RemoveDiacritics(text);
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = Regex.Replace(text, @"\s+", "-");
            text = Regex.Replace(text, @"-+", "-").Trim('-');

            return text;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}