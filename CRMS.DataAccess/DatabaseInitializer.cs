using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace CRMS.DataAccess
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;
        private readonly string _databasePath;

        public DatabaseInitializer(string databasePath = "CRMS.db")
        {
            _databasePath = databasePath;
            _connectionString = $"Data Source={databasePath};Version=3;";
        }

        public void InitializeDatabase()
        {
            try
            {
                // Create database file if it doesn't exist
                if (!File.Exists(_databasePath))
                {
                    SQLiteConnection.CreateFile(_databasePath);
                }

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    CreateTables(connection);
                    InsertDefaultData(connection);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Database initialization failed: {ex.Message}", ex);
            }
        }

        private void CreateTables(SQLiteConnection connection)
        {
            var commands = new List<string>
            {
                // Roles
                @"CREATE TABLE IF NOT EXISTS Roles (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    RoleName TEXT NOT NULL UNIQUE,
                    Description TEXT,
                    IsActive INTEGER DEFAULT 1,
                    CreatedDate TEXT NOT NULL
                )",

                // Users
                @"CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Email TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    RoleId INTEGER NOT NULL,
                    FirstName TEXT,
                    LastName TEXT,
                    PhoneNumber TEXT,
                    IsActive INTEGER DEFAULT 1,
                    IsLocked INTEGER DEFAULT 0,
                    FailedLoginAttempts INTEGER DEFAULT 0,
                    LastLoginDate TEXT,
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT,
                    CreatedByUserId INTEGER,
                    ModifiedByUserId INTEGER,
                    FOREIGN KEY (RoleId) REFERENCES Roles(Id),
                    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id),
                    FOREIGN KEY (ModifiedByUserId) REFERENCES Users(Id)
                )",

                // Permissions
                @"CREATE TABLE IF NOT EXISTS Permissions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PermissionName TEXT NOT NULL,
                    Description TEXT,
                    Module TEXT,
                    IsActive INTEGER DEFAULT 1,
                    CreatedDate TEXT NOT NULL,
                    RoleId INTEGER NOT NULL,
                    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
                )",

                // Customers
                @"CREATE TABLE IF NOT EXISTS Customers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerCode TEXT NOT NULL UNIQUE,
                    FirstName TEXT,
                    LastName TEXT,
                    CompanyName TEXT,
                    TaxNumber TEXT,
                    Email TEXT,
                    PhoneNumber TEXT,
                    Address TEXT,
                    City TEXT,
                    Country TEXT,
                    Website TEXT,
                    CustomerType TEXT,
                    Status TEXT DEFAULT 'Active',
                    Rating REAL DEFAULT 0,
                    Notes TEXT,
                    LastContactDate TEXT,
                    NextContactDate TEXT,
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT,
                    CreatedByUserId INTEGER,
                    ModifiedByUserId INTEGER,
                    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id),
                    FOREIGN KEY (ModifiedByUserId) REFERENCES Users(Id)
                )",

                // Meetings
                @"CREATE TABLE IF NOT EXISTS Meetings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerId INTEGER NOT NULL,
                    UserId INTEGER NOT NULL,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    MeetingDate TEXT NOT NULL,
                    EndDate TEXT,
                    MeetingType TEXT,
                    Result TEXT,
                    Notes TEXT,
                    Status TEXT DEFAULT 'Planned',
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT,
                    CreatedByUserId INTEGER,
                    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
                )",

                // Tasks
                @"CREATE TABLE IF NOT EXISTS Tasks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    AssignedToUserId INTEGER NOT NULL,
                    CustomerId INTEGER,
                    Priority TEXT DEFAULT 'Medium',
                    Status TEXT DEFAULT 'Pending',
                    DueDate TEXT NOT NULL,
                    CompletedDate TEXT,
                    AssignedByUserId INTEGER,
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT,
                    FOREIGN KEY (AssignedToUserId) REFERENCES Users(Id),
                    FOREIGN KEY (AssignedByUserId) REFERENCES Users(Id),
                    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
                )",

                // Offers
                @"CREATE TABLE IF NOT EXISTS Offers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OfferCode TEXT NOT NULL UNIQUE,
                    CustomerId INTEGER NOT NULL,
                    UserId INTEGER NOT NULL,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    TotalAmount REAL DEFAULT 0,
                    TaxAmount REAL DEFAULT 0,
                    DiscountAmount REAL DEFAULT 0,
                    FinalAmount REAL DEFAULT 0,
                    Status TEXT DEFAULT 'Pending',
                    OfferDate TEXT NOT NULL,
                    ExpiryDate TEXT,
                    ApprovalDate TEXT,
                    Notes TEXT,
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT,
                    CreatedByUserId INTEGER,
                    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
                )",

                // OfferItems
                @"CREATE TABLE IF NOT EXISTS OfferItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    OfferId INTEGER NOT NULL,
                    ItemName TEXT NOT NULL,
                    Description TEXT,
                    Quantity REAL DEFAULT 1,
                    UnitPrice REAL DEFAULT 0,
                    TotalPrice REAL DEFAULT 0,
                    TaxRate REAL DEFAULT 0,
                    TaxAmount REAL DEFAULT 0,
                    CreatedByUserId INTEGER,
                    CreatedDate TEXT NOT NULL,
                    FOREIGN KEY (OfferId) REFERENCES Offers(Id),
                    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
                )",

                // Sales
                @"CREATE TABLE IF NOT EXISTS Sales (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SaleCode TEXT NOT NULL UNIQUE,
                    CustomerId INTEGER NOT NULL,
                    UserId INTEGER NOT NULL,
                    OfferId INTEGER,
                    Status TEXT DEFAULT 'Prospect',
                    Amount REAL DEFAULT 0,
                    TaxAmount REAL DEFAULT 0,
                    FinalAmount REAL DEFAULT 0,
                    SaleDate TEXT NOT NULL,
                    ClosingDate TEXT,
                    Notes TEXT,
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT,
                    CreatedByUserId INTEGER,
                    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    FOREIGN KEY (OfferId) REFERENCES Offers(Id),
                    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
                )",

                // Tickets
                @"CREATE TABLE IF NOT EXISTS Tickets (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    TicketCode TEXT NOT NULL UNIQUE,
                    CustomerId INTEGER NOT NULL,
                    AssignedToUserId INTEGER,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    Priority TEXT DEFAULT 'Medium',
                    Status TEXT DEFAULT 'Open',
                    Category TEXT,
                    Department TEXT,
                    CreatedDate TEXT NOT NULL,
                    ClosedDate TEXT,
                    ModifiedDate TEXT,
                    CreatedByUserId INTEGER,
                    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
                    FOREIGN KEY (AssignedToUserId) REFERENCES Users(Id),
                    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
                )",

                // Notifications
                @"CREATE TABLE IF NOT EXISTS Notifications (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Title TEXT NOT NULL,
                    Message TEXT,
                    NotificationType TEXT,
                    Status TEXT DEFAULT 'Unread',
                    RelatedEntityId INTEGER,
                    RelatedEntityType TEXT,
                    CreatedDate TEXT NOT NULL,
                    ReadDate TEXT,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                )",

                // Logs
                @"CREATE TABLE IF NOT EXISTS Logs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER,
                    Action TEXT NOT NULL,
                    TableName TEXT,
                    RecordId INTEGER,
                    OldValue TEXT,
                    NewValue TEXT,
                    Description TEXT,
                    IpAddress TEXT,
                    CreatedDate TEXT NOT NULL,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                )",

                // Files
                @"CREATE TABLE IF NOT EXISTS Files (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FileName TEXT NOT NULL,
                    FileExtension TEXT,
                    FileSize INTEGER,
                    FilePath TEXT NOT NULL,
                    Category TEXT,
                    CustomerId INTEGER,
                    UserId INTEGER,
                    RelatedEntity TEXT,
                    RelatedEntityId INTEGER,
                    CreatedDate TEXT NOT NULL,
                    CreatedByUserId INTEGER,
                    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
                )",

                // SystemSettings
                @"CREATE TABLE IF NOT EXISTS SystemSettings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SettingKey TEXT NOT NULL UNIQUE,
                    SettingValue TEXT,
                    Description TEXT,
                    SettingType TEXT,
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT,
                    ModifiedByUserId INTEGER,
                    FOREIGN KEY (ModifiedByUserId) REFERENCES Users(Id)
                )",

                // Events
                @"CREATE TABLE IF NOT EXISTS Events (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    StartDate TEXT NOT NULL,
                    EndDate TEXT,
                    EventType TEXT,
                    Location TEXT,
                    CustomerId INTEGER,
                    Status TEXT DEFAULT 'Planned',
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT,
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
                )"
            };

            foreach (var command in commands)
            {
                using (var cmd = new SQLiteCommand(command, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InsertDefaultData(SQLiteConnection connection)
        {
            try
            {
                // Check if data already exists
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Roles", connection))
                {
                    var count = (long)cmd.ExecuteScalar();
                    if (count > 0) return; // Data already exists
                }

                // Insert default roles
                var roleCommands = new List<string>
                {
                    @"INSERT INTO Roles (RoleName, Description, IsActive, CreatedDate) VALUES ('Administrator', 'System Administrator with full access', 1, datetime('now'))",
                    @"INSERT INTO Roles (RoleName, Description, IsActive, CreatedDate) VALUES ('Sales Representative', 'Sales representative role', 1, datetime('now'))",
                    @"INSERT INTO Roles (RoleName, Description, IsActive, CreatedDate) VALUES ('Support Staff', 'Support staff role', 1, datetime('now'))",
                    @"INSERT INTO Roles (RoleName, Description, IsActive, CreatedDate) VALUES ('Manager', 'Manager role', 1, datetime('now'))"
                };

                foreach (var cmd in roleCommands)
                {
                    using (var command = new SQLiteCommand(cmd, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                // Insert default admin user
                using (var cmd = new SQLiteCommand(
                    @"INSERT INTO Users (Username, Email, PasswordHash, RoleId, FirstName, LastName, PhoneNumber, IsActive, FailedLoginAttempts, CreatedDate) 
                      VALUES ('admin', 'admin@crms.com', @hash, 1, 'System', 'Administrator', '+905551234567', 1, 0, datetime('now'))", connection))
                {
                    // SHA256 hash of "admin123"
                    cmd.Parameters.AddWithValue("@hash", "0ba904eae8773b70c75333db4de2f3ac45d512672b8550d69c2b00f2db1d3ca7");
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail initialization
                Console.WriteLine($"Error inserting default data: {ex.Message}");
            }
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}