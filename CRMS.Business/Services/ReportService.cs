using System;
using System.Collections.Generic;
using System.Linq;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class SalesReportItem
    {
        public string CustomerName { get; set; }
        public string SaleCode { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime SaleDate { get; set; }
    }

    public class CustomerReportItem
    {
        public string CustomerCode { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Status { get; set; }
        public string CustomerType { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public int TotalTickets { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class TaskReportItem
    {
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class TicketReportItem
    {
        public string TicketCode { get; set; }
        public string CustomerName { get; set; }
        public string Title { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
    }

    public class ReportService
    {
        private readonly UnitOfWork _unitOfWork;

        public ReportService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<SalesReportItem> GetSalesReport(DateTime? startDate = null, DateTime? endDate = null, string status = null)
        {
            try
            {
                var sales = _unitOfWork.Sales.GetAll();

                if (startDate.HasValue)
                    sales = sales.Where(s => s.SaleDate >= startDate.Value);

                if (endDate.HasValue)
                    sales = sales.Where(s => s.SaleDate <= endDate.Value);

                if (!string.IsNullOrEmpty(status))
                    sales = sales.Where(s => s.Status == status);

                var customers = _unitOfWork.Customers.GetAll().ToDictionary(c => c.Id, c => c);

                return sales.Select(s =>
                {
                    customers.TryGetValue(s.CustomerId, out var customer);
                    return new SalesReportItem
                    {
                        CustomerName = customer != null
                            ? $"{customer.FirstName} {customer.LastName}".Trim()
                            : "Bilinmiyor",
                        SaleCode = s.SaleCode,
                        Status = s.Status,
                        Amount = s.Amount,
                        FinalAmount = s.FinalAmount,
                        SaleDate = s.SaleDate
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Satış raporu oluşturulurken hata: {ex.Message}", ex);
            }
        }

        public List<CustomerReportItem> GetCustomerReport(string status = null, string customerType = null)
        {
            try
            {
                var customers = _unitOfWork.Customers.GetAll();

                if (!string.IsNullOrEmpty(status))
                    customers = customers.Where(c => c.Status == status);

                if (!string.IsNullOrEmpty(customerType))
                    customers = customers.Where(c => c.CustomerType == customerType);

                var allSales = _unitOfWork.Sales.GetAll().ToList();
                var allTickets = _unitOfWork.Tickets.GetAll().ToList();

                return customers.Select(c =>
                {
                    var customerSales = allSales.Where(s => s.CustomerId == c.Id).ToList();
                    var customerTickets = allTickets.Where(t => t.CustomerId == c.Id).ToList();

                    return new CustomerReportItem
                    {
                        CustomerCode = c.CustomerCode,
                        FullName = $"{c.FirstName} {c.LastName}".Trim(),
                        CompanyName = c.CompanyName ?? string.Empty,
                        Status = c.Status,
                        CustomerType = c.CustomerType ?? string.Empty,
                        TotalSales = customerSales.Count,
                        TotalSalesAmount = customerSales.Sum(s => s.FinalAmount),
                        TotalTickets = customerTickets.Count,
                        CreatedDate = c.CreatedDate
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Müşteri raporu oluşturulurken hata: {ex.Message}", ex);
            }
        }

        public List<TaskReportItem> GetTaskReport(string status = null, string priority = null, int? userId = null)
        {
            try
            {
                var tasks = _unitOfWork.Tasks.GetAll();

                if (!string.IsNullOrEmpty(status))
                    tasks = tasks.Where(t => t.Status == status);

                if (!string.IsNullOrEmpty(priority))
                    tasks = tasks.Where(t => t.Priority == priority);

                if (userId.HasValue)
                    tasks = tasks.Where(t => t.AssignedToUserId == userId.Value);

                var users = _unitOfWork.Users.GetAll().ToDictionary(u => u.Id, u => u);

                return tasks.Select(t =>
                {
                    users.TryGetValue(t.AssignedToUserId, out var user);
                    return new TaskReportItem
                    {
                        Title = t.Title,
                        AssignedTo = user != null ? $"{user.FirstName} {user.LastName}".Trim() : "Bilinmiyor",
                        Priority = t.Priority,
                        Status = t.Status,
                        DueDate = t.DueDate,
                        CompletedDate = t.CompletedDate,
                        IsOverdue = t.Status != "Completed" && t.Status != "Cancelled" && t.DueDate < DateTime.Now
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Görev raporu oluşturulurken hata: {ex.Message}", ex);
            }
        }

        public List<TicketReportItem> GetTicketReport(string status = null, string priority = null)
        {
            try
            {
                var tickets = _unitOfWork.Tickets.GetAll();

                if (!string.IsNullOrEmpty(status))
                    tickets = tickets.Where(t => t.Status == status);

                if (!string.IsNullOrEmpty(priority))
                    tickets = tickets.Where(t => t.Priority == priority);

                var customers = _unitOfWork.Customers.GetAll().ToDictionary(c => c.Id, c => c);

                return tickets.Select(t =>
                {
                    customers.TryGetValue(t.CustomerId, out var customer);
                    return new TicketReportItem
                    {
                        TicketCode = t.TicketCode,
                        CustomerName = customer != null ? $"{customer.FirstName} {customer.LastName}".Trim() : "Bilinmiyor",
                        Title = t.Title,
                        Priority = t.Priority,
                        Status = t.Status,
                        Category = t.Category ?? string.Empty,
                        CreatedDate = t.CreatedDate,
                        ClosedDate = t.ClosedDate
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Destek talebi raporu oluşturulurken hata: {ex.Message}", ex);
            }
        }

        public Dictionary<string, decimal> GetMonthlySalesSummary(int year)
        {
            try
            {
                var result = new Dictionary<string, decimal>();
                var monthNames = new[] { "Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara" };

                for (int i = 0; i < 12; i++)
                    result[monthNames[i]] = 0;

                var sales = _unitOfWork.Sales.GetAll()
                    .Where(s => s.SaleDate.Year == year && s.Status == "Won");

                foreach (var sale in sales)
                {
                    var monthName = monthNames[sale.SaleDate.Month - 1];
                    result[monthName] += sale.FinalAmount;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Aylık satış özeti oluşturulurken hata: {ex.Message}", ex);
            }
        }

        public Dictionary<string, int> GetTicketStatusSummary()
        {
            try
            {
                var tickets = _unitOfWork.Tickets.GetAll().ToList();
                return new Dictionary<string, int>
                {
                    { "Açık", tickets.Count(t => t.Status == "Open") },
                    { "İşlemde", tickets.Count(t => t.Status == "InProgress") },
                    { "Beklemede", tickets.Count(t => t.Status == "OnHold") },
                    { "Çözüldü", tickets.Count(t => t.Status == "Resolved") },
                    { "Kapatıldı", tickets.Count(t => t.Status == "Closed") }
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Destek talebi özeti oluşturulurken hata: {ex.Message}", ex);
            }
        }

        public Dictionary<string, int> GetSalesPipelineSummary()
        {
            try
            {
                var sales = _unitOfWork.Sales.GetAll().ToList();
                return new Dictionary<string, int>
                {
                    { "Aday", sales.Count(s => s.Status == "Prospect") },
                    { "Teklif", sales.Count(s => s.Status == "Quote") },
                    { "Müzakere", sales.Count(s => s.Status == "Negotiation") },
                    { "Kazanıldı", sales.Count(s => s.Status == "Won") },
                    { "Kaybedildi", sales.Count(s => s.Status == "Lost") }
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Satış pipeline özeti oluşturulurken hata: {ex.Message}", ex);
            }
        }
    }
}
