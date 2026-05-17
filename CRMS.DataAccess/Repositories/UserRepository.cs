using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using CRMS.Entity.Models;

namespace CRMS.DataAccess.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User GetById(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"SELECT u.*, r.RoleName FROM Users u 
                               LEFT JOIN Roles r ON u.RoleId = r.Id 
                               WHERE u.Id = @id";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapToUser(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user: {ex.Message}", ex);
            }
            return null;
        }

        public IEnumerable<User> GetAll()
        {
            var users = new List<User>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"SELECT u.*, r.RoleName FROM Users u 
                               LEFT JOIN Roles r ON u.RoleId = r.Id 
                               ORDER BY u.Id";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(MapToUser(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving users: {ex.Message}", ex);
            }
            return users;
        }

        public IEnumerable<User> Find(System.Linq.Expressions.Expression<Func<User, bool>> predicate)
        {
            // For simple queries, we'll implement basic filtering
            // For complex LINQ queries, you'd need to translate to SQL
            return GetAll().AsEnumerable().Where(predicate.Compile());
        }

        public User GetByUsername(string username)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"SELECT u.*, r.RoleName FROM Users u 
                               LEFT JOIN Roles r ON u.RoleId = r.Id 
                               WHERE u.Username = @username";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapToUser(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user by username: {ex.Message}", ex);
            }
            return null;
        }

        public void Add(User entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO Users (Username, Email, PasswordHash, RoleId, FirstName, LastName, 
                               PhoneNumber, IsActive, IsLocked, FailedLoginAttempts, CreatedDate, CreatedByUserId) 
                               VALUES (@username, @email, @passwordHash, @roleId, @firstName, @lastName, 
                               @phoneNumber, @isActive, @isLocked, @failedLoginAttempts, @createdDate, @createdByUserId)";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding user: {ex.Message}", ex);
            }
        }

        public void Update(User entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"UPDATE Users SET Username=@username, Email=@email, PasswordHash=@passwordHash, 
                               RoleId=@roleId, FirstName=@firstName, LastName=@lastName, PhoneNumber=@phoneNumber, 
                               IsActive=@isActive, IsLocked=@isLocked, FailedLoginAttempts=@failedLoginAttempts, 
                               LastLoginDate=@lastLoginDate, ModifiedDate=@modifiedDate, ModifiedByUserId=@modifiedByUserId 
                               WHERE Id=@id";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.Parameters.AddWithValue("@id", entity.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}", ex);
            }
        }

        public void Delete(User entity)
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
                    using (var cmd = new SQLiteCommand("DELETE FROM Users WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user: {ex.Message}", ex);
            }
        }

        public int SaveChanges()
        {
            // In this implementation, changes are saved immediately
            return 1;
        }

        private User MapToUser(SQLiteDataReader reader)
        {
            return new User
            {
                Id = (int)reader["Id"],
                Username = reader["Username"].ToString(),
                Email = reader["Email"].ToString(),
                PasswordHash = reader["PasswordHash"].ToString(),
                RoleId = (int)reader["RoleId"],
                FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                LastName = reader["LastName"]?.ToString() ?? string.Empty,
                PhoneNumber = reader["PhoneNumber"]?.ToString() ?? string.Empty,
                IsActive = (long)reader["IsActive"] == 1,
                IsLocked = (long)reader["IsLocked"] == 1,
                FailedLoginAttempts = (int)(long)reader["FailedLoginAttempts"],
                LastLoginDate = reader["LastLoginDate"] != DBNull.Value ? DateTime.Parse(reader["LastLoginDate"].ToString()) : (DateTime?)null,
                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString()),
                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? DateTime.Parse(reader["ModifiedDate"].ToString()) : (DateTime?)null,
                CreatedByUserId = reader["CreatedByUserId"] != DBNull.Value ? (int)reader["CreatedByUserId"] : (int?)null,
                ModifiedByUserId = reader["ModifiedByUserId"] != DBNull.Value ? (int)reader["ModifiedByUserId"] : (int?)null
            };
        }

        private void AddParameters(SQLiteCommand cmd, User entity)
        {
            cmd.Parameters.AddWithValue("@username", entity.Username ?? string.Empty);
            cmd.Parameters.AddWithValue("@email", entity.Email ?? string.Empty);
            cmd.Parameters.AddWithValue("@passwordHash", entity.PasswordHash ?? string.Empty);
            cmd.Parameters.AddWithValue("@roleId", entity.RoleId);
            cmd.Parameters.AddWithValue("@firstName", entity.FirstName ?? string.Empty);
            cmd.Parameters.AddWithValue("@lastName", entity.LastName ?? string.Empty);
            cmd.Parameters.AddWithValue("@phoneNumber", entity.PhoneNumber ?? string.Empty);
            cmd.Parameters.AddWithValue("@isActive", entity.IsActive ? 1 : 0);
            cmd.Parameters.AddWithValue("@isLocked", entity.IsLocked ? 1 : 0);
            cmd.Parameters.AddWithValue("@failedLoginAttempts", entity.FailedLoginAttempts);
            cmd.Parameters.AddWithValue("@lastLoginDate", entity.LastLoginDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@modifiedDate", entity.ModifiedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@createdDate", entity.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@createdByUserId", entity.CreatedByUserId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@modifiedByUserId", entity.ModifiedByUserId ?? (object)DBNull.Value);
        }
    }
}