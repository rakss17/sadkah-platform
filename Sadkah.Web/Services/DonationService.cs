using System.Globalization;
using System.Net.Http.Headers;

namespace Sadkah.Web.Services
{
    public sealed class DonationService : IDonationService
    {
        private readonly IApiClient apiClient;
        private readonly IAuthSessionService authSession;

        public DonationService(IApiClient apiClient, IAuthSessionService authSession)
        {
            this.apiClient = apiClient;
            this.authSession = authSession;
        }

        public async Task<ServiceResult<DonationModel>> CreateDonationAsync(DonationModel donation)
        {
            var donorId = await authSession.GetCurrentUserIdFromTokenAsync();

            if (string.IsNullOrWhiteSpace(donorId?.ToString()))
            {
                return ServiceResult<DonationModel>.AuthenticationRequired();
            }

            return await apiClient.PostMultipartAsync<DonationModel>(
                "api/donations",
                () => BuildCreateDonationContent(donation, donorId.ToString()),
                requiresAuthentication: true);

        }

        private static MultipartFormDataContent BuildCreateDonationContent(DonationModel donation, string donorId)
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(donorId), "DonorId" },
                { new StringContent(donation.CampaignId.ToString()), nameof(DonationModel.CampaignId) },
                { new StringContent(donation.Amount.ToString(CultureInfo.InvariantCulture)), nameof(DonationModel.Amount) },
                { new StringContent(donation.IsAnonymous.ToString()), nameof(DonationModel.IsAnonymous)},
                { new StringContent(donation.PaymentReference), nameof(DonationModel.PaymentReference)},
                { new StringContent(donation.Method), nameof(DonationModel.Method)},
                { new StringContent(donation.Method), nameof(DonationModel.Method)}
            };

             if (!string.IsNullOrWhiteSpace(donation.Message))
            {
                content.Add(new StringContent(donation.Message), nameof(DonationModel.Message));
            }

            var byteContent = new ByteArrayContent(donation.ReceiptImageBytes);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue(donation.ReceiptImageFile.ContentType);
            content.Add(byteContent, "ReceiptImageFile", donation.ReceiptImageFile.Name);

            return content;
        }
    }
}