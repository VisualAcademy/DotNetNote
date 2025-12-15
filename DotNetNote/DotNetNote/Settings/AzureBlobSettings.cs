namespace DotNetNote.Settings;

public class AzureBlobSettings
{
    /// <summary>
    /// Full Azure Storage connection string.
    /// Recommended to keep this in user-secrets or environment variables.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Base path for the anonymous download endpoint.
    /// Default: /api/invoicefiles
    /// </summary>
    public string DownloadApiBasePath { get; set; } = "/api/invoicefiles";
}
