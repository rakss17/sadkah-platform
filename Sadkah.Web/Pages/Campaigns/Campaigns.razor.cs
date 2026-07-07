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
        private int totalItems;
        private int totalPages;
        private int pageSize = 10;
        private bool isLoading = true;
        private string? statusMessage;
        private string searchTerm = string.Empty;
        private bool showFilters;
        private int currentPage = 1;

        private const string VerifiedFilterValue = "verified";
        private const string UnverifiedFilterValue = "unverified";

        private IEnumerable<CampaignCategoryModel> _campaignCategories = new List<CampaignCategoryModel>();

        private string selectedCategory = string.Empty;
        private string selectedLocation = string.Empty;
        private string verificationFilter = string.Empty;

        private IEnumerable<CampaignSummary> PagedCampaigns => campaigns;

        private int TotalItems => totalItems;

        private int TotalPages => Math.Max(1, totalPages);

        private int CurrentPage => Math.Clamp(currentPage, 1, TotalPages);

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
                var result = await CampaignService.GetCampaignsAsync(
                    pageNumber: currentPage,
                    pageSize: pageSize,
                    searchTerm: searchTerm,
                    category: selectedCategory,
                    location: selectedLocation,
                    isVerified: GetVerificationFilterValue(),
                    mineOnly: selectedFilter == CampaignFilter.MyCampaigns);

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

                campaigns = result.Data.Items;
                totalItems = result.Data.TotalCount;
                totalPages = result.Data.TotalPages;
                currentPage = result.Data.CurrentPage;
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

        private async Task SelectFilter(CampaignFilter filter)
        {
            if (selectedFilter == filter)
            {
                return;
            }

            selectedFilter = filter;
            ResetPagination();
            await LoadCampaignsAsync();
        }

        private void ToggleFilters()
        {
            showFilters = !showFilters;
        }

        private async Task ClearFilters()
        {
            selectedCategory = string.Empty;
            selectedLocation = string.Empty;
            verificationFilter = string.Empty;
            ResetPagination();
            await LoadCampaignsAsync();
        }

        private async Task SearchCampaignsAsync(string? value)
        {
            searchTerm = value ?? string.Empty;
            ResetPagination();
            await LoadCampaignsAsync();
        }

        private async Task ChangePage(int page)
        {
            currentPage = Math.Clamp(page, 1, TotalPages);
            await LoadCampaignsAsync();
        }

        private async Task ChangePageSize(int newPageSize)
        {
            pageSize = Math.Max(1, newPageSize);
            ResetPagination();
            await LoadCampaignsAsync();
        }

        private async Task ChangeCategoryAsync(string? value)
        {
            selectedCategory = value ?? string.Empty;
            ResetPagination();
            await LoadCampaignsAsync();
        }

        private async Task ChangeLocationAsync(string? value)
        {
            selectedLocation = value ?? string.Empty;
            ResetPagination();
            await LoadCampaignsAsync();
        }

        private async Task ChangeVerificationFilterAsync(string? value)
        {
            verificationFilter = value ?? string.Empty;
            ResetPagination();
            await LoadCampaignsAsync();
        }

        private void CreateCampaign()
        {
            Navigation.NavigateTo("/campaigns/create");
        }

        private void DonateToCampaign(Guid campaignId)
        {
            Navigation.NavigateTo($"/campaigns/{campaignId}/donate");
        }

        private string GetFilterTabClass(CampaignFilter filter)
        {
            return IsSelected(filter)
                ? "filter-tab-button filter-tab-button--active"
                : "filter-tab-button";
        }

        private bool? GetVerificationFilterValue()
        {
            return verificationFilter switch
            {
                VerifiedFilterValue => true,
                UnverifiedFilterValue => false,
                _ => null
            };
        }

        private void ResetPagination()
        {
            currentPage = 1;
        }
    }
}
