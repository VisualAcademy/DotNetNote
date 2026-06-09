namespace DotNetNote.Models;

public class HttpClientDemoResult
{
    public string Url { get; set; } = "";
    public string ResponseText { get; set; } = "";
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = "";
}