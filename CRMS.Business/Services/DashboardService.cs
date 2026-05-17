using System;
using System.Collections.Generic;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.DTOs;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class DashboardService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly CustomerService _customerService;
        private readonly SaleService _saleService;
        private readonly OfferService _offerService;
        private readonly TaskService _taskService;
        private readonly TicketService _ticketService;

        public DashboardService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _customerService = new CustomerService(unitOfWork);
            _saleService = new SaleService(unitOfWork);
            _offerService = new OfferService(unitOfWork);
            _taskService = new TaskService(unitOfWork);
            _ticketService = new TicketService(unitOfWork);
        }

        public DashboardDTO GetDashboardData()
        {
            try
            {
                var dashboard = new DashboardDTO
                {
                    TotalCustomers = _customerService.GetTotalCustomers(),
                    ActiveCustomers = _customerService.GetActiveCustomers(),
                    TotalSalesAmount = _saleService.GetTotalSalesAmount(),
                    NewOffers = _offerService.GetNewOffersCount(),
                    PendingTasks = _taskService.GetPendingTaskCount(),
                    OpenTickets = _ticketService.GetOpenTicketsCount(),
                    DailySales = GetDailySalesData(),
                    RecentActivities = GetRecentActivities()
                };

                return dashboard;
            }
            catch (Exception ex)
            {
                throw new Exception($"Dashboard verileri getirirken hata: {ex.Message}", ex);
            }
        }

        private List<DailySalesDTO> GetDailySalesData()
        {
            var dailySales = new List<DailySalesDTO>();
            try
            {
                var sales = _saleService.GetAllSales();
                var groupedSales = new Dictionary<DateTime, decimal>();

                foreach (var sale in sales)
                {
                    var date = sale.SaleDate.Date;
                    if (!groupedSales.ContainsKey(date))
                        groupedSales[date] = 0;

                    groupedSales[date] += sale.FinalAmount;
                }

                foreach (var kvp in groupedSales)
                {
                    dailySales.Add(new DailySalesDTO
                    {
                        Date = kvp.Key,
                        Amount = kvp.Value
                    });
                }
            }
            catch
            {
                // Return empty list on error
            }

            return dailySales;
        }

        private List<RecentActivityDTO> GetRecentActivities()
        {
            var activities = new List<RecentActivityDTO>();
            try
            {
                var logs = _unitOfWork.Logs.GetAll();
                int count = 0;

                foreach (var log in logs)
                {
                    if (count >= 10) break; // Limit to 10 recent activities

                    activities.Add(new RecentActivityDTO
                    {
                        Id = log.Id,
                        Title = log.TableName,
                        Description = log.Description ?? log.Action,
                        ActivityType = log.Action,
                        CreatedDate = log.CreatedDate
                    });

                    count++;
                }
            }
            catch
            {
                // Return empty list on error
            }

            return activities;
        }
    }
}