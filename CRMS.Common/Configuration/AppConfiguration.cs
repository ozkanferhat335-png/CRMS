using System;

namespace CRMS.Common.Configuration
{
    public static class AppConfiguration
    {
        public static string DatabasePath { get; set; } = "CRMS.db";
        public static string AppVersion { get; set; } = "1.0.0";
        public static string AppTitle { get; set; } = "Gelişmiş CRM Uygulaması";
        public static string CompanyName { get; set; } = "Your Company";
        public static bool EnableLogging { get; set; } = true;
        public static bool EnableAudit { get; set; } = true;
        public static int SessionTimeoutMinutes { get; set; } = 120;
        public static int MaxFailedLoginAttempts { get; set; } = 3;
        public static int AccountLockoutMinutes { get; set; } = 15;
    }
}