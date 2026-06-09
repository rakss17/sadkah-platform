namespace Sadkah.Web.Pages.Campaigns
{
    public partial class Campaigns
    {
        [Inject]
        private ICampaignService CampaignService { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private CampaignFilter selectedFilter = CampaignFilter.All;
        private IReadOnlyList<CampaignSummary> campaigns = [];
        private bool isLoading = true;
        private string? statusMessage;
        private string searchTerm = string.Empty;

        private IEnumerable<CampaignSummary> FilteredCampaigns => selectedFilter switch
        {
            CampaignFilter.MyCampaigns => campaigns.Where(campaign => campaign.IsMine),
            _ => campaigns
        };

        protected override async Task OnInitializedAsync()
        {
            await LoadCampaignsAsync();
        }

        private async Task LoadCampaignsAsync()
        {
            isLoading = true;
            statusMessage = null;

            try
            {
                var result = await CampaignService.GetCampaignsAsync(searchTerm: searchTerm);

                if (result.RequiresAuthentication)
                {
                    Navigation.NavigateTo("/login", replace: true);
                    return;
                }

                if (!result.Success || result.Data is null)
                {
                    statusMessage = result.Message;
                    return;
                }

                campaigns = result.Data;
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

        private async Task SearchCampaignsAsync(string? value)
        {
            searchTerm = value ?? string.Empty;
            await LoadCampaignsAsync();
        }

        private string GetFilterTabClass(CampaignFilter filter)
        {
            return IsSelected(filter)
                ? "filter-tab-button filter-tab-button--active"
                : "filter-tab-button";
        }
    }
}
