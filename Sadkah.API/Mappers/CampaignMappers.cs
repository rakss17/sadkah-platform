using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Mappers
{
    public static class CampaignMappers
    {
        public static CampaignDto ToCampaignDto(this Campaign campaignModel)
        {
            return new CampaignDto
            {
                Id = campaignModel.Id,
                Title = campaignModel.Title,
                CategoryName = campaignModel.Category.Name,
                Description = campaignModel.Description,
                TargetAmount = campaignModel.TargetAmount,
                CurrentAmount = campaignModel.CurrentAmount,
                Deadline = campaignModel.Deadline,
                Status = campaignModel.Status,
                IsVerified = campaignModel.IsVerified,
                OwnerName = campaignModel.Owner != null ? $"{campaignModel.Owner.FirstName} {campaignModel.Owner.LastName}" : "Unknown User",
                Donations = campaignModel.Donations.Select(d => d.ToDonationDto()).ToList(),
                AddressLine1 = campaignModel.AddressLine1,
                AddressLine2 = campaignModel.AddressLine2,
                Barangay = campaignModel.Barangay,
                City = campaignModel.City,
                Province = campaignModel.Province,
                Country = campaignModel.Country,
                DonationMethods = campaignModel.DonationMethods.Select(dm => dm.ToDonationMethodDto()).ToList()
            };
        }

        public static DonationMethodDto ToDonationMethodDto(this DonationMethod donationMethod)
        {
            return new DonationMethodDto
            {
                Id = donationMethod.Id,
                Type = donationMethod.Type,
                Provider = donationMethod.Provider,
                QrImageUrl = donationMethod.QrCodeImageUrl,
                QrImagePublicId = donationMethod.QrCodeImagePublicId,
                CampaignId = donationMethod.CampaignId
            };
        }

        public static Campaign ToCampaignFromCreateDto(this CreateCampaignRequestDto createDto)
        {
            return new Campaign
            {
                OwnerId = createDto.OwnerId,
                Title = createDto.Title!,
                Description = createDto.Description!,
                CategoryId = createDto.CategoryId,
                TargetAmount = createDto.TargetAmount,
                Deadline = createDto.Deadline!.Value,
                Status = Enums.CampaignStatus.Active,
                IsVerified = false,
                AddressLine1 = createDto.AddressLine1,
                AddressLine2 = createDto.AddressLine2,
                Barangay = createDto.Barangay,
                City = createDto.City,
                Province = createDto.Province,
                Country = createDto.Country,
            };
        }

        public static UpdateCampaignRequestDto ToCampaignFromUpdateResponseDto(this Campaign campaign)
        {
            return new UpdateCampaignRequestDto
            {
                OwnerId = campaign.OwnerId,
                Title = campaign.Title,
                Description = campaign.Description,
                TargetAmount = campaign.TargetAmount,
                Deadline = campaign.Deadline,
            };
        }
    }
}