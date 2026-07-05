namespace Sadkah.API.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public CampaignService(ICampaignRepository campaignRepository, ICloudinaryService cloudinaryService)
        {
            _campaignRepository = campaignRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<PagedResult<CampaignDto>> GetAllCampaignsAsync(QueryObject query)
        {
            var campaigns = await _campaignRepository.GetAllCampaignsAsync(query);

            return new PagedResult<CampaignDto>
            {
                Items = campaigns.Items.Select(c => c.ToCampaignDto()).ToList(),
                TotalCount = campaigns.TotalCount,
                PageSize = campaigns.PageSize,
                CurrentPage = campaigns.CurrentPage,
                TotalPages = campaigns.TotalPages
            };
        }

        public async Task<CampaignDto?> GetCampaignByIdAsync(Guid id)
        {
            var campaign = await _campaignRepository.GetCampaignByIdAsync(id);
            return campaign?.ToCampaignDto();
        }

        public async Task<CampaignDto?> CreateCampaignAsync(CreateCampaignRequestDto createDto)
        {
            var campaign = createDto.ToCampaignFromCreateDto();

            campaign.DonationMethods.Clear();

            foreach (var dto in createDto.DonationMethods)
            {
                var directory = $"campaigns/donation-methods/qr-codes";
                var upload = await _cloudinaryService.UploadImageAsync(dto.QrImageFile, directory);

                campaign.DonationMethods.Add(new DonationMethod
                {
                    Type = dto.Type,
                    Provider = dto.Provider,
                    QrCodeImageUrl = upload.SecureUrl.ToString(),
                    QrCodeImagePublicId = upload.PublicId,
                    CampaignId = campaign.Id
                });
            }
            var createdCampaign = await _campaignRepository.CreateCampaignAsync(campaign);

            return createdCampaign?.ToCampaignDto();
        }

        public async Task<CampaignDto?> UpdateCampaignAsync(Guid id, UpdateCampaignRequestDto updateDto)
        {
            var updatedCampaign = await _campaignRepository.UpdateCampaignAsync(id, updateDto);
            return updatedCampaign?.ToCampaignDto();
        }

        public async Task<CampaignDto?> ArchiveCampaignAsync(Guid id)
        {
            var archivedCampaign = await _campaignRepository.ArchiveCampaignAsync(id);
            return archivedCampaign?.ToCampaignDto();
        }

        public async Task<IEnumerable<CampaignCategoryDto>> GetCampaignCategoriesAsync()
        {
            var categories = await _campaignRepository.GetCampaignCategoriesAsync();
            return categories;
        }
    }
}
