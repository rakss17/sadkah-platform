namespace Sadkah.Web.Interfaces
{
    public interface ILocationService
    {
        IReadOnlyList<string> GetProvinces();
        IReadOnlyList<string> GetCities(string? province);
    }
}
