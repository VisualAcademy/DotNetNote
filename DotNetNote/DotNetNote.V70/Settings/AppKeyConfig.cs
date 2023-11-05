namespace DotNetNote.Settings;

public class AppKeyConfig
{
    public bool AzureStorageEnable { get; set; }
    public string AzureStorageContainer { get; set; }
    public string AzureStorageAccount { get; set; }
    public string AzureStorageAccessKey { get; set; }
    public string SendGridKey { get; set; }
}
