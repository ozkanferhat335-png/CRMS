using System;
using System.Collections.Generic;
using System.Linq;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class SaleService
    {
        private readonly UnitOfWork _unitOfWork;

        public SaleService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Sale> GetAllSales()
        {
            try
            {
                return _unitOfWork.Sales.GetAll().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Satışlar getirirken hata: {ex.Message}", ex);
            }
        }

        public Sale GetSaleById(int saleId)
        {
            try
            {
                return _unitOfWork.Sales.GetById(saleId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Satış getirirken hata: {ex.Message}", ex);
            }
        }

        public List<Sale> GetSalesByStatus(string status)
        {
            try
            {
                return _unitOfWork.Sales.Find(s => s.Status == status).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Satış getirirken hata: {ex.Message}", ex);
            }
        }

        public bool AddSale(Sale sale)
        {
            try
            {
                if (string.IsNullOrEmpty(sale.SaleCode))
                    sale.SaleCode = GenerateSaleCode();

                sale.CreatedDate = DateTime.Now;
                _unitOfWork.Sales.Add(sale);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Satış eklenirken hata: {ex.Message}", ex);
            }
        }

        public bool UpdateSale(Sale sale)
        {
            try
            {
                sale.ModifiedDate = DateTime.Now;
                _unitOfWork.Sales.Update(sale);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Satış güncellenirken hata: {ex.Message}", ex);
            }
        }

        public bool DeleteSale(int saleId)
        {
            try
            {
                _unitOfWork.Sales.DeleteById(saleId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Satış silinirken hata: {ex.Message}", ex);
            }
        }

        public decimal GetTotalSalesAmount()
        {
            try
            {
                return _unitOfWork.Sales.GetTotalSalesAmount();
            }
            catch
            {
                return 0;
            }
        }

        public int GetTotalSalesCount()
        {
            try
            {
                return _unitOfWork.Sales.GetAll().Count();
            }
            catch
            {
                return 0;
            }
        }

        private string GenerateSaleCode()
        {
            return $"SALE{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}