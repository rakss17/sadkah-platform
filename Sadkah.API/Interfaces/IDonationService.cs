namespace Sadkah.API.Interfaces
{
    public interface IDonationService
    {
        Task<PagedResult<DonationDto>> GetAllDonationsAsync(QueryObject query);
        Task<DonationDto?> GetDonationByIdAsync(Guid id);
        Task<ServiceResult<DonationDto>> CreateDonationAsync(CreateDonationRequestDto createDto);
        Task<DonationDto?> UpdateAnonymousDonationAsync(Guid id, UpdateAnonymousDonationRequestDto updateDto);
    }
}
