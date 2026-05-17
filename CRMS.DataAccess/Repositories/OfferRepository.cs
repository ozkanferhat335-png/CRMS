using System;
using System.Collections.Generic;
using System.Data.SQLite;
using CRMS.Entity.Models;

namespace CRMS.DataAccess.Repositories
{
    public class OfferRepository : IRepository<Offer>
    {
        private readonly string _connectionString;

        public OfferRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Offer GetById(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Offers WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapToOffer(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving offer: {ex.Message}", ex);
            }
            return null;
        }

        public IEnumerable<Offer> GetAll()
        {
            var offers = new List<Offer>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Offers ORDER BY OfferDate DESC", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                offers.Add(MapToOffer(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving offers: {ex.Message}", ex);
            }
            return offers;
        }

        public IEnumerable<Offer> Find(System.Linq.Expressions.Expression<Func<Offer, bool>> predicate)
        {
            return GetAll().AsEnumerable().Where(predicate.Compile());
        }

        public void Add(Offer entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO Offers (OfferCode, CustomerId, UserId, Title, Description, 
                               TotalAmount, TaxAmount, DiscountAmount, FinalAmount, Status, OfferDate, 
                               ExpiryDate, Notes, CreatedDate, CreatedByUserId) 
                               VALUES (@offerCode, @customerId, @userId, @title, @description, 
                               @totalAmount, @taxAmount, @discountAmount, @finalAmount, @status, @offerDate, 
                               @expiryDate, @notes, @createdDate, @createdByUserId)";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding offer: {ex.Message}", ex);
            }
        }

        public void Update(Offer entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"UPDATE Offers SET CustomerId=@customerId, UserId=@userId, Title=@title, 
                               Description=@description, TotalAmount=@totalAmount, TaxAmount=@taxAmount, 
                               DiscountAmount=@discountAmount, FinalAmount=@finalAmount, Status=@status, 
                               ExpiryDate=@expiryDate, ApprovalDate=@approvalDate, Notes=@notes, 
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
                throw new Exception($"Error updating offer: {ex.Message}", ex);
            }
        }

        public void Delete(Offer entity)
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
                    using (var cmd = new SQLiteCommand("DELETE FROM Offers WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting offer: {ex.Message}", ex);
            }
        }

        public int SaveChanges()
        {
            return 1;
        }

        private Offer MapToOffer(SQLiteDataReader reader)
        {
            return new Offer
            {
                Id = (int)reader["Id"],
                OfferCode = reader["OfferCode"].ToString(),
                CustomerId = (int)reader["CustomerId"],
                UserId = (int)reader["UserId"],
                Title = reader["Title"].ToString(),
                Description = reader["Description"]?.ToString() ?? string.Empty,
                TotalAmount = (decimal)(double)reader["TotalAmount"],
                TaxAmount = (decimal)(double)reader["TaxAmount"],
                DiscountAmount = (decimal)(double)reader["DiscountAmount"],
                FinalAmount = (decimal)(double)reader["FinalAmount"],
                Status = reader["Status"]?.ToString() ?? "Pending",
                OfferDate = DateTime.Parse(reader["OfferDate"].ToString()),
                ExpiryDate = reader["ExpiryDate"] != DBNull.Value ? DateTime.Parse(reader["ExpiryDate"].ToString()) : (DateTime?)null,
                ApprovalDate = reader["ApprovalDate"] != DBNull.Value ? DateTime.Parse(reader["ApprovalDate"].ToString()) : (DateTime?)null,
                Notes = reader["Notes"]?.ToString() ?? string.Empty,
                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString()),
                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? DateTime.Parse(reader["ModifiedDate"].ToString()) : (DateTime?)null,
                CreatedByUserId = reader["CreatedByUserId"] != DBNull.Value ? (int)reader["CreatedByUserId"] : (int?)null
            };
        }

        private void AddParameters(SQLiteCommand cmd, Offer entity)
        {
            cmd.Parameters.AddWithValue("@offerCode", entity.OfferCode ?? string.Empty);
            cmd.Parameters.AddWithValue("@customerId", entity.CustomerId);
            cmd.Parameters.AddWithValue("@userId", entity.UserId);
            cmd.Parameters.AddWithValue("@title", entity.Title ?? string.Empty);
            cmd.Parameters.AddWithValue("@description", entity.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("@totalAmount", entity.TotalAmount);
            cmd.Parameters.AddWithValue("@taxAmount", entity.TaxAmount);
            cmd.Parameters.AddWithValue("@discountAmount", entity.DiscountAmount);
            cmd.Parameters.AddWithValue("@finalAmount", entity.FinalAmount);
            cmd.Parameters.AddWithValue("@status", entity.Status ?? "Pending");
            cmd.Parameters.AddWithValue("@offerDate", entity.OfferDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@expiryDate", entity.ExpiryDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@approvalDate", entity.ApprovalDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@notes", entity.Notes ?? string.Empty);
            cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@createdDate", entity.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@createdByUserId", entity.CreatedByUserId ?? (object)DBNull.Value);
        }
    }
}