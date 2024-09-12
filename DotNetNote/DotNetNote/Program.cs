using Microsoft.Extensions.DependencyInjection;
using Dalbodre.Infrastructures.Cores;
using Dalbodre;
using DotNetNote.Common;
using DotNetNote.Controllers.Articles;
using DotNetNote.Infrastructures.Tenants;
using DotNetNote.Models.Buyers;
using DotNetNote.Models.Companies;
using DotNetNote.Models.Exams;
using DotNetNote.Models.Ideas;
using DotNetNote.Models.Notes;
using DotNetNote.Models.Notifications;
using DotNetNote.Models.RecruitManager;
using DotNetNote.Rules;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.IdentityModel.Tokens;
using DotNetNote.Models.Categories;
using DotNetNote.Records;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ConfigureServices
        builder.Services.AddDbContext<DotNetNote.Components.TodoContext>(options =>
            options.UseInMemoryDatabase("TodoList"));
        await ConfigureServicesAsync(builder.Services, builder.Configuration);

        var app = builder.Build();

        // <TodoComponent>
        using (var scope = app.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var todo = scopedServices.GetRequiredService<DotNetNote.Components.TodoContext>();
            // 여기서 todoContext 사용
            if (todo != null)
            {
                todo.Todos.Add(new DotNetNote.Components.Todo { Id = -2, Title = "Angular", IsDone = false });
                todo.Todos.Add(new DotNetNote.Components.Todo { Id = -1, Title = "ASP.NET Core", IsDone = true });
                todo.SaveChanges();
            }
        }
        // </TodoComponent>

        // Configure
        Configure(app, app.Environment, app.Services);

        #region ASP.NET Core Web API with Minimal APIs 
        var todos = new List<TodoRecord>();

        app.MapGet("/todos", () => todos); 

        app.MapGet("/todos/{id}", Results<Ok<TodoRecord>, NotFound> (int id) =>
        {
            var targetTodo = todos.SingleOrDefault(x => x.Id == id);
            return targetTodo is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(targetTodo);
        });

        app.MapPost("/todos", (TodoRecord task) =>
        {
            todos.Add(task);
            return TypedResults.Created("/todos/{id}", task);
        }); 
        #endregion

        app.Run();
    }

    private static async Task ConfigureServicesAsync(IServiceCollection services, IConfiguration Configuration)
    {
        //services.AddDbContext<DotNetNote.Components.TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
        services.AddHttpContextAccessor();
        services.AddControllersWithViews();
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddDbContext<CompanyContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        services.AddTransient<ICompanyRepository, CompanyRepositoryEntityFramework>();
        services.Configure<DotNetNoteSettings>(Configuration.GetSection("DotNetNoteSettings"));
        services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(30); });
        services.AddAuthentication("Cookies")
            .AddCookie(options =>
            {
                options.LoginPath = "/User/Login/";
                options.AccessDeniedPath = "/User/Forbidden/";
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SymmetricSecurityKey"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });
        services.AddTransient<ISignRepository, SignRepositoryInMemory>();
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Users", policy => policy.RequireRole("Users"));
            options.AddPolicy("Administrators", policy =>
                policy.RequireRole("Users").RequireClaim("UserId",
                    Configuration.GetSection("DotNetNoteSettings").GetSection("SiteAdmin").Value));
        });
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("https://localhost:3000");
            });
        });
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            options.User.RequireUniqueEmail = true;
        });
        services.AddDbContext<DotNetNoteContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        services.AddDbContext<TechContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        #region Tenants Table 생성 및 컬럼 추가 데모
        var tenantSchemaEnhancerCreateAndAlter = new TenantSchemaEnhancerCreateAndAlter(Configuration.GetConnectionString("DefaultConnection"));
        tenantSchemaEnhancerCreateAndAlter.EnsureSchema();
        #endregion

        #region Partners Table 생성 및 컬럼 추가 데모
        var tenantSchemaEnhancerCreatePartnersTable = new TenantSchemaEnhancerCreatePartnersTable(Configuration.GetConnectionString("DefaultConnection"));
        tenantSchemaEnhancerCreatePartnersTable.EnhanceAllTenantDatabases();
        #endregion

        #region Ensure the columns exist in the AspNetUsers table.
        // Ensure the columns exist in the AspNetUsers table.
        var userTableEnhancer = new UserTableEnhancer(Configuration.GetConnectionString("DefaultConnection"));
        await userTableEnhancer.EnsureUserTableColumnsAsync();
        #endregion

        // 의존성 주입 컨테이너 설정 호출
        DependencyInjectionContainer(services, Configuration);
    }

    private static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseRewriter(new RewriteOptions().Add(new RedirectAzureWebsitesRule()).AddRedirectToWwwPermanent());
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
            endpoints.MapBlazorHub();
        });
    }

    private static void DependencyInjectionContainer(IServiceCollection services, IConfiguration Configuration)
    {
        #region ASP.NET Core 쿠키 인증으로 회원 관리 기능 구현하기
        // 34.10. ASP.NET Core 쿠키 인증으로 회원 관리 기능 구현하기
        services.AddSingleton<IUserRepository>(new UserRepository(Configuration.GetConnectionString("DefaultConnection")));
        #endregion

        services.AddTransient<ILoginFailedRepository, LoginFailedRepository>();
        services.AddTransient<ILoginFailedManager, LoginFailedManager>();
        services.AddTransient<IUserModelRepository, UserModelRepository>();

        services.AddSingleton<IBuyerRepository>(new BuyerRepository(Configuration["ConnectionStrings:DefaultConnection"]));

        // 컬렉션 형태의 데이터를 인-메모리 또는 DB에서 가져오는 초간단 리포지토리 패턴
        services.AddTransient<IVariableRepository, VariableRepositoryInMemory>();

        services.AddTransient<IIdeaRepository, IdeaRepository>();
        services.AddTransient<DotNetNote.Models.DataFinder>();

        // AddSingleton 메서드로 의존성 주입 사용하기_DI 사용을 위한 기본 설정 단계 살펴보기
        // AddSingletonDemoController.cs 클래스에서 IInfoService 인터페이스 사용
        //[DI] InfoService 클래스 의존성 주입
        services.AddSingleton<InfoService>();
        services.AddSingleton<IInfoService, InfoService>();

        services.AddTransient<ICopyrightService, CopyrightService>();
        services.AddSingleton<CopyrightService>();
        services.AddTransient<GuidService>();
        services.AddTransient<IGuidService, GuidService>();
        services.AddTransient<ITransientGuidService, TransientGuidService>();
        services.AddScoped<IScopedGuidService, ScopedGuidService>();
        services.AddSingleton<ISingletonGuidService, SingletonGuidService>();
        services.AddTransient<ICommunityCampJoinMemberRepository, CommunityCampJoinMemberRepository>();
        services.AddTransient<INoteRepository, NoteRepository>();
        services.AddTransient<INoteCommentRepository, NoteCommentRepository>();
        services.AddTransient<MaximServiceRepository, MaximServiceRepository>();
        services.AddTransient<ITechRepositoryEf, TechRepositoryEf>();
        services.AddTransient<IAttendeeRepository, AttendeeRepository>();
        services.AddSingleton<AttendeeApp.Models.IAttendeeRepository>(new AttendeeApp.Models.AttendeeRepository(Configuration.GetConnectionString("DefaultConnection")));
        services.AddTransient<IRecruitSettingRepository, RecruitSettingRepository>();
        services.AddTransient<IRecruitRegistrationRepository, RecruitRegistrationRepository>();
        services.AddTransient<IOneRepository, OneRepository>();
        services.AddTransient<ITwoRepository, TwoRepository>();
        services.AddTransient<IThreeRepository, ThreeRepository>();
        services.AddTransient<IFourRepository, FourRepository>();
        services.AddTransient<IFiveRepository, FiveRepository>();
        services.AddTransient<IQuestionRepository, QuestionRepository>();
        services.AddTransient<IHeroRepository, HeroRepository>();
        services.AddTransient<ICharacterRepository, CharacterRepository>();
        services.AddTransient<IPointRepository, PointRepositoryInMemory>();
        services.AddTransient<IPointLogRepository, PointLogRepository>();

        // 종속성 주입(의존성 주입, DI)을 사용하여 컬렉션 컬렉션 형태의 데이터 출력하기
        // ListOfCategoryController.cs 클래스에서 ICategoryRepository 인터페이스 사용
        services.AddTransient<ICategoryRepository, CategoryRepositoryInMemory>();

        services.AddTransient<DotNetSale.Models.ICategoryRepository, DotNetSale.Models.CategoryRepositoryInMemory>();
        services.AddTransient<IGoodsRepository, GoodsRepository>();
        services.AddSingleton<ICompanyRepository>(new CompanyRepositoryAdo(Configuration["ConnectionStrings:DefaultConnection"]));
        services.AddTransient<IMyNotificationRepository>(
            serviceProvider => new MyNotificationRepository(Configuration.GetConnectionString("DefaultConnection"))
        );
        services.AddTransient<IUrlRepository, UrlRepository>();
        services.AddTransient<IBlogService, FileBlogService>();
    }
}


public partial class Program
{ 

}
