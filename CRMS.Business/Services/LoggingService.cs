using System;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class LoggingService
    {
        private readonly UnitOfWork _unitOfWork;

        public LoggingService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void LogAction(int? userId, string action, string tableName, int? recordId, 
            string oldValue = null, string newValue = null, string description = null, string ipAddress = null)
        {
            try
            {
                var log = new Log
                {
                    UserId = userId,
                    Action = action,
                    TableName = tableName,
                    RecordId = recordId,
                    OldValue = oldValue ?? string.Empty,
                    NewValue = newValue ?? string.Empty,
                    Description = description ?? string.Empty,
                    IpAddress = ipAddress ?? string.Empty,
                    CreatedDate = DateTime.Now
                };

                _unitOfWork.Logs.Add(log);
            }
            catch (Exception ex)
            {
                // Log error but don't throw
                Console.WriteLine($"Logging error: {ex.Message}");
            }
        }

        public void LogLogin(int userId, string ipAddress = null)
        {
            LogAction(userId, "Login", "Users", userId, description: "User logged in", ipAddress: ipAddress);
        }

        public void LogLogout(int userId, string ipAddress = null)
        {
            LogAction(userId, "Logout", "Users", userId, description: "User logged out", ipAddress: ipAddress);
        }

        public void LogDataCreate(int? userId, string tableName, int recordId, string newData, string ipAddress = null)
        {
            LogAction(userId, "Create", tableName, recordId, newValue: newData, 
                description: $"Record created in {tableName}", ipAddress: ipAddress);
        }

        public void LogDataUpdate(int? userId, string tableName, int recordId, string oldData, string newData, string ipAddress = null)
        {
            LogAction(userId, "Update", tableName, recordId, oldValue: oldData, newValue: newData, 
                description: $"Record updated in {tableName}", ipAddress: ipAddress);
        }

        public void LogDataDelete(int? userId, string tableName, int recordId, string deletedData, string ipAddress = null)
        {
            LogAction(userId, "Delete", tableName, recordId, oldValue: deletedData, 
                description: $"Record deleted from {tableName}", ipAddress: ipAddress);
        }
    }
}