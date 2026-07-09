namespace Sadkah.API.Services
{
    public class DonationService : IDonationService
    {
        private readonly IDonationRepository _donationRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public DonationService(IDonationRepository donationRepository, ICampaignRepository campaignRepository, ICloudinaryService cloudinaryService)
        {
            _donationRepository = donationRepository;
            _campaignRepository = campaignRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<PagedResult<DonationDto>> GetAllDonationsAsync(QueryObject query)
        {
            var donations = await _donationRepository.GetAllDonationsAsync(query);

            return new PagedResult<DonationDto>
            {
                Items = donations.Items.Select(d => d.ToDonationDto()).ToList(),
                TotalCount = donations.TotalCount,
                PageSize = donations.PageSize,
                CurrentPage = donations.CurrentPage,
                TotalPages = donations.TotalPages
            };
        }

        public async Task<DonationDto?> GetDonationByIdAsync(Guid id)
        {
            var donation = await _donationRepository.GetDonationByIdAsync(id);
            return donation?.ToDonationDto();
        }

        public async Task<ServiceResult<DonationDto>> CreateDonationAsync(CreateDonationRequestDto createDto)
        {
            var isCampaignExisting = await _campaignRepository.IsCampaignExistingAsync(createDto.CampaignId);

            if (!isCampaignExisting)
            {
                return ServiceResult<DonationDto>.Failure("Campaign does not exist.");
            }

            var donation = createDto.ToDonationFromCreateDto();

            var directory = $"sadkah/images/donation/receipt";
            var upload = await _cloudinaryService.UploadImageAsync(createDto.ReceiptImageFile, directory);

            donation.ReceiptImageUrl = upload.SecureUrl.ToString();
            donation.ReceiptImagePublicId = upload.PublicId;
            
            var createdDonation = await _donationRepository.CreateDonationAsync(donation);

            if (createdDonation == null)
            {
                return ServiceResult<DonationDto>.Failure("Failed to create donation.");
            }

            return ServiceResult<DonationDto>.Success(createdDonation.ToDonationDto());
        }

        public async Task<DonationDto?> UpdateAnonymousDonationAsync(Guid id, UpdateAnonymousDonationRequestDto updateDto)
        {
            var updatedDonation = await _donationRepository.UpdateAnonymousDonationAsync(id, updateDto.IsAnonymous!.Value);
            return updatedDonation?.ToDonationDto();
        }
    }
}
