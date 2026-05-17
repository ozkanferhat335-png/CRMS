using System;
using System.Collections.Generic;

namespace CRMS.Common.Helpers
{
    public static class StringHelper
    {
        public static string Truncate(string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text.Length <= length ? text : text.Substring(0, length) + "...";
        }

        public static string GetInitials(string firstName, string lastName)
        {
            string initials = string.Empty;
            
            if (!string.IsNullOrEmpty(firstName))
                initials += firstName[0].ToString().ToUpper();
            
            if (!string.IsNullOrEmpty(lastName))
                initials += lastName[0].ToString().ToUpper();

            return initials;
        }

        public static string GetFullName(string firstName, string lastName)
        {
            return $"{firstName} {lastName}".Trim();
        }

        public static string FormatCurrency(decimal amount)
        {
            return amount.ToString("C2");
        }

        public static string FormatDate(DateTime date, string format = "dd/MM/yyyy")
        {
            return date.ToString(format);
        }

        public static string FormatDateTime(DateTime dateTime, string format = "dd/MM/yyyy HH:mm:ss")
        {
            return dateTime.ToString(format);
        }

        public static string Capitalize(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }

        public static string RemoveSpecialCharacters(string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, @"[^a-zA-Z0-9\s]", "");
        }
    }
}