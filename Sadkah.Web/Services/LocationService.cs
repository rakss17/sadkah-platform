using Microsoft.AspNetCore.Hosting;
using System.Text.Json.Serialization;

namespace Sadkah.Web.Services
{
    public sealed class LocationService : ILocationService
    {
        private readonly Lazy<List<ProvinceRecord>> provinces;
        private readonly Lazy<List<CityRecord>> cities;

        public LocationService(IWebHostEnvironment webHostEnvironment)
        {
            provinces = new Lazy<List<ProvinceRecord>>(() =>
                LoadJson<ProvinceRecord>(Path.Combine(webHostEnvironment.WebRootPath, "data", "ph-provinces.json")));

            cities = new Lazy<List<CityRecord>>(() =>
                LoadJson<CityRecord>(Path.Combine(webHostEnvironment.WebRootPath, "data", "ph-cities.json")));
        }

        public IReadOnlyList<string> GetProvinces() =>
            provinces.Value
                .Select(province => province.ProvinceName)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();

        public IReadOnlyList<string> GetCities(string? province)
        {
            if (string.IsNullOrWhiteSpace(province))
            {
                return Array.Empty<string>();
            }

            var provinceCode = provinces.Value
                .FirstOrDefault(p => string.Equals(p.ProvinceName, province, StringComparison.OrdinalIgnoreCase))
                ?.ProvinceCode;

            if (provinceCode is null)
            {
                return Array.Empty<string>();
            }

            return cities.Value
                .Where(city => city.ProvinceCode == provinceCode)
                .Select(city => city.CityName)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static List<T> LoadJson<T>(string path)
        {
            using var stream = File.OpenRead(path);
            return JsonSerializer.Deserialize<List<T>>(stream) ?? new List<T>();
        }

        private sealed record ProvinceRecord(
            [property: JsonPropertyName("province_code")] string ProvinceCode,
            [property: JsonPropertyName("province_name")] string ProvinceName);

        private sealed record CityRecord(
            [property: JsonPropertyName("city_name")] string CityName,
            [property: JsonPropertyName("province_code")] string ProvinceCode);
    }
}
