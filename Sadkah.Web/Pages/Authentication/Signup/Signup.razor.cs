namespace Sadkah.Web.Pages.Authentication.Signup
{
    public partial class Signup
    {
        [Inject]
        private IAuthService AuthService { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private readonly SignupRequest signupModel = new();
        private bool isSubmitting;
        private string? statusMessage;
        private string statusAlertClass = "alert-danger";

        private async Task HandleSignupAsync()
        {
            isSubmitting = true;
            statusMessage = null;
            statusAlertClass = "alert-danger";

            try
            {
                var result = await AuthService.SignupAsync(signupModel);

                if (!result.Success)
                {
                    statusMessage = result.Message;
                    return;
                }

                statusAlertClass = "alert-success";
                statusMessage = result.Message;
                Navigation.NavigateTo("/");
            }
            finally
            {
                isSubmitting = false;
            }
        }
    }
}
