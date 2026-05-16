using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Mappers
{
    public static class DonationMappers
    {
        public static DonationDto ToDonationDto(this Donation donationModel)
        {
            return new DonationDto
            {
                Id = donationModel.Id,
                DonorId = donationModel.DonorId,
                DonorName = donationModel.Donor != null ? $"{donationModel.Donor.FirstName} {donationModel.Donor.LastName}" : "Unknown User",
                CampaignId = donationModel.CampaignId,
                Amount = donationModel.Amount,
                IsAnonymous = donationModel.IsAnonymous,
                PaymentReference = donationModel.PaymentReference,
                CreatedAt = donationModel.CreatedAt
            };
        }

        public static Donation ToDonationFromCreateDto(this CreateDonationRequestDto createDto)
        {
            return new Donation
            {
                DonorId = createDto.DonorId,
                CampaignId = createDto.CampaignId,
                Amount = createDto.Amount,
                IsAnonymous = createDto.IsAnonymous!.Value,
                PaymentReference = createDto.PaymentReference
            };
        }
    }
}