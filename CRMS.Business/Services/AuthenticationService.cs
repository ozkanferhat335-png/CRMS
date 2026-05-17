using System;
using CRMS.Business.Services.Security;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.DTOs;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class AuthenticationService
    {
        private readonly UnitOfWork _unitOfWork;
        private const int MaxFailedLoginAttempts = 3;
        private const int LockoutDurationMinutes = 15;

        public AuthenticationService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public LoginResponseDTO Login(string username, string password)
        {
            try
            {
                var user = _unitOfWork.Users.GetByUsername(username);

                if (user == null)
                {
                    return new LoginResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Geçersiz kullanıcı adı veya parola"
                    };
                }

                // Check if user is locked
                if (user.IsLocked)
                {
                    return new LoginResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Kullanıcı hesabı kilitli. Lütfen sistem yöneticisine başvurunuz."
                    };
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    return new LoginResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Kullanıcı hesabı pasif durumdadır."
                    };
                }

                // Verify password
                if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
                {
                    user.FailedLoginAttempts++;

                    // Lock account if max attempts reached
                    if (user.FailedLoginAttempts >= MaxFailedLoginAttempts)
                    {
                        user.IsLocked = true;
                    }

                    _unitOfWork.Users.Update(user);

                    return new LoginResponseDTO
                    {
                        IsSuccess = false,
                        Message = $"Geçersiz parola. Kalan deneme: {MaxFailedLoginAttempts - user.FailedLoginAttempts}"
                    };
                }

                // Reset failed login attempts
                user.FailedLoginAttempts = 0;
                user.LastLoginDate = DateTime.Now;
                _unitOfWork.Users.Update(user);

                // Get user role
                var role = _unitOfWork.Roles.GetById(user.RoleId);

                var userDTO = new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    IsLocked = user.IsLocked,
                    RoleId = user.RoleId,
                    RoleName = role?.RoleName ?? string.Empty,
                    LastLoginDate = user.LastLoginDate ?? DateTime.Now,
                    CreatedDate = user.CreatedDate
                };

                return new LoginResponseDTO
                {
                    IsSuccess = true,
                    Message = "Giriş başarılı",
                    User = userDTO,
                    ExpiryDate = DateTime.Now.AddHours(8)
                };
            }
            catch (Exception ex)
            {
                return new LoginResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Giriş sırasında hata oluştu: {ex.Message}"
                };
            }
        }

        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = _unitOfWork.Users.GetById(userId);
                if (user == null)
                    return false;

                if (!PasswordHasher.VerifyPassword(oldPassword, user.PasswordHash))
                    return false;

                user.PasswordHash = PasswordHasher.HashPassword(newPassword);
                user.ModifiedDate = DateTime.Now;
                _unitOfWork.Users.Update(user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ResetPassword(int userId, string newPassword)
        {
            try
            {
                var user = _unitOfWork.Users.GetById(userId);
                if (user == null)
                    return false;

                user.PasswordHash = PasswordHasher.HashPassword(newPassword);
                user.FailedLoginAttempts = 0;
                user.IsLocked = false;
                user.ModifiedDate = DateTime.Now;
                _unitOfWork.Users.Update(user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UnlockUser(int userId)
        {
            try
            {
                var user = _unitOfWork.Users.GetById(userId);
                if (user == null)
                    return false;

                user.IsLocked = false;
                user.FailedLoginAttempts = 0;
                _unitOfWork.Users.Update(user);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}