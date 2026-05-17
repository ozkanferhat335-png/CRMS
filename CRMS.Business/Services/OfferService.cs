using System;
using System.Collections.Generic;
using System.Linq;
using CRMS.DataAccess.Repositories;
using CRMS.Entity.Models;

namespace CRMS.Business.Services
{
    public class OfferService
    {
        private readonly UnitOfWork _unitOfWork;

        public OfferService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Offer> GetAllOffers()
        {
            try
            {
                return _unitOfWork.Offers.GetAll().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Teklifler getirirken hata: {ex.Message}", ex);
            }
        }

        public Offer GetOfferById(int offerId)
        {
            try
            {
                return _unitOfWork.Offers.GetById(offerId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Teklif getirirken hata: {ex.Message}", ex);
            }
        }

        public List<Offer> GetOffersByStatus(string status)
        {
            try
            {
                return _unitOfWork.Offers.Find(o => o.Status == status).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Teklif getirirken hata: {ex.Message}", ex);
            }
        }

        public bool AddOffer(Offer offer)
        {
            try
            {
                if (string.IsNullOrEmpty(offer.OfferCode))
                    offer.OfferCode = GenerateOfferCode();

                offer.CreatedDate = DateTime.Now;
                _unitOfWork.Offers.Add(offer);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Teklif eklenirken hata: {ex.Message}", ex);
            }
        }

        public bool UpdateOffer(Offer offer)
        {
            try
            {
                offer.ModifiedDate = DateTime.Now;
                _unitOfWork.Offers.Update(offer);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Teklif güncellenirken hata: {ex.Message}", ex);
            }
        }

        public bool DeleteOffer(int offerId)
        {
            try
            {
                _unitOfWork.Offers.DeleteById(offerId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Teklif silinirken hata: {ex.Message}", ex);
            }
        }

        public bool ApproveOffer(int offerId)
        {
            try
            {
                var offer = _unitOfWork.Offers.GetById(offerId);
                if (offer == null)
                    return false;

                offer.Status = "Approved";
                offer.ApprovalDate = DateTime.Now;
                _unitOfWork.Offers.Update(offer);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Teklif onaylanırken hata: {ex.Message}", ex);
            }
        }

        public bool RejectOffer(int offerId)
        {
            try
            {
                var offer = _unitOfWork.Offers.GetById(offerId);
                if (offer == null)
                    return false;

                offer.Status = "Rejected";
                _unitOfWork.Offers.Update(offer);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Teklif reddedilirken hata: {ex.Message}", ex);
            }
        }

        public int GetNewOffersCount()
        {
            try
            {
                return _unitOfWork.Offers.Find(o => o.Status == "Pending").Count();
            }
            catch
            {
                return 0;
            }
        }

        private string GenerateOfferCode()
        {
            return $"OFFER{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}