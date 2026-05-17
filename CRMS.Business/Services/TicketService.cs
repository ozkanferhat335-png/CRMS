using System;
using System.Collections.Generic;
using System.Linq;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class TicketService
    {
        private readonly UnitOfWork _unitOfWork;

        public TicketService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Ticket> GetAllTickets()
        {
            try
            {
                return _unitOfWork.Tickets.GetAll().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Destek talepleri getirirken hata: {ex.Message}", ex);
            }
        }

        public List<Ticket> GetOpenTickets()
        {
            try
            {
                return _unitOfWork.Tickets.GetOpenTickets().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Açık destek talepleri getirirken hata: {ex.Message}", ex);
            }
        }

        public Ticket GetTicketById(int ticketId)
        {
            try
            {
                return _unitOfWork.Tickets.GetById(ticketId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Destek talebi getirirken hata: {ex.Message}", ex);
            }
        }

        public bool AddTicket(Ticket ticket)
        {
            try
            {
                if (string.IsNullOrEmpty(ticket.TicketCode))
                    ticket.TicketCode = GenerateTicketCode();

                ticket.CreatedDate = DateTime.Now;
                _unitOfWork.Tickets.Add(ticket);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Destek talebi eklenirken hata: {ex.Message}", ex);
            }
        }

        public bool UpdateTicket(Ticket ticket)
        {
            try
            {
                ticket.ModifiedDate = DateTime.Now;
                _unitOfWork.Tickets.Update(ticket);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Destek talebi güncellenirken hata: {ex.Message}", ex);
            }
        }

        public bool DeleteTicket(int ticketId)
        {
            try
            {
                _unitOfWork.Tickets.DeleteById(ticketId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Destek talebi silinirken hata: {ex.Message}", ex);
            }
        }

        public bool ResolveTicket(int ticketId)
        {
            try
            {
                var ticket = _unitOfWork.Tickets.GetById(ticketId);
                if (ticket == null)
                    return false;

                ticket.Status = "Resolved";
                ticket.ClosedDate = DateTime.Now;
                _unitOfWork.Tickets.Update(ticket);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Destek talebi kapatılırken hata: {ex.Message}", ex);
            }
        }

        public int GetOpenTicketsCount()
        {
            try
            {
                return _unitOfWork.Tickets.GetOpenTickets().Count();
            }
            catch
            {
                return 0;
            }
        }

        private string GenerateTicketCode()
        {
            return $"TICKET{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}