using System;
using System.Collections.Generic;
using System.Linq;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class NotificationService
    {
        private readonly UnitOfWork _unitOfWork;

        public NotificationService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Notification> GetAllNotifications()
        {
            try
            {
                return _unitOfWork.Notifications.GetAll().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Bildirimler getirirken hata: {ex.Message}", ex);
            }
        }

        public List<Notification> GetUnreadNotifications(int userId)
        {
            try
            {
                return _unitOfWork.Notifications.GetUnreadByUser(userId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Okunmamış bildirimler getirirken hata: {ex.Message}", ex);
            }
        }

        public int GetUnreadCount(int userId)
        {
            try
            {
                return _unitOfWork.Notifications.GetUnreadCount(userId);
            }
            catch
            {
                return 0;
            }
        }

        public bool SendNotification(int userId, string title, string message, string notificationType,
            int? relatedEntityId = null, string relatedEntityType = null)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    NotificationType = notificationType,
                    Status = "Unread",
                    RelatedEntityId = relatedEntityId,
                    RelatedEntityType = relatedEntityType,
                    CreatedDate = DateTime.Now
                };

                _unitOfWork.Notifications.Add(notification);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Bildirim gönderilirken hata: {ex.Message}", ex);
            }
        }

        public bool MarkAsRead(int notificationId)
        {
            try
            {
                _unitOfWork.Notifications.MarkAsRead(notificationId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Bildirim okundu işaretlenirken hata: {ex.Message}", ex);
            }
        }

        public bool MarkAllAsRead(int userId)
        {
            try
            {
                _unitOfWork.Notifications.MarkAllAsRead(userId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Tüm bildirimler okundu işaretlenirken hata: {ex.Message}", ex);
            }
        }

        public bool DeleteNotification(int notificationId)
        {
            try
            {
                _unitOfWork.Notifications.DeleteById(notificationId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Bildirim silinirken hata: {ex.Message}", ex);
            }
        }

        public void NotifyTaskAssigned(int userId, string taskTitle, int taskId)
        {
            SendNotification(userId, "Yeni Görev Atandı", $"'{taskTitle}' görevi size atandı.", "Task", taskId, "Task");
        }

        public void NotifyTicketAssigned(int userId, string ticketCode, int ticketId)
        {
            SendNotification(userId, "Yeni Destek Talebi", $"'{ticketCode}' destek talebi size atandı.", "Ticket", ticketId, "Ticket");
        }

        public void NotifyMeetingReminder(int userId, string meetingTitle, int meetingId)
        {
            SendNotification(userId, "Görüşme Hatırlatması", $"'{meetingTitle}' görüşmeniz yaklaşıyor.", "Meeting", meetingId, "Meeting");
        }

        public void NotifyOfferApproved(int userId, string offerCode, int offerId)
        {
            SendNotification(userId, "Teklif Onaylandı", $"'{offerCode}' teklifiniz onaylandı.", "Sale", offerId, "Offer");
        }
    }
}
