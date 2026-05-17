using System;
using System.Collections.Generic;
using System.Linq;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class UserService
    {
        private readonly UnitOfWork _unitOfWork;

        public UserService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<User> GetAllUsers()
        {
            try
            {
                return _unitOfWork.Users.GetAll().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcıları getirirken hata: {ex.Message}", ex);
            }
        }

        public User GetUserById(int userId)
        {
            try
            {
                return _unitOfWork.Users.GetById(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcı getirirken hata: {ex.Message}", ex);
            }
        }

        public User GetUserByUsername(string username)
        {
            try
            {
                return _unitOfWork.Users.GetByUsername(username);
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcı getirirken hata: {ex.Message}", ex);
            }
        }

        public bool AddUser(User user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.PasswordHash))
                    user.PasswordHash = Security.PasswordHasher.HashPassword("DefaultPassword123!");

                user.CreatedDate = DateTime.Now;
                _unitOfWork.Users.Add(user);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcı eklenirken hata: {ex.Message}", ex);
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                user.ModifiedDate = DateTime.Now;
                _unitOfWork.Users.Update(user);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcı güncellenirken hata: {ex.Message}", ex);
            }
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                _unitOfWork.Users.DeleteById(userId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcı silinirken hata: {ex.Message}", ex);
            }
        }

        public List<User> GetUsersByRole(int roleId)
        {
            try
            {
                return _unitOfWork.Users.Find(u => u.RoleId == roleId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Rol kullanıcıları getirirken hata: {ex.Message}", ex);
            }
        }
    }
}