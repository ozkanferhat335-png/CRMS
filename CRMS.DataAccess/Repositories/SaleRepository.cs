using System;
using System.Collections.Generic;
using System.Data.SQLite;
using CRMS.Entity.Models;

namespace CRMS.DataAccess.Repositories
{
    public class SaleRepository : IRepository<Sale>
    {
        private readonly string _connectionString;

        public SaleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Sale GetById(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Sales WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapToSale(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sale: {ex.Message}", ex);
            }
            return null;
        }

        public IEnumerable<Sale> GetAll()
        {
            var sales = new List<Sale>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Sales ORDER BY SaleDate DESC", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sales.Add(MapToSale(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sales: {ex.Message}", ex);
            }
            return sales;
        }

        public IEnumerable<Sale> Find(System.Linq.Expressions.Expression<Func<Sale, bool>> predicate)
        {
            return GetAll().AsEnumerable().Where(predicate.Compile());
        }

        public decimal GetTotalSalesAmount()
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT SUM(FinalAmount) FROM Sales WHERE Status='Won'", connection))
                    {
                        var result = cmd.ExecuteScalar();
                        return result != DBNull.Value && result != null ? Convert.ToDecimal(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating total sales: {ex.Message}", ex);
            }
        }

        public void Add(Sale entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO Sales (SaleCode, CustomerId, UserId, OfferId, Status, Amount, 
                               TaxAmount, FinalAmount, SaleDate, Notes, CreatedDate, CreatedByUserId) 
                               VALUES (@saleCode, @customerId, @userId, @offerId, @status, @amount, 
                               @taxAmount, @finalAmount, @saleDate, @notes, @createdDate, @createdByUserId)";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding sale: {ex.Message}", ex);
            }
        }

        public void Update(Sale entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"UPDATE Sales SET CustomerId=@customerId, UserId=@userId, OfferId=@offerId, 
                               Status=@status, Amount=@amount, TaxAmount=@taxAmount, FinalAmount=@finalAmount, 
                               ClosingDate=@closingDate, Notes=@notes, ModifiedDate=@modifiedDate WHERE Id=@id";
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
                throw new Exception($"Error updating sale: {ex.Message}", ex);
            }
        }

        public void Delete(Sale entity)
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
                    using (var cmd = new SQLiteCommand("DELETE FROM Sales WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting sale: {ex.Message}", ex);
            }
        }

        public int SaveChanges()
        {
            return 1;
        }

        private Sale MapToSale(SQLiteDataReader reader)
        {
            return new Sale
            {
                Id = (int)reader["Id"],
                SaleCode = reader["SaleCode"].ToString(),
                CustomerId = (int)reader["CustomerId"],
                UserId = (int)reader["UserId"],
                OfferId = reader["OfferId"] != DBNull.Value ? (int)reader["OfferId"] : (int?)null,
                Status = reader["Status"]?.ToString() ?? "Prospect",
                Amount = (decimal)(double)reader["Amount"],
                TaxAmount = (decimal)(double)reader["TaxAmount"],
                FinalAmount = (decimal)(double)reader["FinalAmount"],
                SaleDate = DateTime.Parse(reader["SaleDate"].ToString()),
                ClosingDate = reader["ClosingDate"] != DBNull.Value ? DateTime.Parse(reader["ClosingDate"].ToString()) : (DateTime?)null,
                Notes = reader["Notes"]?.ToString() ?? string.Empty,
                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString()),
                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? DateTime.Parse(reader["ModifiedDate"].ToString()) : (DateTime?)null,
                CreatedByUserId = reader["CreatedByUserId"] != DBNull.Value ? (int)reader["CreatedByUserId"] : (int?)null
            };
        }

        private void AddParameters(SQLiteCommand cmd, Sale entity)
        {
            cmd.Parameters.AddWithValue("@saleCode", entity.SaleCode ?? string.Empty);
            cmd.Parameters.AddWithValue("@customerId", entity.CustomerId);
            cmd.Parameters.AddWithValue("@userId", entity.UserId);
            cmd.Parameters.AddWithValue("@offerId", entity.OfferId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@status", entity.Status ?? "Prospect");
            cmd.Parameters.AddWithValue("@amount", entity.Amount);
            cmd.Parameters.AddWithValue("@taxAmount", entity.TaxAmount);
            cmd.Parameters.AddWithValue("@finalAmount", entity.FinalAmount);
            cmd.Parameters.AddWithValue("@saleDate", entity.SaleDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@closingDate", entity.ClosingDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@notes", entity.Notes ?? string.Empty);
            cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@createdDate", entity.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@createdByUserId", entity.CreatedByUserId ?? (object)DBNull.Value);
        }
    }
}