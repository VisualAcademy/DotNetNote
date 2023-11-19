namespace DotNetNote.Models;

/// <summary>
/// 리포지토리 인터페이스
/// </summary>
public interface IAttendeeRepository
{
    List<Attendee> GetAll();

    void Add(Attendee model);

    void Delete(Attendee model);
}
