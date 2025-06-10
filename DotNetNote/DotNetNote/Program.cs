using Azunt.Infrastructures;
using Azunt.Infrastructures.Tenants;
using Azunt.NoteManagement;
using Azunt.ResourceManagement;
using Azunt.Web.Components.Pages.Notes.Services;
using Azunt.Web.Infrastructures;
using Dalbodre;
using Dalbodre.Infrastructures.Cores;
using DotNetNote.Common;
using DotNetNote.Controllers.Articles;
using DotNetNote.Models.Buyers;
using DotNetNote.Models.Categories;
using DotNetNote.Models.Companies;
using DotNetNote.Models.Exams;
using DotNetNote.Models.Ideas;
using DotNetNote.Models.Notes;
using DotNetNote.Models.Notifications;
using DotNetNote.Models.RecruitManager;
using DotNetNote.Records;
using DotNetNote.Rules;
using DotNetNote.Services.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ConfigureServices
        builder.Services.AddFluentUIComponents();

        builder.Services.AddDbContext<DotNetNote.Components.TodoContext>(options =>
            options.UseInMemoryDatabase("TodoList"));
        await ConfigureServicesAsync(builder.Services, builder.Configuration);

        // ITaskService를 DI 컨테이너에 등록
        builder.Services.AddSingleton<ITaskService, TaskService>();

        // HttpClient 등록
        builder.Services.AddHttpClient();

        // Azure Translator 설정 바인딩
        builder.Services.Configure<AzureTranslatorSettings>(builder.Configuration.GetSection("AzureTranslator"));
        builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<AzureTranslatorSettings>>().Value);

        #region Note Module Test
        var defaultConnStr = builder.Configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("DefaultConnection is missing in configuration.");

        builder.Services.AddDependencyInjectionContainerForNoteApp(defaultConnStr, Azunt.Models.Enums.RepositoryMode.EfCore);
        builder.Services.AddTransient<NoteDbContextFactory>();
        //builder.Services.AddScoped<INoteStorageService, LocalNoteStorageService>(); 
        #endregion

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
        {
            // URL 재작성(Rewriting) 미들웨어 사용하기
            app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/"));

            // 사용자 정의 미들웨어 만들어보기 
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Started.");
                await next(context);
                Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Finished.");
            });

            #region Back-End Web Development with .NET
            // TODO 리스트를 저장할 메모리 리스트
            var todos = new List<TodoRecord>();

            // 전체 TODO 목록 가져오기
            app.MapGet("/todos", () => todos);

            // 특정 ID의 TODO 항목 가져오기
            app.MapGet("/todos/{id}", Results<Ok<TodoRecord>, NotFound> (int id) =>
            {
                var targetTodo = todos.SingleOrDefault(x => x.Id == id);
                return targetTodo is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(targetTodo);
            });

            // 새로운 TODO 항목 추가
            app.MapPost("/todos", (TodoRecord task) =>
            {
                todos.Add(task);
                return TypedResults.Created($"/todos/{task.Id}", task);
            });

            // 특정 ID의 TODO 항목 업데이트
            app.MapPut("/todos/{id}", (int id, TodoRecord updatedTodo) =>
            {
                var index = todos.FindIndex(t => t.Id == id);
                if (index == -1)
                {
                    return Results.NotFound();
                }

                // 새로운 TodoRecord를 생성하고 기존 리스트에 할당
                todos[index] = todos[index] with
                {
                    Name = updatedTodo.Name,
                    DueDate = updatedTodo.DueDate,
                    IsCompleted = updatedTodo.IsCompleted
                };

                return Results.Ok(todos[index]);
            });

            // 특정 ID의 TODO 항목 삭제
            app.MapDelete("/todos/{id}", (int id) =>
            {
                todos.RemoveAll(t => id == t.Id);
                return TypedResults.NoContent();
            });
            #endregion

            #region Endpoint Filter demo
            // 새로운 TODO 항목 추가
            app.MapPost("/todoswithvalidation", (TodoRecord task) =>
            {
                todos.Add(task);
                return TypedResults.Created($"/todos/{task.Id}", task);
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var taskArgument = context.GetArgument<TodoRecord>(0);
                var errors = new Dictionary<string, string[]>();
                if (taskArgument.DueDate < DateTime.UtcNow)
                {
                    errors.Add(nameof(TodoRecord.DueDate), ["Cannot have due date in the past."]);
                }
                if (taskArgument.IsCompleted)
                {
                    errors.Add(nameof(TodoRecord.IsCompleted), ["Cannot add completed todo."]);
                }

                if (errors.Count > 0)
                {
                    return Results.ValidationProblem(errors);
                }

                return await next(context);
            });
            #endregion
        }
        {
            #region Endpoint with DI
            // 전체 TODO 목록 가져오기 (DI 사용)
            app.MapGet("/di/todos", (ITaskService taskService) =>
            {
                return taskService.GetTodos();
            });

            // 특정 ID의 TODO 항목 가져오기 (DI 사용)
            app.MapGet("/di/todos/{id}", (int id, ITaskService taskService) =>
            {
                var todo = taskService.GetTodoById(id);
                return todo is null ? Results.NotFound() : Results.Ok(todo);
            });

            // 새로운 TODO 항목 추가 (DI 사용)
            app.MapPost("/di/todos", (TodoRecord task, ITaskService taskService) =>
            {
                var addedTask = taskService.AddTodo(task);
                return Results.Created($"/di/todos/{addedTask.Id}", addedTask);
            });

            // 특정 ID의 TODO 항목 삭제 (DI 사용)
            app.MapDelete("/di/todos/{id}", (int id, ITaskService taskService) =>
            {
                taskService.DeleteTodoById(id);
                return Results.NoContent();
            });
            #endregion
        }
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


        #region Read SiteAdmin from configuration
        var siteAdminId = Configuration["DotNetNoteSettings:SiteAdmin"]
            ?? throw new InvalidOperationException("Missing configuration: SiteAdmin");
        #endregion

        #region Authorization Policy Configuration
        // ASP.NET Core 애플리케이션에서 권한 정책을 정의합니다.
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Users", policy => policy.RequireRole("Users"));
            options.AddPolicy("Administrators", policy =>
                policy.RequireRole("Users").RequireClaim("UserId", siteAdminId));

            // "AdminOnly" 정책: "Administrators" 역할을 가진 사용자만 접근 가능
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrators"));

            // "ManagerOnly" 정책: "Managers" 역할을 가진 사용자만 접근 가능
            options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Managers"));
        });
        #endregion


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

        #region AspNetUsers 테이블에 새로운 컬럼 추가 
        //var aspNetUsersTableAddColumn = new AspNetUsersTableEnhancer(Configuration.GetConnectionString("DefaultConnection"));
        //aspNetUsersTableAddColumn.AddShowInDropdownColumnIfNotExists(); 
        #endregion

        #region Ensure the columns exist in the AspNetUsers table.
        // Ensure the columns exist in the AspNetUsers table.
        var userTableEnhancer = new UserTableEnhancer(Configuration.GetConnectionString("DefaultConnection"));
        await userTableEnhancer.EnsureUserTableColumnsAsync();
        #endregion

        // 의존성 주입 컨테이너 설정 호출
        DependencyInjectionContainer(services, Configuration);

        // HttpClient 등록
        // HttpClient 인스턴스를 DI(Dependency Injection) 컨테이너에 등록하여 재사용성을 높임
        services.AddHttpClient();
    }

    private static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseDefaultFiles();

        app.UseStaticFiles();
        //app.MapStaticAssets();

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







        #region 데이터베이스 및 인증 스키마 초기화
        var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
        bool initializeDatabase = config.GetValue<bool>("Database:InitializeOnStartup");

        if (initializeDatabase)
        {
            DatabaseInitializer.Initialize(app.ApplicationServices);
        }
        else
        {
            Console.WriteLine("Database initialization is skipped (Database:InitializeOnStartup = false)");
        }
        #endregion
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
        services.AddTransient<DotNetNote.Models.Notes.INoteRepository, DotNetNote.Models.Notes.NoteRepository>();
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


        #region ResourceManagement 
        // Resource 모듈 등록 (AdoNet 모드 선택)
        var defaultConnection = Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(defaultConnection))
            throw new InvalidOperationException("DefaultConnection is not configured in appsettings.json");
        services.AddDependencyInjectionContainerForResourceApp(defaultConnection, Azunt.Models.Enums.RepositoryMode.EfCore);
        services.AddTransient<ResourceAppDbContextFactory>();
        #endregion
    }
}

public partial class Program
{

}
