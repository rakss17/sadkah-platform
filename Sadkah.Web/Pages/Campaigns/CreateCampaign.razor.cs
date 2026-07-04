using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace Sadkah.Web.Pages.Campaigns
{
    public partial class CreateCampaign : IDisposable
    {
        [Inject]
        private ICampaignService CampaignService { get; set; } = default!;

        [Inject]
        private ILocationService LocationService { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private CampaignModel campaignModel = new();
        private EditContext editContext = default!;
        private ValidationMessageStore? _donationMethodMessageStore;
        private IEnumerable<CampaignCategoryModel> _campaignCategories = new List<CampaignCategoryModel>();
        private IReadOnlyList<string> _countries = Array.Empty<string>();
        private IReadOnlyList<string> _provinces = Array.Empty<string>();
        private bool isPublishing;
        private string? statusMessage;
        private string statusAlertClass = "create-alert--error";

        private IReadOnlyList<string> Cities => LocationService.GetCities(campaignModel.Province);

        private const long MaxDonationImageSizeBytes = 2 * 1024 * 1024;
        private static readonly string[] AllowedDonationImageContentTypes = { "image/jpeg", "image/png" };

        private static readonly Dictionary<DonationMethodType, string[]> DonationProviderOptions = new()
        {
            [DonationMethodType.OnlineBanking] = new[] { "BDO", "BPI", "Metrobank", "Landbank", "PNB", "UnionBank", "Security Bank", "RCBC" },
            [DonationMethodType.EWallet] = new[] { "GCash", "Maya", "GrabPay", "ShopeePay", "Coins.ph" }
        };

        protected override void OnInitialized()
        {
            editContext = new EditContext(campaignModel);
            _donationMethodMessageStore = new ValidationMessageStore(editContext);
            editContext.OnValidationRequested += HandleDonationMethodsValidationRequested;
            editContext.OnFieldChanged += HandleDonationMethodFieldChanged;
        }

        protected override async Task OnInitializedAsync()
        {
            _provinces = LocationService.GetProvinces();

            var result = await CampaignService.GetCampaignCategoriesAsync();

            if (result.RequiresAuthentication)
            {
                Navigation.NavigateTo("/login", replace: true);
                return;
            }

            if (!result.Success)
            {
                statusMessage = result.Message;
                statusAlertClass = "create-alert--error";
                return;
            }

            _campaignCategories = result.Data ?? new List<CampaignCategoryModel>();
        }

        private void HandleProvinceChanged()
        {
            if (!Cities.Contains(campaignModel.City, StringComparer.OrdinalIgnoreCase))
            {
                campaignModel.City = string.Empty;
            }
        }

        private IReadOnlyList<string> GetDonationProviderOptions(DonationMethodType? type)
        {
            if (type is null || !DonationProviderOptions.TryGetValue(type.Value, out var options))
            {
                return Array.Empty<string>();
            }

            return options;
        }

        private void AddDonationMethod()
        {
            campaignModel.DonationMethods.Add(new DonationMethodModel());
        }

        private void RemoveDonationMethod(DonationMethodModel method)
        {
            campaignModel.DonationMethods.Remove(method);
        }

        private void HandleDonationMethodTypeChanged(DonationMethodModel method)
        {
            if (!GetDonationProviderOptions(method.Type).Contains(method.Provider))
            {
                method.Provider = string.Empty;
            }
        }

        private void RemoveDonationMethodImage(DonationMethodModel method)
        {
            method.QrImageFileName = null;
            method.QrImageDataUrl = null;
            method.UploadError = null;
        }

        private void HandleDonationMethodFieldChanged(object? sender, FieldChangedEventArgs e)
        {
            if (campaignModel.DonationMethods.Any(method => ReferenceEquals(method, e.FieldIdentifier.Model)))
            {
                ValidateDonationMethods();
            }
        }

        private void HandleDonationMethodsValidationRequested(object? sender, ValidationRequestedEventArgs e)
        {
            ValidateDonationMethods();
        }

        private void ValidateDonationMethods()
        {
            _donationMethodMessageStore?.Clear();

            foreach (var method in campaignModel.DonationMethods)
            {
                var results = new List<ValidationResult>();
                Validator.TryValidateObject(method, new ValidationContext(method), results, validateAllProperties: true);

                foreach (var result in results)
                {
                    foreach (var memberName in result.MemberNames)
                    {
                        _donationMethodMessageStore?.Add(new FieldIdentifier(method, memberName), result.ErrorMessage ?? string.Empty);
                    }
                }
            }

            editContext.NotifyValidationStateChanged();
        }

        private async Task HandleDonationImageUploadAsync(InputFileChangeEventArgs e, DonationMethodModel method)
        {
            var file = e.File;
            method.UploadError = null;

            if (file.Size > MaxDonationImageSizeBytes)
            {
                method.UploadError = "Image must be 2MB or smaller.";
                return;
            }

            if (!AllowedDonationImageContentTypes.Contains(file.ContentType))
            {
                method.UploadError = "Only JPG or PNG images are allowed.";
                return;
            }

            try
            {
                using var stream = file.OpenReadStream(MaxDonationImageSizeBytes);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);

                method.QrImageFileName = file.Name;
                method.QrImageDataUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(memoryStream.ToArray())}";
            }
            catch (IOException)
            {
                method.UploadError = "Image must be 2MB or smaller.";
            }
        }

        private void HandleInvalidSubmit(EditContext editContext)
        {
            statusMessage = "Please fix the highlighted fields before publishing.";
            statusAlertClass = "create-alert--error";
        }

        private async Task HandlePublishCampaignAsync()
        {
            Console.WriteLine("campaignModel: " + JsonSerializer.Serialize(campaignModel));
            // isPublishing = true;
            // statusMessage = null;
            // statusAlertClass = "create-alert--error";

            // try
            // {
            //     var result = await CampaignService.CreateCampaignAsync(campaignModel);

            //     if (result.RequiresAuthentication)
            //     {
            //         Navigation.NavigateTo("/login", replace: true);
            //         return;
            //     }

            //     if (!result.Success)
            //     {
            //         statusMessage = result.Message;
            //         return;
            //     }

            //     Navigation.NavigateTo("/campaigns");
            // }
            // finally
            // {
            //     isPublishing = false;
            // }
        }

        public void Dispose()
        {
            editContext.OnValidationRequested -= HandleDonationMethodsValidationRequested;
            editContext.OnFieldChanged -= HandleDonationMethodFieldChanged;
        }
    }
}
