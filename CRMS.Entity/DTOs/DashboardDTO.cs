using System;
using System.Collections.Generic;

namespace CRMS.Entity.DTOs
{
    public class DashboardDTO
    {
        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public int NewOffers { get; set; }
        public int PendingTasks { get; set; }
        public int OpenTickets { get; set; }
        public List<DailySalesDTO> DailySales { get; set; }
        public List<RecentActivityDTO> RecentActivities { get; set; }
    }

    public class DailySalesDTO
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }

    public class RecentActivityDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ActivityType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}