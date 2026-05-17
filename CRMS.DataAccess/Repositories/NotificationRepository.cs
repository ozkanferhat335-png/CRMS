using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using CRMS.Entity.Models;

namespace CRMS.DataAccess.Repositories
{
    public class NotificationRepository : IRepository<Notification>
    {
        private readonly string _connectionString;

        public NotificationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Notification GetById(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Notifications WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return MapToNotification(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving notification: {ex.Message}", ex);
            }
            return null;
        }

        public IEnumerable<Notification> GetAll()
        {
            var notifications = new List<Notification>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Notifications ORDER BY CreatedDate DESC", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                notifications.Add(MapToNotification(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving notifications: {ex.Message}", ex);
            }
            return notifications;
        }

        public IEnumerable<Notification> Find(System.Linq.Expressions.Expression<Func<Notification, bool>> predicate)
        {
            return GetAll().AsEnumerable().Where(predicate.Compile());
        }

        public IEnumerable<Notification> GetUnreadByUser(int userId)
        {
            var notifications = new List<Notification>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = "SELECT * FROM Notifications WHERE UserId=@userId AND Status='Unread' ORDER BY CreatedDate DESC";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                notifications.Add(MapToNotification(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving unread notifications: {ex.Message}", ex);
            }
            return notifications;
        }

        public int GetUnreadCount(int userId)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Notifications WHERE UserId=@userId AND Status='Unread'", connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public void MarkAsRead(int notificationId)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = "UPDATE Notifications SET Status='Read', ReadDate=@readDate WHERE Id=@id";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@readDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@id", notificationId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error marking notification as read: {ex.Message}", ex);
            }
        }

        public void MarkAllAsRead(int userId)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = "UPDATE Notifications SET Status='Read', ReadDate=@readDate WHERE UserId=@userId AND Status='Unread'";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@readDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error marking all notifications as read: {ex.Message}", ex);
            }
        }

        public void Add(Notification entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO Notifications (UserId, Title, Message, NotificationType, Status, 
                               RelatedEntityId, RelatedEntityType, CreatedDate) 
                               VALUES (@userId, @title, @message, @notificationType, @status, 
                               @relatedEntityId, @relatedEntityType, @createdDate)";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding notification: {ex.Message}", ex);
            }
        }

        public void Update(Notification entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = "UPDATE Notifications SET Status=@status, ReadDate=@readDate WHERE Id=@id";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@status", entity.Status ?? "Unread");
                        cmd.Parameters.AddWithValue("@readDate", entity.ReadDate.HasValue ? (object)entity.ReadDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value);
                        cmd.Parameters.AddWithValue("@id", entity.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating notification: {ex.Message}", ex);
            }
        }

        public void Delete(Notification entity)
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
                    using (var cmd = new SQLiteCommand("DELETE FROM Notifications WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting notification: {ex.Message}", ex);
            }
        }

        public int SaveChanges()
        {
            return 1;
        }

        private Notification MapToNotification(SQLiteDataReader reader)
        {
            return new Notification
            {
                Id = (int)reader["Id"],
                UserId = (int)reader["UserId"],
                Title = reader["Title"].ToString(),
                Message = reader["Message"]?.ToString() ?? string.Empty,
                NotificationType = reader["NotificationType"]?.ToString() ?? string.Empty,
                Status = reader["Status"]?.ToString() ?? "Unread",
                RelatedEntityId = reader["RelatedEntityId"] != DBNull.Value ? (int)reader["RelatedEntityId"] : (int?)null,
                RelatedEntityType = reader["RelatedEntityType"]?.ToString() ?? string.Empty,
                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString()),
                ReadDate = reader["ReadDate"] != DBNull.Value ? DateTime.Parse(reader["ReadDate"].ToString()) : (DateTime?)null
            };
        }

        private void AddParameters(SQLiteCommand cmd, Notification entity)
        {
            cmd.Parameters.AddWithValue("@userId", entity.UserId);
            cmd.Parameters.AddWithValue("@title", entity.Title ?? string.Empty);
            cmd.Parameters.AddWithValue("@message", entity.Message ?? string.Empty);
            cmd.Parameters.AddWithValue("@notificationType", entity.NotificationType ?? string.Empty);
            cmd.Parameters.AddWithValue("@status", entity.Status ?? "Unread");
            cmd.Parameters.AddWithValue("@relatedEntityId", entity.RelatedEntityId.HasValue ? (object)entity.RelatedEntityId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@relatedEntityType", entity.RelatedEntityType ?? string.Empty);
            cmd.Parameters.AddWithValue("@createdDate", entity.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
