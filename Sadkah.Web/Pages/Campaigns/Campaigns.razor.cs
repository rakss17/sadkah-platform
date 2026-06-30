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
        private bool showFilters;
        private string selectedCategory = string.Empty;
        private string selectedLocation = string.Empty;
        private string verificationFilter = string.Empty;

        private const string VerifiedFilterValue = "verified";
        private const string UnverifiedFilterValue = "unverified";

        private IEnumerable<CampaignCategoryModel> _campaignCategories = new List<CampaignCategoryModel>();

        private IEnumerable<CampaignSummary> FilteredCampaigns => selectedFilter switch
        {
            CampaignFilter.MyCampaigns => ApplyAdvancedFilters(campaigns.Where(campaign => campaign.IsMine)),
            _ => ApplyAdvancedFilters(campaigns)
        };

        private IEnumerable<string> AvailableCategories => campaigns
            .Select(campaign => campaign.Category)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase);

        private IEnumerable<string> AvailableLocations => campaigns
            .Select(campaign => campaign.Location)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase);

        private bool HasAdvancedFilters =>
            !string.IsNullOrWhiteSpace(selectedCategory)
            || !string.IsNullOrWhiteSpace(selectedLocation)
            || !string.IsNullOrWhiteSpace(verificationFilter);

        private int ActiveFilterCount
        {
            get
            {
                var count = 0;

                if (!string.IsNullOrWhiteSpace(selectedCategory))
                {
                    count++;
                }

                if (!string.IsNullOrWhiteSpace(selectedLocation))
                {
                    count++;
                }

                if (!string.IsNullOrWhiteSpace(verificationFilter))
                {
                    count++;
                }

                return count;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadCampaignsAsync();
            await LoadCampaignCategoriesAsync();
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

        private async Task LoadCampaignCategoriesAsync()
        {
            isLoading = true;
            statusMessage = null;

            try
            {
                var result = await CampaignService.GetCampaignCategoriesAsync();

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

                _campaignCategories = result.Data;
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

        private void ToggleFilters()
        {
            showFilters = !showFilters;
        }

        private void ClearFilters()
        {
            selectedCategory = string.Empty;
            selectedLocation = string.Empty;
            verificationFilter = string.Empty;
        }

        private async Task SearchCampaignsAsync(string? value)
        {
            searchTerm = value ?? string.Empty;
            await LoadCampaignsAsync();
        }

        private void CreateCampaign()
        {
            Navigation.NavigateTo("/campaigns/create");
        }

        private string GetFilterTabClass(CampaignFilter filter)
        {
            return IsSelected(filter)
                ? "filter-tab-button filter-tab-button--active"
                : "filter-tab-button";
        }

        private IEnumerable<CampaignSummary> ApplyAdvancedFilters(IEnumerable<CampaignSummary> source)
        {
            var filteredCampaigns = source;

            if (!string.IsNullOrWhiteSpace(selectedCategory))
            {
                filteredCampaigns = filteredCampaigns.Where(campaign =>
                    string.Equals(campaign.Category, selectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(selectedLocation))
            {
                filteredCampaigns = filteredCampaigns.Where(campaign =>
                    string.Equals(campaign.Location, selectedLocation, StringComparison.OrdinalIgnoreCase));
            }

            filteredCampaigns = verificationFilter switch
            {
                VerifiedFilterValue => filteredCampaigns.Where(campaign => campaign.IsVerified),
                UnverifiedFilterValue => filteredCampaigns.Where(campaign => !campaign.IsVerified),
                _ => filteredCampaigns
            };

            return filteredCampaigns;
        }
    }
}
