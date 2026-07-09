using Microsoft.AspNetCore.Components.Forms;

namespace Sadkah.Web.Pages.Campaigns
{
    public partial class MakeDonation
    {
        [Parameter]
        public Guid CampaignId { get; set; }

        [Inject]
        private ICampaignService CampaignService { get; set; } = default!;

        [Inject]
        private IDonationService DonationService { get; set; } = default!;

        [Inject]
        private IAuthSessionService AuthSessionService { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IOcrService OcrService { get; set; } = default!;

        private DonationModel donationModel = new();
        private EditContext? _editContext;

        private CampaignSummary? _campaign { get; set; }

        private IEnumerable<DonationMethodResult> _donationMethods = new List<DonationMethodResult>();

        private Guid _selectedDonationMethod;

        private string _currentUserFullName = string.Empty;
        private string _currentUserEmail = string.Empty;
        private string _qrCodeImagePreviewUrl = string.Empty;
        private string _receiptImagePreviewUrl = string.Empty;
        private string? _statusMessage;

        private bool _isLoading = true;
        private bool _isUploadingReceipt;
        private bool _isQrImageOpen;
        private bool _isReceiptImageOpen;
        private bool _isReceiptUploaded;
        private bool _isSubmittingDonation;
        private bool IsReceiptImageInvalid =>
            _editContext is not null &&
            _editContext.GetValidationMessages(_editContext.Field(nameof(DonationModel.ReceiptImageFile))).Any();

        private const long MaxDonationImageSizeBytes = DonationModel.MaxQrImageSizeBytes;
        private static readonly string[] AllowedDonationImageContentTypes = { "image/jpeg", "image/png" };

        protected override void OnInitialized()
        {
            _editContext = new EditContext(donationModel);
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadCampaignsAsync();

            _currentUserFullName = await AuthSessionService.GetCurrentUserFullNameAsync() ?? string.Empty;
            _currentUserEmail = await AuthSessionService.GetCurrentUserEmailAsync() ?? string.Empty;

            donationModel.CampaignId = CampaignId;
        }


        private async Task LoadCampaignsAsync()
        {
            _isLoading = true;
            _statusMessage = null;

            try
            {
                var result = await CampaignService.GetCampaignByIdAsync(CampaignId);

                if (result.RequiresAuthentication)
                {
                    Navigation.NavigateTo("/login", replace: true);
                    return;
                }

                if (!result.Success || result.Data is null)
                {
                    _statusMessage = result.Message;
                    return;
                }

                _campaign = result.Data;
                _donationMethods = result.Data.DonationMethods;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task HandleSubmitDonationAsync()
        {
            _isSubmittingDonation = true;
            _statusMessage = null;

            try
            {
                var result = await DonationService.CreateDonationAsync(donationModel);

                if (result.RequiresAuthentication)
                {
                    Navigation.NavigateTo("/login", replace: true);
                    return;
                }

                if (!result.Success)
                {
                    _statusMessage = result.Message;
                    return;
                }

                Navigation.NavigateTo("/campaigns");
            }
            finally
            {
                _isSubmittingDonation = false;
            }
        }
        

        private async Task HandleReceiptImageUploadAsync(InputFileChangeEventArgs e, DonationModel donation)
        {
            var file = e.File;
            _isUploadingReceipt = true;
            donation.UploadError = null;

            if (file.Size > MaxDonationImageSizeBytes)
            {
                donation.UploadError = "Image must be 2MB or smaller.";
                return;
            }

            if (!AllowedDonationImageContentTypes.Contains(file.ContentType))
            {
                donation.UploadError = "Only JPG and PNG images are allowed.";
                return;
            }

            try
            {
                await using var stream = file.OpenReadStream(maxAllowedSize: MaxDonationImageSizeBytes);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);

                var imageBytes = memoryStream.ToArray();
                donation.ReceiptImageBytes = imageBytes;
                donation.ReceiptImageFile = file;
                _receiptImagePreviewUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(imageBytes)}";

                await ExtractReceiptAsync();
            }
            catch (Exception)
            {
                donation.UploadError = "Failed to read the selected image. Please try again.";
            }
            finally
            {
                _isUploadingReceipt = false;
                _isReceiptUploaded = true;
            }
        }

        private async Task ExtractReceiptAsync()
        {
            if (donationModel.ReceiptImageBytes is null || donationModel.ReceiptImageFile is null)
            {
                donationModel.UploadError = "Please upload a receipt image first.";
                return;
            }

            donationModel.UploadError = null;

            try
            {
                var result = await OcrService.ExtractReceiptTextAsync(
                    donationModel.ReceiptImageBytes,
                    donationModel.ReceiptImageFile.Name,
                    donationModel.ReceiptImageFile.ContentType);

                if (!result.Success || result.Data is null)
                {
                    donationModel.UploadError = string.IsNullOrWhiteSpace(result.Message)
                        ? "Could not read the receipt. Please try again."
                        : result.Message;
                    return;
                }

                if (result.Data.Amount.HasValue)
                {
                    donationModel.Amount = result.Data.Amount.Value;
                }

                if (!string.IsNullOrWhiteSpace(result.Data.ReferenceNumber))
                {
                    donationModel.PaymentReference = result.Data.ReferenceNumber;
                }
            }
            finally {}
        }

        private void HandleInvalidSubmit(EditContext editContext)
        {
            _statusMessage = "Please fix the highlighted fields before publishing.";
        }

        private void OnDonationMethodSelected()
        {
            var donationMethod = _donationMethods.SingleOrDefault(x => x.Id == _selectedDonationMethod);
            _qrCodeImagePreviewUrl = donationMethod?.QrImageUrl ?? string.Empty;
            donationModel.Method = donationMethod?.Provider ?? string.Empty;
        }

        private void RemoveReceiptImage(DonationModel donation)
        {
            donation.ReceiptImageFile = null!;
            donation.ReceiptImageBytes = [];
            donation.UploadError = null;
            donation.Amount = 0;
            donation.PaymentReference = string.Empty;
            _receiptImagePreviewUrl = string.Empty;
            _isReceiptUploaded = false;
        }

        private void ShowQrImage()
        {
            _isQrImageOpen = true;
        }

        private void CloseQrImage()
        {
            _isQrImageOpen = false;
        }

        private void ShowReceiptImage()
        {
            _isReceiptImageOpen = true;
        }

        private void CloseReceiptImage()
        {
            _isReceiptImageOpen = false;
        }

        private string BuildBreadcrumb(
            Breadcrumb.BreadcrumbItem crumb,
            int index,
            List<Breadcrumb.BreadcrumbItem> breadcrumbs)
        {
            return string.Equals(crumb.Segment, CampaignId.ToString(), StringComparison.OrdinalIgnoreCase)
                ? _campaign?.Title ?? "Unknown Campaign"
                : crumb.Title;
        }
    }
}