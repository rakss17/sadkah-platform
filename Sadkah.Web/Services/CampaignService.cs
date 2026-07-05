using System.Globalization;
using System.Net.Http.Headers;

namespace Sadkah.Web.Services
{
    public sealed class CampaignService : ICampaignService
    {
        private readonly IApiClient apiClient;
        private readonly IAuthSessionService authSession;

        public CampaignService(IApiClient apiClient, IAuthSessionService authSession)
        {
            this.apiClient = apiClient;
            this.authSession = authSession;
        }

        public async Task<ServiceResult<PagedResult<CampaignSummary>>> GetCampaignsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? category = null,
            string? location = null,
            bool? isVerified = null,
            bool mineOnly = false)
        {
            var queryParameters = new List<string>
            {
                $"pageNumber={Math.Max(1, pageNumber)}",
                $"pageSize={Math.Max(1, pageSize)}"
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                queryParameters.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                queryParameters.Add($"category={Uri.EscapeDataString(category)}");
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                queryParameters.Add($"location={Uri.EscapeDataString(location)}");
            }

            if (isVerified.HasValue)
            {
                queryParameters.Add($"isVerified={isVerified.Value.ToString().ToLowerInvariant()}");
            }

            if (mineOnly)
            {
                var accessToken = await authSession.GetAccessTokenAsync();
                var ownerId = GetOwnerId(accessToken);

                if (string.IsNullOrWhiteSpace(ownerId))
                {
                    return ServiceResult<PagedResult<CampaignSummary>>.AuthenticationRequired();
                }

                queryParameters.Add($"userId={Uri.EscapeDataString(ownerId)}");
            }

            var url = $"api/campaigns?{string.Join("&", queryParameters)}";

            var result = await apiClient.GetAsync<List<CampaignModel>>(url, requiresAuthentication: true);

            if (result.RequiresAuthentication)
            {
                return ServiceResult<PagedResult<CampaignSummary>>.AuthenticationRequired(result.Message);
            }

            if (!result.Success || result.Data is null)
            {
                return ServiceResult<PagedResult<CampaignSummary>>.Fail(result.Message);
            }

            var currentUserFullName = await authSession.GetCurrentUserFullNameAsync();
            var campaigns = result.Data
                .Select(campaign => campaign.ToSummary(currentUserFullName))
                .ToList();

            var pagedResult = new PagedResult<CampaignSummary>
            {
                Items = campaigns,
                TotalCount = GetMetadataValue(result.Metadata, "totalCount", campaigns.Count),
                PageSize = GetMetadataValue(result.Metadata, "pageSize", pageSize),
                CurrentPage = GetMetadataValue(result.Metadata, "currentPage", pageNumber),
                TotalPages = GetMetadataValue(result.Metadata, "totalPages", campaigns.Count > 0 ? 1 : 0)
            };

            return ServiceResult<PagedResult<CampaignSummary>>.Ok(pagedResult, result.Message);
        }

        public async Task<ServiceResult<CampaignModel>> CreateCampaignAsync(CampaignModel campaign)
        {
            var accessToken = await authSession.GetAccessTokenAsync();
            var ownerId = GetOwnerId(accessToken);

            if (string.IsNullOrWhiteSpace(ownerId))
            {
                return ServiceResult<CampaignModel>.AuthenticationRequired();
            }

            return await apiClient.PostMultipartAsync<CampaignModel>(
                "api/campaigns",
                () => BuildCreateCampaignContent(campaign, ownerId),
                requiresAuthentication: true);
        }

        private static MultipartFormDataContent BuildCreateCampaignContent(CampaignModel campaign, string ownerId)
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(ownerId), "OwnerId" },
                { new StringContent(campaign.Title), nameof(CampaignModel.Title) },
                { new StringContent(campaign.Description), nameof(CampaignModel.Description) },
                { new StringContent(campaign.CategoryId.ToString()), nameof(CampaignModel.CategoryId) },
                { new StringContent(campaign.TargetAmount.ToString(CultureInfo.InvariantCulture)), nameof(CampaignModel.TargetAmount) },
                { new StringContent(campaign.Deadline.GetValueOrDefault().ToString("o")), nameof(CampaignModel.Deadline) },
                { new StringContent(campaign.AddressLine1), nameof(CampaignModel.AddressLine1) },
                { new StringContent(campaign.Barangay), nameof(CampaignModel.Barangay) },
                { new StringContent(campaign.City), nameof(CampaignModel.City) },
                { new StringContent(campaign.Province), nameof(CampaignModel.Province) },
                { new StringContent(campaign.Country), nameof(CampaignModel.Country) }
            };

            if (!string.IsNullOrWhiteSpace(campaign.AddressLine2))
            {
                content.Add(new StringContent(campaign.AddressLine2), nameof(CampaignModel.AddressLine2));
            }

            for (var i = 0; i < campaign.DonationMethods.Count; i++)
            {
                var method = campaign.DonationMethods[i];

                content.Add(new StringContent(method.Type?.ToString() ?? string.Empty), $"DonationMethods[{i}].Type");
                content.Add(new StringContent(method.Provider), $"DonationMethods[{i}].Provider");

                if (method.QrImageBytes is not null && method.QrImageFile is not null)
                {
                    var byteContent = new ByteArrayContent(method.QrImageBytes);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue(method.QrImageFile.ContentType);
                    content.Add(byteContent, $"DonationMethods[{i}].QrImageFile", method.QrImageFile.Name);
                }
            }

            return content;
        }

        public async Task<ServiceResult<IEnumerable<CampaignCategoryModel>>> GetCampaignCategoriesAsync()
        {
            var result = await apiClient.GetAsync<List<CampaignCategoryModel>>("api/campaigns/categories", requiresAuthentication: true);

            if (!result.Success || result.Data is null)
            {
                return ServiceResult<IEnumerable<CampaignCategoryModel>>.Fail(result.Message);
            }

            return ServiceResult<IEnumerable<CampaignCategoryModel>>.Ok(result.Data, result.Message);
        }

        private static string? GetOwnerId(string? accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return null;
            }

            var tokenParts = accessToken.Split('.');
            if (tokenParts.Length < 2)
            {
                return null;
            }

            try
            {
                var payload = tokenParts[1]
                    .Replace('-', '+')
                    .Replace('_', '/');
                payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

                using var document = System.Text.Json.JsonDocument.Parse(Convert.FromBase64String(payload));
                if (document.RootElement.TryGetProperty("sub", out var subjectClaim))
                {
                    return subjectClaim.GetString();
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        private static int GetMetadataValue(JsonElement? metadata, string propertyName, int fallback)
        {
            if (metadata is not { ValueKind: JsonValueKind.Object } metadataValue ||
                !metadataValue.TryGetProperty(propertyName, out var property) ||
                property.ValueKind != JsonValueKind.Number ||
                !property.TryGetInt32(out var value))
            {
                return fallback;
            }

            return value;
        }
    }
}
