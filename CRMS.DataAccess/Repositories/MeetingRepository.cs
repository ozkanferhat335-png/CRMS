using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using CRMS.Entity.Models;

namespace CRMS.DataAccess.Repositories
{
    public class MeetingRepository : IRepository<Meeting>
    {
        private readonly string _connectionString;

        public MeetingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Meeting GetById(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Meetings WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return MapToMeeting(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving meeting: {ex.Message}", ex);
            }
            return null;
        }

        public IEnumerable<Meeting> GetAll()
        {
            var meetings = new List<Meeting>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Meetings ORDER BY MeetingDate DESC", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                meetings.Add(MapToMeeting(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving meetings: {ex.Message}", ex);
            }
            return meetings;
        }

        public IEnumerable<Meeting> Find(System.Linq.Expressions.Expression<Func<Meeting, bool>> predicate)
        {
            return GetAll().AsEnumerable().Where(predicate.Compile());
        }

        public IEnumerable<Meeting> GetByCustomerId(int customerId)
        {
            var meetings = new List<Meeting>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Meetings WHERE CustomerId=@customerId ORDER BY MeetingDate DESC", connection))
                    {
                        cmd.Parameters.AddWithValue("@customerId", customerId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                meetings.Add(MapToMeeting(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving meetings by customer: {ex.Message}", ex);
            }
            return meetings;
        }

        public IEnumerable<Meeting> GetUpcomingMeetings(int userId)
        {
            var meetings = new List<Meeting>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = "SELECT * FROM Meetings WHERE UserId=@userId AND MeetingDate >= @now AND Status='Planned' ORDER BY MeetingDate ASC";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                meetings.Add(MapToMeeting(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving upcoming meetings: {ex.Message}", ex);
            }
            return meetings;
        }

        public void Add(Meeting entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO Meetings (CustomerId, UserId, Title, Description, MeetingDate, EndDate, 
                               MeetingType, Result, Notes, Status, CreatedDate, CreatedByUserId) 
                               VALUES (@customerId, @userId, @title, @description, @meetingDate, @endDate, 
                               @meetingType, @result, @notes, @status, @createdDate, @createdByUserId)";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding meeting: {ex.Message}", ex);
            }
        }

        public void Update(Meeting entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"UPDATE Meetings SET CustomerId=@customerId, UserId=@userId, Title=@title, 
                               Description=@description, MeetingDate=@meetingDate, EndDate=@endDate, 
                               MeetingType=@meetingType, Result=@result, Notes=@notes, Status=@status, 
                               ModifiedDate=@modifiedDate WHERE Id=@id";
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
                throw new Exception($"Error updating meeting: {ex.Message}", ex);
            }
        }

        public void Delete(Meeting entity)
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
                    using (var cmd = new SQLiteCommand("DELETE FROM Meetings WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting meeting: {ex.Message}", ex);
            }
        }

        public int SaveChanges()
        {
            return 1;
        }

        private Meeting MapToMeeting(SQLiteDataReader reader)
        {
            return new Meeting
            {
                Id = (int)reader["Id"],
                CustomerId = (int)reader["CustomerId"],
                UserId = (int)reader["UserId"],
                Title = reader["Title"].ToString(),
                Description = reader["Description"]?.ToString() ?? string.Empty,
                MeetingDate = DateTime.Parse(reader["MeetingDate"].ToString()),
                EndDate = reader["EndDate"] != DBNull.Value ? DateTime.Parse(reader["EndDate"].ToString()) : (DateTime?)null,
                MeetingType = reader["MeetingType"]?.ToString() ?? string.Empty,
                Result = reader["Result"]?.ToString() ?? string.Empty,
                Notes = reader["Notes"]?.ToString() ?? string.Empty,
                Status = reader["Status"]?.ToString() ?? "Planned",
                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString()),
                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? DateTime.Parse(reader["ModifiedDate"].ToString()) : (DateTime?)null,
                CreatedByUserId = reader["CreatedByUserId"] != DBNull.Value ? (int)reader["CreatedByUserId"] : (int?)null
            };
        }

        private void AddParameters(SQLiteCommand cmd, Meeting entity)
        {
            cmd.Parameters.AddWithValue("@customerId", entity.CustomerId);
            cmd.Parameters.AddWithValue("@userId", entity.UserId);
            cmd.Parameters.AddWithValue("@title", entity.Title ?? string.Empty);
            cmd.Parameters.AddWithValue("@description", entity.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("@meetingDate", entity.MeetingDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@endDate", entity.EndDate.HasValue ? (object)entity.EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value);
            cmd.Parameters.AddWithValue("@meetingType", entity.MeetingType ?? string.Empty);
            cmd.Parameters.AddWithValue("@result", entity.Result ?? string.Empty);
            cmd.Parameters.AddWithValue("@notes", entity.Notes ?? string.Empty);
            cmd.Parameters.AddWithValue("@status", entity.Status ?? "Planned");
            cmd.Parameters.AddWithValue("@createdDate", entity.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@createdByUserId", entity.CreatedByUserId.HasValue ? (object)entity.CreatedByUserId.Value : DBNull.Value);
        }
    }
}
