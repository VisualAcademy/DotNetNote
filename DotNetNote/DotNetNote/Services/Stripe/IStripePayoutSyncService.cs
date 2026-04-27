namespace Azunt.Web.Services.Stripe;

public interface IStripePayoutSyncService
{
    Task<StripePayoutSyncResult> SyncRecentAsync(
        string tenantId,
        CancellationToken cancellationToken = default);
}

public sealed class StripePayoutSyncResult
{
    public int PayoutCount { get; set; }
    public int TransactionCount { get; set; }
    public string? ErrorMessage { get; set; }
}
