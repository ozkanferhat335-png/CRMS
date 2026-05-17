using System;
using System.Collections.Generic;
using System.Linq;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class EventService
    {
        private readonly UnitOfWork _unitOfWork;

        public EventService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Event> GetAllEvents()
        {
            try
            {
                return _unitOfWork.Events.GetAll().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Etkinlikler getirirken hata: {ex.Message}", ex);
            }
        }

        public Event GetEventById(int eventId)
        {
            try
            {
                return _unitOfWork.Events.GetById(eventId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Etkinlik getirirken hata: {ex.Message}", ex);
            }
        }

        public List<Event> GetEventsByUser(int userId)
        {
            try
            {
                return _unitOfWork.Events.GetByUser(userId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcı etkinlikleri getirirken hata: {ex.Message}", ex);
            }
        }

        public List<Event> GetEventsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                return _unitOfWork.Events.GetByDateRange(startDate, endDate).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Tarih aralığı etkinlikleri getirirken hata: {ex.Message}", ex);
            }
        }

        public List<Event> GetTodayEvents(int userId)
        {
            try
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                return _unitOfWork.Events.GetByDateRange(today, tomorrow)
                    .Where(e => e.UserId == userId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Bugünkü etkinlikler getirirken hata: {ex.Message}", ex);
            }
        }

        public bool AddEvent(Event eventEntity)
        {
            try
            {
                eventEntity.CreatedDate = DateTime.Now;
                _unitOfWork.Events.Add(eventEntity);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Etkinlik eklenirken hata: {ex.Message}", ex);
            }
        }

        public bool UpdateEvent(Event eventEntity)
        {
            try
            {
                _unitOfWork.Events.Update(eventEntity);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Etkinlik güncellenirken hata: {ex.Message}", ex);
            }
        }

        public bool DeleteEvent(int eventId)
        {
            try
            {
                _unitOfWork.Events.DeleteById(eventId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Etkinlik silinirken hata: {ex.Message}", ex);
            }
        }

        public bool CompleteEvent(int eventId)
        {
            try
            {
                var ev = _unitOfWork.Events.GetById(eventId);
                if (ev == null)
                    return false;

                ev.Status = "Completed";
                _unitOfWork.Events.Update(ev);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Etkinlik tamamlanırken hata: {ex.Message}", ex);
            }
        }
    }
}
