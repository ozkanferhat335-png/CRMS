using System;
using System.Collections.Generic;
using System.Linq;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class SettingsService
    {
        private readonly UnitOfWork _unitOfWork;

        public SettingsService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<SystemSettings> GetAllSettings()
        {
            try
            {
                return _unitOfWork.Settings.GetAll().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ayarlar getirirken hata: {ex.Message}", ex);
            }
        }

        public string GetSettingValue(string key, string defaultValue = "")
        {
            try
            {
                var setting = _unitOfWork.Settings.GetByKey(key);
                return setting?.SettingValue ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public bool SetSettingValue(string key, string value, int? modifiedByUserId = null)
        {
            try
            {
                _unitOfWork.Settings.UpsertByKey(key, value, modifiedByUserId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ayar kaydedilirken hata: {ex.Message}", ex);
            }
        }

        public bool AddSetting(SystemSettings setting)
        {
            try
            {
                setting.CreatedDate = DateTime.Now;
                _unitOfWork.Settings.Add(setting);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ayar eklenirken hata: {ex.Message}", ex);
            }
        }

        public bool UpdateSetting(SystemSettings setting)
        {
            try
            {
                setting.ModifiedDate = DateTime.Now;
                _unitOfWork.Settings.Update(setting);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ayar güncellenirken hata: {ex.Message}", ex);
            }
        }

        public bool DeleteSetting(int settingId)
        {
            try
            {
                _unitOfWork.Settings.DeleteById(settingId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ayar silinirken hata: {ex.Message}", ex);
            }
        }

        public int GetIntSetting(string key, int defaultValue = 0)
        {
            var value = GetSettingValue(key);
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        public bool GetBoolSetting(string key, bool defaultValue = false)
        {
            var value = GetSettingValue(key);
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }
    }
}
