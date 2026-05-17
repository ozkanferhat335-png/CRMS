using System;
using System.Collections.Generic;
using System.Data.SQLite;
using CRMS.Entity.Models;

namespace CRMS.DataAccess.Repositories
{
    public class TaskRepository : IRepository<Task>
    {
        private readonly string _connectionString;

        public TaskRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task GetById(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Tasks WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapToTask(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving task: {ex.Message}", ex);
            }
            return null;
        }

        public IEnumerable<Task> GetAll()
        {
            var tasks = new List<Task>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Tasks ORDER BY DueDate ASC", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tasks.Add(MapToTask(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving tasks: {ex.Message}", ex);
            }
            return tasks;
        }

        public IEnumerable<Task> Find(System.Linq.Expressions.Expression<Func<Task, bool>> predicate)
        {
            return GetAll().AsEnumerable().Where(predicate.Compile());
        }

        public IEnumerable<Task> GetPendingTasks()
        {
            var tasks = new List<Task>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = "SELECT * FROM Tasks WHERE Status IN ('Pending', 'InProgress') ORDER BY DueDate ASC";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tasks.Add(MapToTask(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving pending tasks: {ex.Message}", ex);
            }
            return tasks;
        }

        public void Add(Task entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO Tasks (Title, Description, AssignedToUserId, CustomerId, Priority, 
                               Status, DueDate, AssignedByUserId, CreatedDate) 
                               VALUES (@title, @description, @assignedToUserId, @customerId, @priority, 
                               @status, @dueDate, @assignedByUserId, @createdDate)";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding task: {ex.Message}", ex);
            }
        }

        public void Update(Task entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"UPDATE Tasks SET Title=@title, Description=@description, AssignedToUserId=@assignedToUserId, 
                               CustomerId=@customerId, Priority=@priority, Status=@status, DueDate=@dueDate, 
                               CompletedDate=@completedDate, ModifiedDate=@modifiedDate WHERE Id=@id";
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
                throw new Exception($"Error updating task: {ex.Message}", ex);
            }
        }

        public void Delete(Task entity)
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
                    using (var cmd = new SQLiteCommand("DELETE FROM Tasks WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting task: {ex.Message}", ex);
            }
        }

        public int SaveChanges()
        {
            return 1;
        }

        private Task MapToTask(SQLiteDataReader reader)
        {
            return new Task
            {
                Id = (int)reader["Id"],
                Title = reader["Title"].ToString(),
                Description = reader["Description"]?.ToString() ?? string.Empty,
                AssignedToUserId = (int)reader["AssignedToUserId"],
                CustomerId = reader["CustomerId"] != DBNull.Value ? (int)reader["CustomerId"] : (int?)null,
                Priority = reader["Priority"]?.ToString() ?? "Medium",
                Status = reader["Status"]?.ToString() ?? "Pending",
                DueDate = DateTime.Parse(reader["DueDate"].ToString()),
                CompletedDate = reader["CompletedDate"] != DBNull.Value ? DateTime.Parse(reader["CompletedDate"].ToString()) : (DateTime?)null,
                AssignedByUserId = reader["AssignedByUserId"] != DBNull.Value ? (int)reader["AssignedByUserId"] : (int?)null,
                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString()),
                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? DateTime.Parse(reader["ModifiedDate"].ToString()) : (DateTime?)null
            };
        }

        private void AddParameters(SQLiteCommand cmd, Task entity)
        {
            cmd.Parameters.AddWithValue("@title", entity.Title ?? string.Empty);
            cmd.Parameters.AddWithValue("@description", entity.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("@assignedToUserId", entity.AssignedToUserId);
            cmd.Parameters.AddWithValue("@customerId", entity.CustomerId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@priority", entity.Priority ?? "Medium");
            cmd.Parameters.AddWithValue("@status", entity.Status ?? "Pending");
            cmd.Parameters.AddWithValue("@dueDate", entity.DueDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@completedDate", entity.CompletedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@assignedByUserId", entity.AssignedByUserId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@createdDate", entity.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}