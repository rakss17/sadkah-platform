using Microsoft.AspNetCore.DataProtection;
using Sadkah.Web.Models;
using Sadkah.Web.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("SadkahApi", client =>
{
    var apiBaseUrl = builder.Configuration["Api:BaseUrl"]
        ?? throw new InvalidOperationException("Missing Api:BaseUrl configuration value.");

    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddScoped<IAuthSessionService, AuthSessionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<IDonationService, DonationService>();
builder.Services.AddScoped<IOcrService, OcrService>();
builder.Services.AddSingleton<ILocationService, LocationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapPost("/auth/session", (AuthResult authResult, HttpContext httpContext, IDataProtectionProvider dataProtectionProvider) =>
{
    if (string.IsNullOrWhiteSpace(authResult.AccessToken))
    {
        return Results.BadRequest();
    }

    var protector = dataProtectionProvider.CreateProtector(AuthSessionService.ProtectorPurpose);
    var protectedSession = protector.Protect(JsonSerializer.Serialize(authResult));

    httpContext.Response.Cookies.Append(AuthSessionService.CookieName, protectedSession, new CookieOptions
    {
        HttpOnly = true,
        Secure = httpContext.Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.AddDays(7)
    });

    return Results.NoContent();
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
