namespace Sadkah.Web.Pages.Campaigns
{
    public partial class MakeDonation
    {
        [Parameter]
        public Guid CampaignId { get; set; }

        private string CampaignTitle { get; set; } = "Sample Campaign Title";

        private string BuildBreadcrumb(
            Breadcrumb.BreadcrumbItem crumb,
            int index,
            List<Breadcrumb.BreadcrumbItem> breadcrumbs)
        {
            return string.Equals(crumb.Segment, CampaignId.ToString(), StringComparison.OrdinalIgnoreCase)
                ? CampaignTitle
                : crumb.Title;
        }
    }
}