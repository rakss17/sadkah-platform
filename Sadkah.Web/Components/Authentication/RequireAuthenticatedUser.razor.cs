using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Sadkah.Web.Components.Authentication
{
    public partial class RequireAuthenticatedUser
    {
        [Inject]
        private IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        private bool isAuthenticated;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            var token = await JsRuntime.InvokeAsync<string?>("localStorage.getItem", "sadkah_access_token");

            if (string.IsNullOrWhiteSpace(token))
            {
                Navigation.NavigateTo("/login", replace: true);
                return;
            }

            isAuthenticated = true;
            StateHasChanged();
        }
    }
}
