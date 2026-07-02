using Microsoft.AspNetCore.Components.Forms;

namespace Sadkah.Web.Pages.Campaigns
{
    public partial class CreateCampaign
    {
        [Inject]
        private ICampaignService CampaignService { get; set; } = default!;

        [Inject]
        private ILocationService LocationService { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private CampaignModel campaignModel = new();
        private IEnumerable<CampaignCategoryModel> _campaignCategories = new List<CampaignCategoryModel>();
        private IReadOnlyList<string> _countries = Array.Empty<string>();
        private IReadOnlyList<string> _provinces = Array.Empty<string>();
        private bool isPublishing;
        private string? statusMessage;
        private string statusAlertClass = "create-alert--error";

        private IReadOnlyList<string> Cities => LocationService.GetCities(campaignModel.Province);

        protected override async Task OnInitializedAsync()
        {
            _provinces = LocationService.GetProvinces();

            var result = await CampaignService.GetCampaignCategoriesAsync();

            if (result.RequiresAuthentication)
            {
                Navigation.NavigateTo("/login", replace: true);
                return;
            }

            if (!result.Success)
            {
                statusMessage = result.Message;
                statusAlertClass = "create-alert--error";
                return;
            }

            _campaignCategories = result.Data ?? new List<CampaignCategoryModel>();
        }

        private void HandleProvinceChanged()
        {
            if (!Cities.Contains(campaignModel.City, StringComparer.OrdinalIgnoreCase))
            {
                campaignModel.City = string.Empty;
            }
        }

        private void HandleInvalidSubmit(EditContext editContext)
        {
            statusMessage = "Please fix the highlighted fields before publishing.";
            statusAlertClass = "create-alert--error";
        }

        private async Task HandlePublishCampaignAsync()
        {
            isPublishing = true;
            statusMessage = null;
            statusAlertClass = "create-alert--error";

            try
            {
                var result = await CampaignService.CreateCampaignAsync(campaignModel);

                if (result.RequiresAuthentication)
                {
                    Navigation.NavigateTo("/login", replace: true);
                    return;
                }

                if (!result.Success)
                {
                    statusMessage = result.Message;
                    return;
                }

                Navigation.NavigateTo("/campaigns");
            }
            finally
            {
                isPublishing = false;
            }
        }
    }
}
