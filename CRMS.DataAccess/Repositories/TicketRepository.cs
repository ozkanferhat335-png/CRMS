using System;
using System.Collections.Generic;
using System.Data.SQLite;
using CRMS.Entity.Models;

namespace CRMS.DataAccess.Repositories
{
    public class TicketRepository : IRepository<Ticket>
    {
        private readonly string _connectionString;

        public TicketRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Ticket GetById(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Tickets WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapToTicket(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving ticket: {ex.Message}", ex);
            }
            return null;
        }

        public IEnumerable<Ticket> GetAll()
        {
            var tickets = new List<Ticket>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Tickets ORDER BY CreatedDate DESC", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tickets.Add(MapToTicket(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving tickets: {ex.Message}", ex);
            }
            return tickets;
        }

        public IEnumerable<Ticket> Find(System.Linq.Expressions.Expression<Func<Ticket, bool>> predicate)
        {
            return GetAll().AsEnumerable().Where(predicate.Compile());
        }

        public IEnumerable<Ticket> GetOpenTickets()
        {
            var tickets = new List<Ticket>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = "SELECT * FROM Tickets WHERE Status IN ('Open', 'InProgress') ORDER BY CreatedDate DESC";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tickets.Add(MapToTicket(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving open tickets: {ex.Message}", ex);
            }
            return tickets;
        }

        public void Add(Ticket entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO Tickets (TicketCode, CustomerId, AssignedToUserId, Title, Description, 
                               Priority, Status, Category, Department, CreatedDate, CreatedByUserId) 
                               VALUES (@ticketCode, @customerId, @assignedToUserId, @title, @description, 
                               @priority, @status, @category, @department, @createdDate, @createdByUserId)";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding ticket: {ex.Message}", ex);
            }
        }

        public void Update(Ticket entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"UPDATE Tickets SET CustomerId=@customerId, AssignedToUserId=@assignedToUserId, 
                               Title=@title, Description=@description, Priority=@priority, Status=@status, 
                               Category=@category, Department=@department, ClosedDate=@closedDate, 
                               ModifiedDate=@modifiedDate WHERE Id=@id";
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
                throw new Exception($"Error updating ticket: {ex.Message}", ex);
            }
        }

        public void Delete(Ticket entity)
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
                    using (var cmd = new SQLiteCommand("DELETE FROM Tickets WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting ticket: {ex.Message}", ex);
            }
        }

        public int SaveChanges()
        {
            return 1;
        }

        private Ticket MapToTicket(SQLiteDataReader reader)
        {
            return new Ticket
            {
                Id = (int)reader["Id"],
                TicketCode = reader["TicketCode"].ToString(),
                CustomerId = (int)reader["CustomerId"],
                AssignedToUserId = reader["AssignedToUserId"] != DBNull.Value ? (int)reader["AssignedToUserId"] : (int?)null,
                Title = reader["Title"].ToString(),
                Description = reader["Description"]?.ToString() ?? string.Empty,
                Priority = reader["Priority"]?.ToString() ?? "Medium",
                Status = reader["Status"]?.ToString() ?? "Open",
                Category = reader["Category"]?.ToString() ?? string.Empty,
                Department = reader["Department"]?.ToString() ?? string.Empty,
                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString()),
                ClosedDate = reader["ClosedDate"] != DBNull.Value ? DateTime.Parse(reader["ClosedDate"].ToString()) : (DateTime?)null,
                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? DateTime.Parse(reader["ModifiedDate"].ToString()) : (DateTime?)null,
                CreatedByUserId = reader["CreatedByUserId"] != DBNull.Value ? (int)reader["CreatedByUserId"] : (int?)null
            };
        }

        private void AddParameters(SQLiteCommand cmd, Ticket entity)
        {
            cmd.Parameters.AddWithValue("@ticketCode", entity.TicketCode ?? string.Empty);
            cmd.Parameters.AddWithValue("@customerId", entity.CustomerId);
            cmd.Parameters.AddWithValue("@assignedToUserId", entity.AssignedToUserId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@title", entity.Title ?? string.Empty);
            cmd.Parameters.AddWithValue("@description", entity.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("@priority", entity.Priority ?? "Medium");
            cmd.Parameters.AddWithValue("@status", entity.Status ?? "Open");
            cmd.Parameters.AddWithValue("@category", entity.Category ?? string.Empty);
            cmd.Parameters.AddWithValue("@department", entity.Department ?? string.Empty);
            cmd.Parameters.AddWithValue("@closedDate", entity.ClosedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@createdDate", entity.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@createdByUserId", entity.CreatedByUserId ?? (object)DBNull.Value);
        }
    }
}