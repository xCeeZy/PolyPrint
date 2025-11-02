using System;
using System.Globalization;
using System.Linq;

namespace PolyPrint.AppData
{
    public static class StringHelper
    {
        public static string Normalize(string value)
        {
            return value?.Trim() ?? string.Empty;
        }

        public static string NormalizeMultiline(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : string.Join(Environment.NewLine, value.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(line => line.Trim()));
        }

        public static string CapitalizeFirstLetter(string value)
        {
            string normalized = Normalize(value);
            if (string.IsNullOrEmpty(normalized))
            {
                return string.Empty;
            }

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToUpper(normalized.Substring(0, 1)) + normalized.Substring(1);
        }

        public static string CapitalizeWords(string value)
        {
            string normalized = Normalize(value);
            if (string.IsNullOrEmpty(normalized))
            {
                return string.Empty;
            }

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(normalized);
        }

        public static string ExtractDigits(string value)
        {
            return new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
        }

        public static string NormalizePhone(string value)
        {
            string digits = ExtractDigits(value);
            if (string.IsNullOrEmpty(digits))
            {
                return string.Empty;
            }

            if (digits.Length == 10)
            {
                digits = "7" + digits;
            }

            return "+" + digits;
        }

        public static string SanitizeForMetadata(string value, char separator)
        {
            return Normalize(value).Replace(separator.ToString(), string.Empty);
        }
    }
}
