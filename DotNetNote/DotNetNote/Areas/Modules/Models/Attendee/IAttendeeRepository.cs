namespace AttendeeApp.Models;

public interface IAttendeeRepository
{
    List<Attendee> GetAll();
    void Add(Attendee model);
    void Delete(Attendee model);
}
