public class AppKeyConfig
{
    public bool AzureStorageEnable { get; set; }

    public string AzureStorageContainer { get; set; } = string.Empty;
    public string AzureStorageAccount { get; set; } = string.Empty;
    public string AzureStorageAccessKey { get; set; } = string.Empty;
    public string SendGridKey { get; set; } = string.Empty;
}