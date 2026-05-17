using System;
using System.Collections.Generic;
using System.Linq;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class TaskService
    {
        private readonly UnitOfWork _unitOfWork;

        public TaskService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Task> GetAllTasks()
        {
            try
            {
                return _unitOfWork.Tasks.GetAll().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Görevler getirirken hata: {ex.Message}", ex);
            }
        }

        public List<Task> GetPendingTasks()
        {
            try
            {
                return _unitOfWork.Tasks.GetPendingTasks().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Bekleyen görevler getirirken hata: {ex.Message}", ex);
            }
        }

        public List<Task> GetTasksByUser(int userId)
        {
            try
            {
                return _unitOfWork.Tasks.Find(t => t.AssignedToUserId == userId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcı görevleri getirirken hata: {ex.Message}", ex);
            }
        }

        public Task GetTaskById(int taskId)
        {
            try
            {
                return _unitOfWork.Tasks.GetById(taskId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Görev getirirken hata: {ex.Message}", ex);
            }
        }

        public bool AddTask(Task task)
        {
            try
            {
                task.CreatedDate = DateTime.Now;
                _unitOfWork.Tasks.Add(task);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Görev eklenirken hata: {ex.Message}", ex);
            }
        }

        public bool UpdateTask(Task task)
        {
            try
            {
                task.ModifiedDate = DateTime.Now;
                _unitOfWork.Tasks.Update(task);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Görev güncellenirken hata: {ex.Message}", ex);
            }
        }

        public bool DeleteTask(int taskId)
        {
            try
            {
                _unitOfWork.Tasks.DeleteById(taskId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Görev silinirken hata: {ex.Message}", ex);
            }
        }

        public bool CompleteTask(int taskId)
        {
            try
            {
                var task = _unitOfWork.Tasks.GetById(taskId);
                if (task == null)
                    return false;

                task.Status = "Completed";
                task.CompletedDate = DateTime.Now;
                task.ModifiedDate = DateTime.Now;
                _unitOfWork.Tasks.Update(task);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Görev tamamlanırken hata: {ex.Message}", ex);
            }
        }

        public int GetPendingTaskCount()
        {
            try
            {
                return _unitOfWork.Tasks.GetPendingTasks().Count();
            }
            catch
            {
                return 0;
            }
        }
    }
}