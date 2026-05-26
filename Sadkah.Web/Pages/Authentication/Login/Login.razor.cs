using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Sadkah.Web.Models;

namespace Sadkah.Web.Pages.Authentication.Login
{
    public partial class Login
    {
        [Inject]
        private IHttpClientFactory HttpClientFactory { get; set; } = default!;

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = default!;

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
                var client = HttpClientFactory.CreateClient("SadkahApi");
                var response = await client.PostAsJsonAsync("api/user/login", loginModel);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResult>>();

                if (!response.IsSuccessStatusCode || apiResponse is not { Success: true, Data: not null })
                {
                    statusMessage = apiResponse?.Message ?? "Unable to sign in. Please check your email and password.";
                    return;
                }

                await JsRuntime.InvokeVoidAsync("localStorage.setItem", "sadkah_access_token", apiResponse.Data.AccessToken);
                await JsRuntime.InvokeVoidAsync("localStorage.setItem", "sadkah_refresh_token", apiResponse.Data.RefreshToken);
                await JsRuntime.InvokeVoidAsync("localStorage.setItem", "sadkah_user_email", apiResponse.Data.Email);
                await JsRuntime.InvokeVoidAsync("localStorage.setItem", "sadkah_user_full_name", apiResponse.Data.FullName);

                statusAlertClass = "alert-success";
                statusMessage = apiResponse.Message;
                Navigation.NavigateTo("/dashboard");
            }
            catch (HttpRequestException)
            {
                statusMessage = "Could not reach the Sadkah API. Make sure Sadkah.API is running.";
            }
            finally
            {
                isSubmitting = false;
            }
        }
    }
}
