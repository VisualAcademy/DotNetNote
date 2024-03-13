namespace DotNetNote.Models;

public interface IUrlRepository
{
    bool IsExists(string email);
}