using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mail;

namespace PolyPrint.AppData
{
    public static class ValidationHelper
    {
        public static bool RequireNotEmpty(IDictionary<string, string> fields, out string errorMessage)
        {
            foreach (KeyValuePair<string, string> field in fields)
            {
                if (string.IsNullOrWhiteSpace(field.Value))
                {
                    errorMessage = $"Поле \"{field.Key}\" должно быть заполнено.";
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }

        public static bool ValidateEmail(string email, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessage = null;
                return true;
            }

            try
            {
                _ = new MailAddress(email);
                errorMessage = null;
                return true;
            }
            catch (FormatException)
            {
                errorMessage = "Введите корректный e-mail.";
                return false;
            }
        }

        public static bool ValidatePhone(string phone, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                errorMessage = "Укажите телефон.";
                return false;
            }

            string digits = StringHelper.ExtractDigits(phone);
            if (digits.Length < 10)
            {
                errorMessage = "Телефон должен содержать не менее 10 цифр.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public static bool TryParseDecimal(string input, string fieldName, out decimal value, out string errorMessage)
        {
            if (decimal.TryParse(input, NumberStyles.Number, CultureInfo.CurrentCulture, out value) ||
                decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
            {
                errorMessage = null;
                return true;
            }

            errorMessage = $"Поле \"{fieldName}\" указано некорректно.";
            return false;
        }

        public static bool TryParseInt(string input, string fieldName, out int value, out string errorMessage)
        {
            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.CurrentCulture, out value) ||
                int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                errorMessage = null;
                return true;
            }

            errorMessage = $"Поле \"{fieldName}\" указано некорректно.";
            return false;
        }

        public static bool EnsureDateSelected(DateTime? date, string fieldName, out string errorMessage)
        {
            if (date.HasValue)
            {
                errorMessage = null;
                return true;
            }

            errorMessage = $"Выберите значение для \"{fieldName}\".";
            return false;
        }
    }
}
