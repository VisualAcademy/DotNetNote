namespace DotNetNote.Models;

public class Choice
{
    public int Id { get; set; }

    // 선택지 텍스트
    public string ChoiceText { get; set; } = string.Empty;

    public bool IsAnswer { get; set; }

    public bool IsSelected { get; set; }

    // EF Core Navigation Property
    // EF에서 로딩되므로 null-forgiving 사용
    public Question Question { get; set; } = null!;
}
