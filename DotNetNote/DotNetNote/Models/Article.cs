namespace DotNetNote.Models;

public class Article
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public DateTime PostDate { get; set; }
}
