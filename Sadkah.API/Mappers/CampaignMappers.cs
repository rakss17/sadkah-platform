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
                Description = campaignModel.Description,
                TargetAmount = campaignModel.TargetAmount,
                CurrentAmount = campaignModel.CurrentAmount,
                Deadline = campaignModel.Deadline,
                Status = campaignModel.Status,
                IsVerified = campaignModel.IsVerified,
                OwnerName = campaignModel.Owner != null ? $"{campaignModel.Owner.FirstName} {campaignModel.Owner.LastName}" : "Unknown User",
                Donations = campaignModel.Donations.Select(d => d.ToDonationDto()).ToList()
            };
        }

        public static Campaign ToCampaignFromCreateDto(this CreateCampaignRequestDto createDto)
        {
            return new Campaign
            {
                OwnerId = createDto.OwnerId,
                Title = createDto.Title!,
                Description = createDto.Description!,
                TargetAmount = createDto.TargetAmount,
                Deadline = createDto.Deadline!.Value,
                Status = Enums.CampaignStatus.Active,
                IsVerified = false,
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