using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using CRMS.Entity.Models;

namespace CRMS.DataAccess.Repositories
{
    public class SettingsRepository : IRepository<SystemSettings>
    {
        private readonly string _connectionString;

        public SettingsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SystemSettings GetById(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM SystemSettings WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return MapToSettings(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving setting: {ex.Message}", ex);
            }
            return null;
        }

        public SystemSettings GetByKey(string key)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM SystemSettings WHERE SettingKey=@key", connection))
                    {
                        cmd.Parameters.AddWithValue("@key", key);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return MapToSettings(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving setting by key: {ex.Message}", ex);
            }
            return null;
        }

        public IEnumerable<SystemSettings> GetAll()
        {
            var settings = new List<SystemSettings>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM SystemSettings ORDER BY SettingKey", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                settings.Add(MapToSettings(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving settings: {ex.Message}", ex);
            }
            return settings;
        }

        public IEnumerable<SystemSettings> Find(System.Linq.Expressions.Expression<Func<SystemSettings, bool>> predicate)
        {
            return GetAll().AsEnumerable().Where(predicate.Compile());
        }

        public void Add(SystemSettings entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO SystemSettings (SettingKey, SettingValue, Description, SettingType, CreatedDate) 
                               VALUES (@settingKey, @settingValue, @description, @settingType, @createdDate)";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding setting: {ex.Message}", ex);
            }
        }

        public void Update(SystemSettings entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"UPDATE SystemSettings SET SettingValue=@settingValue, Description=@description, 
                               SettingType=@settingType, ModifiedDate=@modifiedDate, ModifiedByUserId=@modifiedByUserId 
                               WHERE Id=@id";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@modifiedByUserId", entity.ModifiedByUserId.HasValue ? (object)entity.ModifiedByUserId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@id", entity.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating setting: {ex.Message}", ex);
            }
        }

        public void UpsertByKey(string key, string value, int? modifiedByUserId = null)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO SystemSettings (SettingKey, SettingValue, CreatedDate, ModifiedDate, ModifiedByUserId) 
                               VALUES (@key, @value, @now, @now, @userId)
                               ON CONFLICT(SettingKey) DO UPDATE SET SettingValue=@value, ModifiedDate=@now, ModifiedByUserId=@userId";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@key", key);
                        cmd.Parameters.AddWithValue("@value", value ?? string.Empty);
                        cmd.Parameters.AddWithValue("@now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@userId", modifiedByUserId.HasValue ? (object)modifiedByUserId.Value : DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error upserting setting: {ex.Message}", ex);
            }
        }

        public void Delete(SystemSettings entity)
        {
            DeleteById(entity.Id);
        }

        public void DeleteById(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("DELETE FROM SystemSettings WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting setting: {ex.Message}", ex);
            }
        }

        public int SaveChanges()
        {
            return 1;
        }

        private SystemSettings MapToSettings(SQLiteDataReader reader)
        {
            return new SystemSettings
            {
                Id = (int)reader["Id"],
                SettingKey = reader["SettingKey"].ToString(),
                SettingValue = reader["SettingValue"]?.ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString() ?? string.Empty,
                SettingType = reader["SettingType"]?.ToString() ?? "String",
                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString()),
                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? DateTime.Parse(reader["ModifiedDate"].ToString()) : (DateTime?)null,
                ModifiedByUserId = reader["ModifiedByUserId"] != DBNull.Value ? (int)reader["ModifiedByUserId"] : (int?)null
            };
        }

        private void AddParameters(SQLiteCommand cmd, SystemSettings entity)
        {
            cmd.Parameters.AddWithValue("@settingKey", entity.SettingKey ?? string.Empty);
            cmd.Parameters.AddWithValue("@settingValue", entity.SettingValue ?? string.Empty);
            cmd.Parameters.AddWithValue("@description", entity.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("@settingType", entity.SettingType ?? "String");
            cmd.Parameters.AddWithValue("@createdDate", entity.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
