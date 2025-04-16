namespace Azunt.Infrastructures.Auth;

public class TenantSchemaEnhancerEnsureDefaultUsers
{
    public static async Task RunAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<TenantSchemaEnhancerEnsureDefaultUsers>>();
        var config = services.GetRequiredService<IConfiguration>();
        var dbContext = services.GetRequiredService<ApplicationDbContext>();

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        // 1. AspNetUsers 테이블 사용자 수 확인
        int userCount = await dbContext.Users.CountAsync();
        if (userCount > 0)
        {
            logger.LogInformation("AspNetUsers 테이블에 사용자가 이미 존재합니다. 기본 사용자 및 역할 생성을 건너뜁니다.");
            return;
        }

        logger.LogInformation("AspNetUsers 테이블이 비어있습니다. 기본 역할과 사용자를 생성합니다.");

        // 2. 기본 Role 정의 및 생성
        string[] requiredRoles = {
            Dul.Roles.Administrators.ToString(),
            Dul.Roles.Everyone.ToString(),
            Dul.Roles.Users.ToString(),
            Dul.Roles.Guests.ToString()
        };

        foreach (var roleName in requiredRoles)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                var role = new ApplicationRole
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    Description = $"{roleName} 그룹"
                };
                await roleManager.CreateAsync(role);
                logger.LogInformation($"Role created: {roleName}");
            }
            else
            {
                logger.LogInformation($"Role already exists: {roleName}");
            }
        }

        // 3. appsettings.json 에서 기본 사용자 정보 읽기
        string adminEmail = config["DefaultUsers:AdministratorEmail"] ?? throw new InvalidOperationException("AdministratorEmail is not configured.");
        string adminPassword = config["DefaultUsers:AdministratorPassword"] ?? throw new InvalidOperationException("AdministratorPassword is not configured.");
        string guestEmail = config["DefaultUsers:GuestEmail"] ?? throw new InvalidOperationException("GuestEmail is not configured.");
        string guestPassword = config["DefaultUsers:GuestPassword"] ?? throw new InvalidOperationException("GuestPassword is not configured.");
        string anonymousEmail = config["DefaultUsers:AnonymousEmail"] ?? throw new InvalidOperationException("AnonymousEmail is not configured.");
        string anonymousPassword = config["DefaultUsers:AnonymousPassword"] ?? throw new InvalidOperationException("AnonymousPassword is not configured.");

        // 4. 기본 사용자 생성
        await CreateUserIfNotExists(userManager, logger, adminEmail, adminPassword,
            new[] { Dul.Roles.Administrators.ToString(), Dul.Roles.Users.ToString() }, emailConfirmed: true);

        await CreateUserIfNotExists(userManager, logger, guestEmail, guestPassword,
            new[] { Dul.Roles.Guests.ToString() });

        await CreateUserIfNotExists(userManager, logger, anonymousEmail, anonymousPassword,
            new[] { Dul.Roles.Guests.ToString() });
    }

    private static async Task CreateUserIfNotExists(UserManager<ApplicationUser> userManager, ILogger logger,
        string email, string password, string[] roles, bool emailConfirmed = false)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            logger.LogWarning($"User already exists: {email}");
            return;
        }

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = emailConfirmed
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            logger.LogInformation($"User created: {email}");
            foreach (var role in roles)
            {
                await userManager.AddToRoleAsync(user, role);
                logger.LogInformation($"Added {email} to role {role}");
            }
        }
        else
        {
            foreach (var error in result.Errors)
            {
                logger.LogError($"Error creating user {email}: {error.Description}");
            }
        }
    }
}
