using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using CRMS.Entity.Models;

namespace CRMS.DataAccess.Repositories
{
    public class EventRepository : IRepository<Event>
    {
        private readonly string _connectionString;

        public EventRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Event GetById(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Events WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return MapToEvent(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving event: {ex.Message}", ex);
            }
            return null;
        }

        public IEnumerable<Event> GetAll()
        {
            var events = new List<Event>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Events ORDER BY StartDate DESC", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                events.Add(MapToEvent(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving events: {ex.Message}", ex);
            }
            return events;
        }

        public IEnumerable<Event> Find(System.Linq.Expressions.Expression<Func<Event, bool>> predicate)
        {
            return GetAll().AsEnumerable().Where(predicate.Compile());
        }

        public IEnumerable<Event> GetByUser(int userId)
        {
            var events = new List<Event>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Events WHERE UserId=@userId ORDER BY StartDate ASC", connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                events.Add(MapToEvent(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving events by user: {ex.Message}", ex);
            }
            return events;
        }

        public IEnumerable<Event> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var events = new List<Event>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = "SELECT * FROM Events WHERE StartDate >= @startDate AND StartDate <= @endDate ORDER BY StartDate ASC";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                events.Add(MapToEvent(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving events by date range: {ex.Message}", ex);
            }
            return events;
        }

        public void Add(Event entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO Events (UserId, Title, Description, StartDate, EndDate, EventType, 
                               Location, CustomerId, Status, CreatedDate) 
                               VALUES (@userId, @title, @description, @startDate, @endDate, @eventType, 
                               @location, @customerId, @status, @createdDate)";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding event: {ex.Message}", ex);
            }
        }

        public void Update(Event entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"UPDATE Events SET UserId=@userId, Title=@title, Description=@description, 
                               StartDate=@startDate, EndDate=@endDate, EventType=@eventType, Location=@location, 
                               CustomerId=@customerId, Status=@status, ModifiedDate=@modifiedDate WHERE Id=@id";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@id", entity.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating event: {ex.Message}", ex);
            }
        }

        public void Delete(Event entity)
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
                    using (var cmd = new SQLiteCommand("DELETE FROM Events WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting event: {ex.Message}", ex);
            }
        }

        public int SaveChanges()
        {
            return 1;
        }

        private Event MapToEvent(SQLiteDataReader reader)
        {
            return new Event
            {
                Id = (int)reader["Id"],
                UserId = (int)reader["UserId"],
                Title = reader["Title"].ToString(),
                Description = reader["Description"]?.ToString() ?? string.Empty,
                StartDate = DateTime.Parse(reader["StartDate"].ToString()),
                EndDate = reader["EndDate"] != DBNull.Value ? DateTime.Parse(reader["EndDate"].ToString()) : (DateTime?)null,
                EventType = reader["EventType"]?.ToString() ?? string.Empty,
                Location = reader["Location"]?.ToString() ?? string.Empty,
                CustomerId = reader["CustomerId"] != DBNull.Value ? (int)reader["CustomerId"] : (int?)null,
                Status = reader["Status"]?.ToString() ?? "Planned",
                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString()),
                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? DateTime.Parse(reader["ModifiedDate"].ToString()) : (DateTime?)null
            };
        }

        private void AddParameters(SQLiteCommand cmd, Event entity)
        {
            cmd.Parameters.AddWithValue("@userId", entity.UserId);
            cmd.Parameters.AddWithValue("@title", entity.Title ?? string.Empty);
            cmd.Parameters.AddWithValue("@description", entity.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("@startDate", entity.StartDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@endDate", entity.EndDate.HasValue ? (object)entity.EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value);
            cmd.Parameters.AddWithValue("@eventType", entity.EventType ?? string.Empty);
            cmd.Parameters.AddWithValue("@location", entity.Location ?? string.Empty);
            cmd.Parameters.AddWithValue("@customerId", entity.CustomerId.HasValue ? (object)entity.CustomerId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@status", entity.Status ?? "Planned");
            cmd.Parameters.AddWithValue("@createdDate", entity.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
