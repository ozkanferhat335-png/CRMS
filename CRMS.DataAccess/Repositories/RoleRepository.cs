using System;
using System.Collections.Generic;
using System.Data.SQLite;
using CRMS.Entity.Models;

namespace CRMS.DataAccess.Repositories
{
    public class RoleRepository : IRepository<Role>
    {
        private readonly string _connectionString;

        public RoleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Role GetById(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Roles WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapToRole(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving role: {ex.Message}", ex);
            }
            return null;
        }

        public IEnumerable<Role> GetAll()
        {
            var roles = new List<Role>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Roles ORDER BY Id", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                roles.Add(MapToRole(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving roles: {ex.Message}", ex);
            }
            return roles;
        }

        public IEnumerable<Role> Find(System.Linq.Expressions.Expression<Func<Role, bool>> predicate)
        {
            return GetAll().AsEnumerable().Where(predicate.Compile());
        }

        public void Add(Role entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO Roles (RoleName, Description, IsActive, CreatedDate) 
                               VALUES (@roleName, @description, @isActive, @createdDate)";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@roleName", entity.RoleName ?? string.Empty);
                        cmd.Parameters.AddWithValue("@description", entity.Description ?? string.Empty);
                        cmd.Parameters.AddWithValue("@isActive", entity.IsActive ? 1 : 0);
                        cmd.Parameters.AddWithValue("@createdDate", entity.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding role: {ex.Message}", ex);
            }
        }

        public void Update(Role entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"UPDATE Roles SET RoleName=@roleName, Description=@description, 
                               IsActive=@isActive, ModifiedDate=@modifiedDate WHERE Id=@id";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@roleName", entity.RoleName ?? string.Empty);
                        cmd.Parameters.AddWithValue("@description", entity.Description ?? string.Empty);
                        cmd.Parameters.AddWithValue("@isActive", entity.IsActive ? 1 : 0);
                        cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@id", entity.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating role: {ex.Message}", ex);
            }
        }

        public void Delete(Role entity)
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
                    using (var cmd = new SQLiteCommand("DELETE FROM Roles WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting role: {ex.Message}", ex);
            }
        }

        public int SaveChanges()
        {
            return 1;
        }

        private Role MapToRole(SQLiteDataReader reader)
        {
            return new Role
            {
                Id = (int)reader["Id"],
                RoleName = reader["RoleName"].ToString(),
                Description = reader["Description"]?.ToString() ?? string.Empty,
                IsActive = (long)reader["IsActive"] == 1,
                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString())
            };
        }
    }
}