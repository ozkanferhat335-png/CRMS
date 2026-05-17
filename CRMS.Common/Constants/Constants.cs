using System;
using System.Collections.Generic;

namespace CRMS.Common.Constants
{
    public static class RoleConstants
    {
        public const string Administrator = "Administrator";
        public const string SalesRepresentative = "Sales Representative";
        public const string SupportStaff = "Support Staff";
        public const string Manager = "Manager";
    }

    public static class PermissionConstants
    {
        // Customer Permissions
        public const string ViewCustomers = "ViewCustomers";
        public const string CreateCustomer = "CreateCustomer";
        public const string EditCustomer = "EditCustomer";
        public const string DeleteCustomer = "DeleteCustomer";

        // Sale Permissions
        public const string ViewSales = "ViewSales";
        public const string CreateSale = "CreateSale";
        public const string EditSale = "EditSale";
        public const string DeleteSale = "DeleteSale";

        // Offer Permissions
        public const string ViewOffers = "ViewOffers";
        public const string CreateOffer = "CreateOffer";
        public const string ApproveOffer = "ApproveOffer";

        // Task Permissions
        public const string ViewTasks = "ViewTasks";
        public const string CreateTask = "CreateTask";
        public const string EditTask = "EditTask";

        // Ticket Permissions
        public const string ViewTickets = "ViewTickets";
        public const string CreateTicket = "CreateTicket";
        public const string ResolveTicket = "ResolveTicket";

        // Report Permissions
        public const string ViewReports = "ViewReports";

        // Admin Permissions
        public const string ManageUsers = "ManageUsers";
        public const string ManageRoles = "ManageRoles";
        public const string ViewLogs = "ViewLogs";
        public const string SystemSettings = "SystemSettings";
    }

    public static class TaskStatusConstants
    {
        public const string Pending = "Pending";
        public const string InProgress = "InProgress";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
    }

    public static class TaskPriorityConstants
    {
        public const string Low = "Low";
        public const string Medium = "Medium";
        public const string High = "High";
        public const string Urgent = "Urgent";
    }

    public static class SaleStatusConstants
    {
        public const string Prospect = "Prospect";
        public const string Quote = "Quote";
        public const string Negotiation = "Negotiation";
        public const string Won = "Won";
        public const string Lost = "Lost";
        public const string Cancelled = "Cancelled";
    }

    public static class OfferStatusConstants
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Expired = "Expired";
    }

    public static class TicketStatusConstants
    {
        public const string Open = "Open";
        public const string InProgress = "InProgress";
        public const string OnHold = "OnHold";
        public const string Resolved = "Resolved";
        public const string Closed = "Closed";
    }

    public static class TicketPriorityConstants
    {
        public const string Low = "Low";
        public const string Medium = "Medium";
        public const string High = "High";
        public const string Urgent = "Urgent";
    }

    public static class CustomerStatusConstants
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Prospect = "Prospect";
    }

    public static class CustomerTypeConstants
    {
        public const string Individual = "Individual";
        public const string Corporate = "Corporate";
    }

    public static class MeetingTypeConstants
    {
        public const string Call = "Call";
        public const string Meeting = "Meeting";
        public const string Email = "Email";
        public const string WhatsApp = "WhatsApp";
    }

    public static class NotificationTypeConstants
    {
        public const string Task = "Task";
        public const string Meeting = "Meeting";
        public const string Sale = "Sale";
        public const string Ticket = "Ticket";
        public const string System = "System";
    }
}