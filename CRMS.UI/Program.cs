using System;
using System.Windows.Forms;
using CRMS.DataAccess;
using CRMS.DataAccess.Repositories;
using CRMS.Business.Services;
using CRMS.Common.Configuration;

namespace CRMS.UI
{
    static class Program
    {
        public static DatabaseInitializer DatabaseInitializer { get; private set; }
        public static UnitOfWork UnitOfWork { get; private set; }
        public static AuthenticationService AuthenticationService { get; private set; }
        public static LoggingService LoggingService { get; private set; }
        public static UserDTO CurrentUser { get; set; }

        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Initialize Database
                DatabaseInitializer = new DatabaseInitializer(AppConfiguration.DatabasePath);
                DatabaseInitializer.InitializeDatabase();

                // Initialize UnitOfWork
                UnitOfWork = new UnitOfWork(DatabaseInitializer.GetConnectionString());

                // Initialize Services
                AuthenticationService = new AuthenticationService(UnitOfWork);
                LoggingService = new LoggingService(UnitOfWork);

                // Show Login Form
                Application.Run(new Forms.LoginForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Uygulama başlatılırken hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}