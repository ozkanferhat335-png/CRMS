using System;
using System.Collections.Generic;
using System.Linq;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class MeetingService
    {
        private readonly UnitOfWork _unitOfWork;

        public MeetingService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Meeting> GetAllMeetings()
        {
            try
            {
                return _unitOfWork.Meetings.GetAll().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Görüşmeler getirirken hata: {ex.Message}", ex);
            }
        }

        public Meeting GetMeetingById(int meetingId)
        {
            try
            {
                return _unitOfWork.Meetings.GetById(meetingId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Görüşme getirirken hata: {ex.Message}", ex);
            }
        }

        public List<Meeting> GetMeetingsByCustomer(int customerId)
        {
            try
            {
                return _unitOfWork.Meetings.GetByCustomerId(customerId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Müşteri görüşmeleri getirirken hata: {ex.Message}", ex);
            }
        }

        public List<Meeting> GetUpcomingMeetings(int userId)
        {
            try
            {
                return _unitOfWork.Meetings.GetUpcomingMeetings(userId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Yaklaşan görüşmeler getirirken hata: {ex.Message}", ex);
            }
        }

        public List<Meeting> GetMeetingsByStatus(string status)
        {
            try
            {
                return _unitOfWork.Meetings.Find(m => m.Status == status).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Görüşmeler getirirken hata: {ex.Message}", ex);
            }
        }

        public bool AddMeeting(Meeting meeting)
        {
            try
            {
                meeting.CreatedDate = DateTime.Now;
                _unitOfWork.Meetings.Add(meeting);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Görüşme eklenirken hata: {ex.Message}", ex);
            }
        }

        public bool UpdateMeeting(Meeting meeting)
        {
            try
            {
                _unitOfWork.Meetings.Update(meeting);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Görüşme güncellenirken hata: {ex.Message}", ex);
            }
        }

        public bool DeleteMeeting(int meetingId)
        {
            try
            {
                _unitOfWork.Meetings.DeleteById(meetingId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Görüşme silinirken hata: {ex.Message}", ex);
            }
        }

        public bool CompleteMeeting(int meetingId, string result)
        {
            try
            {
                var meeting = _unitOfWork.Meetings.GetById(meetingId);
                if (meeting == null)
                    return false;

                meeting.Status = "Completed";
                meeting.Result = result;
                _unitOfWork.Meetings.Update(meeting);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Görüşme tamamlanırken hata: {ex.Message}", ex);
            }
        }

        public int GetTotalMeetingsCount()
        {
            try
            {
                return _unitOfWork.Meetings.GetAll().Count();
            }
            catch
            {
                return 0;
            }
        }
    }
}
