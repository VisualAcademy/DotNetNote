namespace Azunt.Web.Models;

public class PhotoLog
{
    public long Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public long? EmployeeId { get; set; }
}