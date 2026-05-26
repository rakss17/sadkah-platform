namespace Sadkah.Web.Components.Authentication
{
    public partial class RequireAuthenticatedUser
    {
        [Inject]
        private IAuthSessionService AuthSession { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        private bool hasCheckedAuthentication;
        private bool isAuthenticated;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            hasCheckedAuthentication = true;

            if (!await AuthSession.IsAuthenticatedAsync())
            {
                Navigation.NavigateTo("/login", replace: true);
                return;
            }

            isAuthenticated = true;
            StateHasChanged();
        }
    }
}
