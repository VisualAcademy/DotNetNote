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

        // ITaskService�� DI �����̳ʿ� ���
        builder.Services.AddSingleton<ITaskService, TaskService>();

        // HttpClient ���
        builder.Services.AddHttpClient();

        // Azure Translator ���� ���ε�
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
            // ���⼭ todoContext ���
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
            // URL ���ۼ�(Rewriting) �̵���� ����ϱ�
            app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/"));

            // ����� ���� �̵���� ������ 
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Started.");
                await next(context);
                Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Finished.");
            });

            #region Back-End Web Development with .NET
            // TODO ����Ʈ�� ������ �޸� ����Ʈ
            var todos = new List<TodoRecord>();

            // ��ü TODO ��� ��������
            app.MapGet("/todos", () => todos);

            // Ư�� ID�� TODO �׸� ��������
            app.MapGet("/todos/{id}", Results<Ok<TodoRecord>, NotFound> (int id) =>
            {
                var targetTodo = todos.SingleOrDefault(x => x.Id == id);
                return targetTodo is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(targetTodo);
            });

            // ���ο� TODO �׸� �߰�
            app.MapPost("/todos", (TodoRecord task) =>
            {
                todos.Add(task);
                return TypedResults.Created($"/todos/{task.Id}", task);
            });

            // Ư�� ID�� TODO �׸� ������Ʈ
            app.MapPut("/todos/{id}", (int id, TodoRecord updatedTodo) =>
            {
                var index = todos.FindIndex(t => t.Id == id);
                if (index == -1)
                {
                    return Results.NotFound();
                }

                // ���ο� TodoRecord�� �����ϰ� ���� ����Ʈ�� �Ҵ�
                todos[index] = todos[index] with
                {
                    Name = updatedTodo.Name,
                    DueDate = updatedTodo.DueDate,
                    IsCompleted = updatedTodo.IsCompleted
                };

                return Results.Ok(todos[index]);
            });

            // Ư�� ID�� TODO �׸� ����
            app.MapDelete("/todos/{id}", (int id) =>
            {
                todos.RemoveAll(t => id == t.Id);
                return TypedResults.NoContent();
            });
            #endregion

            #region Endpoint Filter demo
            // ���ο� TODO �׸� �߰�
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
            // ��ü TODO ��� �������� (DI ���)
            app.MapGet("/di/todos", (ITaskService taskService) =>
            {
                return taskService.GetTodos();
            });

            // Ư�� ID�� TODO �׸� �������� (DI ���)
            app.MapGet("/di/todos/{id}", (int id, ITaskService taskService) =>
            {
                var todo = taskService.GetTodoById(id);
                return todo is null ? Results.NotFound() : Results.Ok(todo);
            });

            // ���ο� TODO �׸� �߰� (DI ���)
            app.MapPost("/di/todos", (TodoRecord task, ITaskService taskService) =>
            {
                var addedTask = taskService.AddTodo(task);
                return Results.Created($"/di/todos/{addedTask.Id}", addedTask);
            });

            // Ư�� ID�� TODO �׸� ���� (DI ���)
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
        // ASP.NET Core ���ø����̼ǿ��� ���� ��å�� �����մϴ�.
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Users", policy => policy.RequireRole("Users"));
            options.AddPolicy("Administrators", policy =>
                policy.RequireRole("Users").RequireClaim("UserId", siteAdminId));

            // "AdminOnly" ��å: "Administrators" ������ ���� ����ڸ� ���� ����
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrators"));

            // "ManagerOnly" ��å: "Managers" ������ ���� ����ڸ� ���� ����
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

        #region AspNetUsers ���̺� ���ο� �÷� �߰� 
        //var aspNetUsersTableAddColumn = new AspNetUsersTableEnhancer(Configuration.GetConnectionString("DefaultConnection"));
        //aspNetUsersTableAddColumn.AddShowInDropdownColumnIfNotExists(); 
        #endregion

        #region Ensure the columns exist in the AspNetUsers table.
        // Ensure the columns exist in the AspNetUsers table.
        var userTableEnhancer = new UserTableEnhancer(Configuration.GetConnectionString("DefaultConnection"));
        await userTableEnhancer.EnsureUserTableColumnsAsync();
        #endregion

        // ������ ���� �����̳� ���� ȣ��
        DependencyInjectionContainer(services, Configuration);

        // HttpClient ���
        // HttpClient �ν��Ͻ��� DI(Dependency Injection) �����̳ʿ� ����Ͽ� ���뼺�� ����
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







        #region �����ͺ��̽� �� ���� ��Ű�� �ʱ�ȭ
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
        #region ASP.NET Core ��Ű �������� ȸ�� ���� ��� �����ϱ�
        // 34.10. ASP.NET Core ��Ű �������� ȸ�� ���� ��� �����ϱ�
        services.AddSingleton<IUserRepository>(new UserRepository(Configuration.GetConnectionString("DefaultConnection")));
        #endregion

        services.AddTransient<ILoginFailedRepository, LoginFailedRepository>();
        services.AddTransient<ILoginFailedManager, LoginFailedManager>();
        services.AddTransient<IUserModelRepository, UserModelRepository>();

        services.AddSingleton<IBuyerRepository>(new BuyerRepository(Configuration["ConnectionStrings:DefaultConnection"]));

        // �÷��� ������ �����͸� ��-�޸� �Ǵ� DB���� �������� �ʰ��� �������丮 ����
        services.AddTransient<IVariableRepository, VariableRepositoryInMemory>();

        services.AddTransient<IIdeaRepository, IdeaRepository>();
        services.AddTransient<DotNetNote.Models.DataFinder>();

        // AddSingleton �޼���� ������ ���� ����ϱ�_DI ����� ���� �⺻ ���� �ܰ� ���캸��
        // AddSingletonDemoController.cs Ŭ�������� IInfoService �������̽� ���
        //[DI] InfoService Ŭ���� ������ ����
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

        // ���Ӽ� ����(������ ����, DI)�� ����Ͽ� �÷��� �÷��� ������ ������ ����ϱ�
        // ListOfCategoryController.cs Ŭ�������� ICategoryRepository �������̽� ���
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
        // Resource ��� ��� (AdoNet ��� ����)
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
