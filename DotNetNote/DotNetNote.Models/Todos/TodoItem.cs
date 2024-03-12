// 'DotNetNote.Models' 네임스페이스는 이 애플리케이션의 모델 클래스들을 포함하고 있습니다.
namespace DotNetNote.Models;

/// <summary>
/// 'TodoItem' 클래스는 하나의 TODO 항목을 표현하는 클래스입니다.
/// 각각의 TODO 항목은 고유의 ID, 제목, 그리고 완료 여부를 가지고 있습니다.
/// </summary>
public class TodoItem
{
    // 'Id' 프로퍼티는 각 TODO 항목의 고유 식별자입니다. 
    // 일반적으로 이 값은 데이터베이스에서 자동으로 생성됩니다.
    public int Id { get; set; }

    // 'Title' 프로퍼티는 TODO 항목의 제목을 나타냅니다. 
    // 이 값은 사용자에게 보여지며, TODO 항목의 내용을 요약하는데 사용됩니다.
    public string Title { get; set; }

    // 'IsDone' 프로퍼티는 TODO 항목의 완료 여부를 나타냅니다. 
    // 만약 이 값이 'true'라면, 해당 TODO 항목은 이미 완료된 것으로 간주됩니다.
    public bool IsDone { get; set; }
}
