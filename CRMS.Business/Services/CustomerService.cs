using System;
using System.Collections.Generic;
using System.Linq;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.DTOs;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class CustomerService
    {
        private readonly UnitOfWork _unitOfWork;

        public CustomerService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<CustomerDTO> GetAllCustomers()
        {
            try
            {
                return _unitOfWork.Customers.GetAll()
                    .Select(c => MapToDTO(c))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Müşterileri getirirken hata: {ex.Message}", ex);
            }
        }

        public CustomerDTO GetCustomerById(int customerId)
        {
            try
            {
                var customer = _unitOfWork.Customers.GetById(customerId);
                return customer != null ? MapToDTO(customer) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Müşteri getirirken hata: {ex.Message}", ex);
            }
        }

        public List<CustomerDTO> SearchCustomers(string searchTerm)
        {
            try
            {
                return _unitOfWork.Customers.SearchCustomers(searchTerm)
                    .Select(c => MapToDTO(c))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Müşteri araması sırasında hata: {ex.Message}", ex);
            }
        }

        public bool AddCustomer(Customer customer)
        {
            try
            {
                if (string.IsNullOrEmpty(customer.CustomerCode))
                    customer.CustomerCode = GenerateCustomerCode();

                customer.CreatedDate = DateTime.Now;
                _unitOfWork.Customers.Add(customer);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Müşteri eklenirken hata: {ex.Message}", ex);
            }
        }

        public bool UpdateCustomer(Customer customer)
        {
            try
            {
                customer.ModifiedDate = DateTime.Now;
                _unitOfWork.Customers.Update(customer);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Müşteri güncellenirken hata: {ex.Message}", ex);
            }
        }

        public bool DeleteCustomer(int customerId)
        {
            try
            {
                _unitOfWork.Customers.DeleteById(customerId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Müşteri silinirken hata: {ex.Message}", ex);
            }
        }

        public int GetTotalCustomers()
        {
            try
            {
                return _unitOfWork.Customers.GetAll().Count();
            }
            catch
            {
                return 0;
            }
        }

        public int GetActiveCustomers()
        {
            try
            {
                return _unitOfWork.Customers.Find(c => c.Status == "Active").Count();
            }
            catch
            {
                return 0;
            }
        }

        private string GenerateCustomerCode()
        {
            return $"CUST{DateTime.Now:yyyyMMddHHmmss}";
        }

        private CustomerDTO MapToDTO(Customer customer)
        {
            return new CustomerDTO
            {
                Id = customer.Id,
                CustomerCode = customer.CustomerCode,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                CompanyName = customer.CompanyName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                City = customer.City,
                Country = customer.Country,
                Status = customer.Status,
                Rating = customer.Rating,
                CreatedDate = customer.CreatedDate
            };
        }
    }
}