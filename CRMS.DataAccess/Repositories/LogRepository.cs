using System;
using System.Collections.Generic;
using System.Data.SQLite;
using CRMS.Entity.Models;

namespace CRMS.DataAccess.Repositories
{
    public class LogRepository : IRepository<Log>
    {
        private readonly string _connectionString;

        public LogRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Log GetById(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Logs WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapToLog(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving log: {ex.Message}", ex);
            }
            return null;
        }

        public IEnumerable<Log> GetAll()
        {
            var logs = new List<Log>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Logs ORDER BY CreatedDate DESC", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add(MapToLog(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving logs: {ex.Message}", ex);
            }
            return logs;
        }

        public IEnumerable<Log> Find(System.Linq.Expressions.Expression<Func<Log, bool>> predicate)
        {
            return GetAll().AsEnumerable().Where(predicate.Compile());
        }

        public void Add(Log entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO Logs (UserId, Action, TableName, RecordId, OldValue, NewValue, 
                               Description, IpAddress, CreatedDate) 
                               VALUES (@userId, @action, @tableName, @recordId, @oldValue, @newValue, 
                               @description, @ipAddress, @createdDate)";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding log: {ex.Message}", ex);
            }
        }

        public void Update(Log entity)
        {
            // Logs are typically immutable
            throw new NotImplementedException("Logs are immutable and cannot be updated.");
        }

        public void Delete(Log entity)
        {
            throw new NotImplementedException("Logs are typically not deleted.");
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException("Logs are typically not deleted.");
        }

        public int SaveChanges()
        {
            return 1;
        }

        private Log MapToLog(SQLiteDataReader reader)
        {
            return new Log
            {
                Id = (int)reader["Id"],
                UserId = reader["UserId"] != DBNull.Value ? (int)reader["UserId"] : (int?)null,
                Action = reader["Action"].ToString(),
                TableName = reader["TableName"]?.ToString() ?? string.Empty,
                RecordId = reader["RecordId"] != DBNull.Value ? (int)reader["RecordId"] : (int?)null,
                OldValue = reader["OldValue"]?.ToString() ?? string.Empty,
                NewValue = reader["NewValue"]?.ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString() ?? string.Empty,
                IpAddress = reader["IpAddress"]?.ToString() ?? string.Empty,
                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString())
            };
        }

        private void AddParameters(SQLiteCommand cmd, Log entity)
        {
            cmd.Parameters.AddWithValue("@userId", entity.UserId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@action", entity.Action ?? string.Empty);
            cmd.Parameters.AddWithValue("@tableName", entity.TableName ?? string.Empty);
            cmd.Parameters.AddWithValue("@recordId", entity.RecordId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@oldValue", entity.OldValue ?? string.Empty);
            cmd.Parameters.AddWithValue("@newValue", entity.NewValue ?? string.Empty);
            cmd.Parameters.AddWithValue("@description", entity.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("@ipAddress", entity.IpAddress ?? string.Empty);
            cmd.Parameters.AddWithValue("@createdDate", entity.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}