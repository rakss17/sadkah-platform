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

            var request = new CreateCampaignRequest(
                ownerId,
                campaign.Title,
                campaign.Description,
                campaign.CategoryId,
                campaign.TargetAmount,
                campaign.Deadline.GetValueOrDefault(),
                campaign.AddressLine1,
                campaign.Barangay,
                campaign.City,
                campaign.Province,
                campaign.Country,
                campaign.AddressLine2);

            return await apiClient.PostAsync<CreateCampaignRequest, CampaignModel>(
                "api/campaigns",
                request,
                requiresAuthentication: true);
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

        private sealed record CreateCampaignRequest(
            string OwnerId,
            string Title,
            string Description,
            Guid CategoryId,
            decimal TargetAmount,
            DateTime Deadline,
            string AddressLine1,
            string Barangay,
            string City,
            string Province,
            string Country = "Philippines",
            string? AddressLine2 = null);
    }
}
