namespace Sadkah.Web.Models
{
    public sealed record CampaignSummary(
        Guid Id,
        string Title,
        string Description,
        string Category,
        string Location,
        int DaysLeft,
        decimal RaisedAmount,
        decimal GoalAmount,
        bool IsVerified,
        bool IsMine,
        DonationMethodResult[] DonationMethods);
}
