namespace Sadkah.Web.Components.Authentication
{
    public partial class CheckAuth
    {
        [Inject]
        private IAuthSessionService AuthSession { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public AuthGuardMode Mode { get; set; } = AuthGuardMode.RequireAuthenticated;

        private bool hasCheckedAuthentication;
        private bool shouldRenderChildContent;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            var isAuthenticated = await AuthSession.IsAuthenticatedAsync();

            if (Mode == AuthGuardMode.RequireAuthenticated && !isAuthenticated)
            {
                Navigation.NavigateTo("/login", replace: true);
                return;
            }

            if (Mode == AuthGuardMode.RedirectAuthenticated && isAuthenticated)
            {
                Navigation.NavigateTo("/dashboard", replace: true);
                return;
            }

            hasCheckedAuthentication = true;
            shouldRenderChildContent = true;
            StateHasChanged();
        }
    }
}
