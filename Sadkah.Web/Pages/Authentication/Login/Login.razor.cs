namespace Sadkah.Web.Pages.Authentication.Login
{
    public partial class Login
    {
        [Inject]
        private IAuthService AuthService { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private readonly LoginRequest loginModel = new();
        private bool isSubmitting;
        private string? statusMessage;
        private string statusAlertClass = "alert-danger";

        private async Task HandleLoginAsync()
        {
            isSubmitting = true;
            statusMessage = null;
            statusAlertClass = "alert-danger";

            try
            {
                var result = await AuthService.LoginAsync(loginModel);

                if (!result.Success)
                {
                    statusMessage = result.Message;
                    return;
                }

                statusAlertClass = "alert-success";
                statusMessage = result.Message;
                Navigation.NavigateTo("/dashboard");
            }
            finally
            {
                isSubmitting = false;
            }
        }
    }
}
