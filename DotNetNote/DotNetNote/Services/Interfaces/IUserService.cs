namespace DotNetNote.Services.Interfaces;

public interface IUserService
{
    ApplicationUser GetUser();

    ApplicationUser GetUserNotCached();

    ApplicationUser GetUser(string email);

    ApplicationUser GetUserById(string id);

    ApplicationUser GetUserByIdCached(string id);

    Task<ApplicationUser> GetUserAsync();

    Task<ApplicationUser> GetUserAsyncCached();

    Task<ApplicationUser> GetUserAsyncWithRoleInfo();

    Task<TimeZoneInfo> GetTimeZoneInfoAsync();

    void ConfirmEmail(string email);

    bool HasAccessTo(string packageFeature);

    Task RemoveUserCacheAsync();
}
