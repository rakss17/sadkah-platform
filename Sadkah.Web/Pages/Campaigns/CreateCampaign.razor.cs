namespace Sadkah.Web.Pages.Campaigns
{
    public partial class CreateCampaign
    {
        [Inject]
        private ICampaignService CampaignService { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private CampaignModel campaignModel = new();
        private bool isPublishing;
        private string? statusMessage;
        private string statusAlertClass = "create-alert--error";

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
