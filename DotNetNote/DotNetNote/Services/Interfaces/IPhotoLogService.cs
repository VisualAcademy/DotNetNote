namespace Azunt.Web.Services.Interfaces;

using Azunt.Web.Models;

public interface IPhotoLogService
{
    Task<List<PhotoLog>> GetPagedAsync(int pageNumber, int pageSize);

    Task<int> GetCountAsync();

    Task<List<PhotoLog>> GetPagedByEmployeeIdAsync(long employeeId, int pageNumber, int pageSize);

    Task<int> GetCountByEmployeeIdAsync(long employeeId);

    Task<PhotoLog?> GetByIdAsync(long photoLogId);

    Task<bool> DeleteAsync(long photoLogId);
}