using System;
using System.Collections.Generic;
using System.Data.SQLite;
using CRMS.Entity.Models;

namespace CRMS.DataAccess.Repositories
{
    public class CustomerRepository : IRepository<Customer>
    {
        private readonly string _connectionString;

        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Customer GetById(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Customers WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapToCustomer(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving customer: {ex.Message}", ex);
            }
            return null;
        }

        public IEnumerable<Customer> GetAll()
        {
            var customers = new List<Customer>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Customers ORDER BY Id DESC", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                customers.Add(MapToCustomer(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving customers: {ex.Message}", ex);
            }
            return customers;
        }

        public IEnumerable<Customer> Find(System.Linq.Expressions.Expression<Func<Customer, bool>> predicate)
        {
            return GetAll().AsEnumerable().Where(predicate.Compile());
        }

        public IEnumerable<Customer> SearchCustomers(string searchTerm)
        {
            var customers = new List<Customer>();
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"SELECT * FROM Customers WHERE 
                               FirstName LIKE @search OR 
                               LastName LIKE @search OR 
                               CompanyName LIKE @search OR 
                               Email LIKE @search OR 
                               PhoneNumber LIKE @search OR 
                               CustomerCode LIKE @search 
                               ORDER BY FirstName ASC";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@search", $"%{searchTerm}%");
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                customers.Add(MapToCustomer(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching customers: {ex.Message}", ex);
            }
            return customers;
        }

        public void Add(Customer entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"INSERT INTO Customers (CustomerCode, FirstName, LastName, CompanyName, TaxNumber, 
                               Email, PhoneNumber, Address, City, Country, Website, CustomerType, Status, 
                               Rating, Notes, LastContactDate, NextContactDate, CreatedDate, CreatedByUserId) 
                               VALUES (@customerCode, @firstName, @lastName, @companyName, @taxNumber, 
                               @email, @phoneNumber, @address, @city, @country, @website, @customerType, 
                               @status, @rating, @notes, @lastContactDate, @nextContactDate, @createdDate, @createdByUserId)";
                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        AddParameters(cmd, entity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding customer: {ex.Message}", ex);
            }
        }

        public void Update(Customer entity)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    var sql = @"UPDATE Customers SET FirstName=@firstName, LastName=@lastName, CompanyName=@companyName, 
                               TaxNumber=@taxNumber, Email=@email, PhoneNumber=@phoneNumber, Address=@address, 
                               City=@city, Country=@country, Website=@website, CustomerType=@customerType, 
                               Status=@status, Rating=@rating, Notes=@notes, LastContactDate=@lastContactDate, 
                               NextContactDate=@nextContactDate, ModifiedDate=@modifiedDate, ModifiedByUserId=@modifiedByUserId 
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
                throw new Exception($"Error updating customer: {ex.Message}", ex);
            }
        }

        public void Delete(Customer entity)
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
                    using (var cmd = new SQLiteCommand("DELETE FROM Customers WHERE Id=@id", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting customer: {ex.Message}", ex);
            }
        }

        public int SaveChanges()
        {
            return 1;
        }

        private Customer MapToCustomer(SQLiteDataReader reader)
        {
            return new Customer
            {
                Id = (int)reader["Id"],
                CustomerCode = reader["CustomerCode"].ToString(),
                FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                LastName = reader["LastName"]?.ToString() ?? string.Empty,
                CompanyName = reader["CompanyName"]?.ToString() ?? string.Empty,
                TaxNumber = reader["TaxNumber"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString() ?? string.Empty,
                PhoneNumber = reader["PhoneNumber"]?.ToString() ?? string.Empty,
                Address = reader["Address"]?.ToString() ?? string.Empty,
                City = reader["City"]?.ToString() ?? string.Empty,
                Country = reader["Country"]?.ToString() ?? string.Empty,
                Website = reader["Website"]?.ToString() ?? string.Empty,
                CustomerType = reader["CustomerType"]?.ToString() ?? string.Empty,
                Status = reader["Status"]?.ToString() ?? "Active",
                Rating = (double)reader["Rating"],
                Notes = reader["Notes"]?.ToString() ?? string.Empty,
                LastContactDate = reader["LastContactDate"] != DBNull.Value ? DateTime.Parse(reader["LastContactDate"].ToString()) : (DateTime?)null,
                NextContactDate = reader["NextContactDate"] != DBNull.Value ? DateTime.Parse(reader["NextContactDate"].ToString()) : (DateTime?)null,
                CreatedDate = DateTime.Parse(reader["CreatedDate"].ToString()),
                ModifiedDate = reader["ModifiedDate"] != DBNull.Value ? DateTime.Parse(reader["ModifiedDate"].ToString()) : (DateTime?)null,
                CreatedByUserId = reader["CreatedByUserId"] != DBNull.Value ? (int)reader["CreatedByUserId"] : (int?)null,
                ModifiedByUserId = reader["ModifiedByUserId"] != DBNull.Value ? (int)reader["ModifiedByUserId"] : (int?)null
            };
        }

        private void AddParameters(SQLiteCommand cmd, Customer entity)
        {
            cmd.Parameters.AddWithValue("@customerCode", entity.CustomerCode ?? string.Empty);
            cmd.Parameters.AddWithValue("@firstName", entity.FirstName ?? string.Empty);
            cmd.Parameters.AddWithValue("@lastName", entity.LastName ?? string.Empty);
            cmd.Parameters.AddWithValue("@companyName", entity.CompanyName ?? string.Empty);
            cmd.Parameters.AddWithValue("@taxNumber", entity.TaxNumber ?? string.Empty);
            cmd.Parameters.AddWithValue("@email", entity.Email ?? string.Empty);
            cmd.Parameters.AddWithValue("@phoneNumber", entity.PhoneNumber ?? string.Empty);
            cmd.Parameters.AddWithValue("@address", entity.Address ?? string.Empty);
            cmd.Parameters.AddWithValue("@city", entity.City ?? string.Empty);
            cmd.Parameters.AddWithValue("@country", entity.Country ?? string.Empty);
            cmd.Parameters.AddWithValue("@website", entity.Website ?? string.Empty);
            cmd.Parameters.AddWithValue("@customerType", entity.CustomerType ?? string.Empty);
            cmd.Parameters.AddWithValue("@status", entity.Status ?? "Active");
            cmd.Parameters.AddWithValue("@rating", entity.Rating);
            cmd.Parameters.AddWithValue("@notes", entity.Notes ?? string.Empty);
            cmd.Parameters.AddWithValue("@lastContactDate", entity.LastContactDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@nextContactDate", entity.NextContactDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@modifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@createdDate", entity.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@createdByUserId", entity.CreatedByUserId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@modifiedByUserId", entity.ModifiedByUserId ?? (object)DBNull.Value);
        }
    }
}