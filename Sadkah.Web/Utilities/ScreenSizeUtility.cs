using Microsoft.JSInterop;

namespace Sadkah.Web.Utilities
{
    public static class ScreenSizeUtility
    {
        private const string ModulePath = "./js/screen-size.js";

        public static async ValueTask<ScreenSize> GetScreenSizeAsync(IJSRuntime jsRuntime)
        {
            await using var module = await jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);

            return await module.InvokeAsync<ScreenSize>("getScreenSize");
        }

        public static async ValueTask<bool> IsMobileAsync(IJSRuntime jsRuntime)
        {
            var screenSize = await GetScreenSizeAsync(jsRuntime);

            return screenSize.Width < 768;
        }
    }

    public sealed record ScreenSize(int Width, int Height, string Breakpoint);
}
