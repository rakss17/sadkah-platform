using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Sadkah.Web.Models;

namespace Sadkah.Web.Pages.Campaigns
{
    public partial class Campaigns
    {
        [Inject]
        private IHttpClientFactory HttpClientFactory { get; set; } = default!;

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private CampaignFilter selectedFilter = CampaignFilter.All;
        private List<CampaignCard> campaigns = [];
        private bool isLoading = true;
        private string? statusMessage;
        private string? currentUserFullName;

        private IEnumerable<CampaignCard> FilteredCampaigns => selectedFilter switch
        {
            CampaignFilter.MyCampaigns => campaigns.Where(campaign => campaign.IsMine),
            _ => campaigns
        };

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            await LoadCampaignsAsync();
            StateHasChanged();
        }

        private async Task LoadCampaignsAsync()
        {
            isLoading = true;
            statusMessage = null;

            try
            {
                var token = await JsRuntime.InvokeAsync<string?>("localStorage.getItem", "sadkah_access_token");
                currentUserFullName = await JsRuntime.InvokeAsync<string?>("localStorage.getItem", "sadkah_user_full_name");

                if (string.IsNullOrWhiteSpace(token))
                {
                    Navigation.NavigateTo("/login", replace: true);
                    return;
                }

                var client = HttpClientFactory.CreateClient("SadkahApi");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync("api/campaigns?pageSize=50");
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<CampaignResponse>>>();

                if (!response.IsSuccessStatusCode || apiResponse is not { Success: true, Data: not null })
                {
                    statusMessage = apiResponse?.Message ?? "Unable to load campaigns.";
                    return;
                }

                campaigns = apiResponse.Data.Select(MapCampaign).ToList();
            }
            catch (HttpRequestException)
            {
                statusMessage = "Could not reach the Sadkah API. Make sure Sadkah.API is running.";
            }
            finally
            {
                isLoading = false;
            }
        }

        private bool IsSelected(CampaignFilter filter) => selectedFilter == filter;

        private void SelectFilter(CampaignFilter filter)
        {
            selectedFilter = filter;
        }

        private string GetFilterTabClass(CampaignFilter filter)
        {
            return IsSelected(filter)
                ? "filter-tab-button filter-tab-button--active"
                : "filter-tab-button";
        }

        private enum CampaignFilter
        {
            All,
            MyCampaigns
        }

        private CampaignCard MapCampaign(CampaignResponse campaign)
        {
            return new CampaignCard(
                campaign.Title,
                GetStatusLabel(campaign.Status),
                campaign.OwnerName,
                GetDaysLeft(campaign.Deadline),
                campaign.CurrentAmount,
                campaign.TargetAmount,
                campaign.IsVerified,
                IsCurrentUsersCampaign(campaign.OwnerName));
        }

        private bool IsCurrentUsersCampaign(string ownerName)
        {
            return !string.IsNullOrWhiteSpace(currentUserFullName)
                && string.Equals(ownerName, currentUserFullName, StringComparison.OrdinalIgnoreCase);
        }

        private static int GetDaysLeft(DateTime deadline)
        {
            return Math.Max(0, (deadline.Date - DateTime.Today).Days);
        }

        private static string GetStatusLabel(int status)
        {
            return status switch
            {
                0 => "Pending",
                1 => "Active",
                2 => "Completed",
                3 => "Rejected",
                _ => "Campaign"
            };
        }

        private sealed record CampaignCard(
            string Title,
            string Category,
            string Location,
            int DaysLeft,
            decimal RaisedAmount,
            decimal GoalAmount,
            bool IsVerified,
            bool IsMine);

        private sealed class CampaignResponse
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public decimal TargetAmount { get; set; }
            public decimal CurrentAmount { get; set; }
            public DateTime Deadline { get; set; }
            public int Status { get; set; }
            public bool IsVerified { get; set; }
            public string OwnerName { get; set; } = string.Empty;
        }
    }
}
