using System;
using System.Collections.Generic;

namespace CRMS.Common.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return false;

            // Remove common phone number characters
            string cleaned = phoneNumber.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("+", "");
            
            // Check if it contains only digits
            return cleaned.Length >= 10 && cleaned.Length <= 15 && IsNumeric(cleaned);
        }

        public static bool IsValidTaxNumber(string taxNumber)
        {
            return !string.IsNullOrEmpty(taxNumber) && taxNumber.Length >= 5;
        }

        public static bool IsValidPassword(string password)
        {
            // At least 8 characters, 1 uppercase, 1 lowercase, 1 number
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpper = false, hasLower = false, hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsLower(c)) hasLower = true;
                if (char.IsDigit(c)) hasDigit = true;
            }

            return hasUpper && hasLower && hasDigit;
        }

        public static bool IsNumeric(string text)
        {
            return !string.IsNullOrEmpty(text) && double.TryParse(text, out _);
        }

        public static bool IsDecimal(string text)
        {
            return !string.IsNullOrEmpty(text) && decimal.TryParse(text, out _);
        }

        public static bool IsEmpty(string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }
    }
}