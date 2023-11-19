# ASP.NET Core에서 인증과 권한 부여를 관리하는 'AuthorizeFilter' 사용하기

## AuthorizeFilter 필터 소개 

ASP.NET Core는 다양한 보안 기능을 제공하는데, 그 중 하나가 'AuthorizeFilter'입니다. 이 필터를 사용하면 애플리케이션의 인증과 권한 부여를 관리할 수 있습니다.

'AuthorizeFilter'는 컨트롤러나 액션 메서드에서 'Authorize' 특성을 사용하여 쉽게 적용할 수 있습니다. 예를 들어, 다음과 같은 코드를 사용하면 'AdminController' 클래스에서 'Admin' 역할을 가진 사용자만 액세스할 수 있도록 설정할 수 있습니다.

```csharp
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // ...
}
```

위 코드에서 'Authorize' 특성은 'AdminController' 클래스에서 'Admin' 역할을 가진 사용자만 액세스할 수 있도록 설정합니다. 이 경우 'AuthorizeFilter'는 요청한 사용자가 'Admin' 역할을 가지고 있는지 검사하고, 해당 역할을 가지고 있지 않은 경우 요청을 거부합니다.

'AuthorizeFilter'는 다양한 인증 및 권한 부여 방식을 지원합니다. 기본적으로 'AuthorizeFilter'는 쿠키 기반 인증을 사용하지만, JWT(JSON Web Token) 기반 인증 등 다른 인증 방식으로 구성할 수도 있습니다. 또한 'AuthorizeFilter'는 권한 부여 방식으로 'Role-based', 'Policy-based' 등 다양한 방식을 지원합니다.

'AuthorizeFilter'는 보안에 관련된 중요한 기능이므로 올바르게 구성되어야 합니다. 필요한 경우 'IAuthorizationService'를 사용하여 인증과 권한 부여를 수행할 수 있습니다.

ASP.NET Core에서 'AuthorizeFilter'를 사용하여 인증과 권한 부여를 관리하면 보안적으로 안전한 애플리케이션을 구현할 수 있습니다.

## AuthorizeFilter를 사용하여 전역적으로 MVC 또는 Razor Pages에 인증 설정

ASP.NET Core에서 전역적으로 MVC 또는 Razor Pages에 인증된 사용자만 접근하도록 설정하려면, "Startup.cs" 파일의 "ConfigureServices" 메서드에서 "services.AddAuthorization()" 메서드를 호출하여 권한 부여 서비스를 추가하고, "services.AddMvc()" 또는 "services.AddRazorPages()" 메서드 다음에 "services.AddMvc(options => options.Filters.Add(new AuthorizeFilter()));" 또는 "services.AddRazorPages(options => options.Conventions.AuthorizePage("/Index"));" 메서드를 호출하여 "AuthorizeFilter"를 추가하면 됩니다.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ...

    services.AddAuthorization(options =>
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    });

    services.AddMvc(options => options.Filters.Add(new AuthorizeFilter()));
    // 또는 services.AddRazorPages(options => options.Conventions.AuthorizePage("/Index"));
}
```

위 코드에서 "services.AddAuthorization()" 메서드는 권한 부여 서비스를 추가하고, "options.DefaultPolicy" 속성을 사용하여 인증된 사용자만 액세스할 수 있도록 설정합니다.

그리고 "services.AddMvc()" 또는 "services.AddRazorPages()" 메서드 다음에 "services.AddMvc(options => options.Filters.Add(new AuthorizeFilter()));" 또는 "services.AddRazorPages(options => options.Conventions.AuthorizePage("/Index"));" 메서드를 호출하여 "AuthorizeFilter"를 추가합니다. 이렇게 하면 전역적으로 모든 MVC 또는 Razor Pages에 대해 인증된 사용자만 액세스할 수 있게 됩니다.

위 코드에서 "services.AddMvc()" 또는 "services.AddRazorPages()" 메서드를 호출할 때 "options.Filters.Add(new AuthorizeFilter())"를 사용하여 "AuthorizeFilter"를 추가하면, 기본적으로 모든 액션 또는 페이지에 대해 인증이 필요하게 됩니다. 만약 특정 액션 또는 페이지에 대해서만 인증이 필요하다면, 해당 액션 또는 페이지에 대해 "AllowAnonymous" 특성을 사용하여 인증을 해제할 수 있습니다.

```csharp
[AllowAnonymous]
public IActionResult About()
{
    // ...
}
```

위 코드에서 "AllowAnonymous" 특성을 사용하여 "About" 액션에 대해 인증을 해제합니다.
