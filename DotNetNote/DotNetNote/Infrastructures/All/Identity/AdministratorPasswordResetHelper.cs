namespace DotNetNote.Infrastructures.All.Identity;

public class AdministratorPasswordResetHelper
{
    /// <summary>
    /// 관리자 계정 암호를 초기화하는 메서드
    /// </summary>
    public static async Task ResetAdministratorPassword(IServiceProvider serviceProvider, string domainName = "hawaso.com")
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // 관리자 계정 검색
        var administrator = await userManager.FindByEmailAsync($"administrator@{domainName}");
        if (administrator != null)
        {
            // 암호 재설정을 위한 토큰 생성 및 암호 재설정
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(administrator);
            var resetResult = await userManager.ResetPasswordAsync(administrator, resetToken, "Pa$$w0rd");

            if (!resetResult.Succeeded)
            {
                // 에러 발생 시 예외 처리
                throw new Exception("Failed to reset administrator password: " +
                                    string.Join(", ", resetResult.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            throw new Exception($"Administrator account with email administrator@{domainName} not found.");
        }
    }
}
