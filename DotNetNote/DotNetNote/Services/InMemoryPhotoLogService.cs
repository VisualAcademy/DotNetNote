namespace Azunt.Web.Services;

using Azunt.Web.Models;
using Azunt.Web.Services.Interfaces;

public class InMemoryPhotoLogService : IPhotoLogService
{
    private readonly List<PhotoLog> _photoLogs = new()
    {
        new PhotoLog { Id = 1, FileName = "employee1_photo_001.jpg", EmployeeId = 1 },
        new PhotoLog { Id = 2, FileName = "employee1_photo_002.jpg", EmployeeId = 1 },
        new PhotoLog { Id = 3, FileName = "employee1_photo_003.jpg", EmployeeId = 1 },
        new PhotoLog { Id = 4, FileName = "employee2_photo_001.jpg", EmployeeId = 2 },
        new PhotoLog { Id = 5, FileName = "employee2_photo_002.jpg", EmployeeId = 2 },
        new PhotoLog { Id = 6, FileName = "employee3_photo_001.jpg", EmployeeId = 3 },
        new PhotoLog { Id = 7, FileName = "employee3_photo_002.jpg", EmployeeId = 3 },
        new PhotoLog { Id = 8, FileName = "employee4_photo_001.jpg", EmployeeId = 4 },
        new PhotoLog { Id = 9, FileName = "employee5_photo_001.jpg", EmployeeId = 5 },
        new PhotoLog { Id = 10, FileName = "employee5_photo_002.jpg", EmployeeId = 5 },
        new PhotoLog { Id = 11, FileName = "employee5_photo_003.jpg", EmployeeId = 5 },
        new PhotoLog { Id = 12, FileName = "employee6_photo_001.jpg", EmployeeId = 6 }
    };

    public Task<List<PhotoLog>> GetPagedAsync(int pageNumber, int pageSize)
    {
        pageNumber = NormalizePageNumber(pageNumber);
        pageSize = NormalizePageSize(pageSize);

        var result = _photoLogs
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<int> GetCountAsync()
    {
        return Task.FromResult(_photoLogs.Count);
    }

    public Task<List<PhotoLog>> GetPagedByEmployeeIdAsync(long employeeId, int pageNumber, int pageSize)
    {
        pageNumber = NormalizePageNumber(pageNumber);
        pageSize = NormalizePageSize(pageSize);

        var result = _photoLogs
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<int> GetCountByEmployeeIdAsync(long employeeId)
    {
        var count = _photoLogs.Count(x => x.EmployeeId == employeeId);
        return Task.FromResult(count);
    }

    public Task<PhotoLog?> GetByIdAsync(long photoLogId)
    {
        var result = _photoLogs.FirstOrDefault(x => x.Id == photoLogId);
        return Task.FromResult(result);
    }

    public Task<bool> DeleteAsync(long photoLogId)
    {
        var item = _photoLogs.FirstOrDefault(x => x.Id == photoLogId);

        if (item == null)
        {
            return Task.FromResult(false);
        }

        _photoLogs.Remove(item);
        return Task.FromResult(true);
    }

    private static int NormalizePageNumber(int pageNumber)
    {
        return pageNumber < 1 ? 1 : pageNumber;
    }

    private static int NormalizePageSize(int pageSize)
    {
        return pageSize < 1 ? 10 : pageSize;
    }
}