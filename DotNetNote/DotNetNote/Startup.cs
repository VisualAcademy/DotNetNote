using DotNetNote.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetNote
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //services.AddSingleton<ICompanyRepository>(new CompanyRepositoryAdo(Configuration["ConnectionStrings:DefaultConnection"]));
            //services.AddSingleton<ICompanyRepository>(new CompanyRepositoryDapper(Configuration["ConnectionStrings:DefaultConnection"]));
            services.AddEntityFrameworkSqlServer().AddDbContext<CompanyContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<ICompanyRepository, CompanyRepositoryEntityFramework>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();

                endpoints.MapGet("/", async context =>
                {
                    //await context.Response.WriteAsync("Hello World!");
                    // 한글 출력
                    context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";
                    await context.Response.WriteAsync("안녕하세요.", System.Text.Encoding.UTF8);
                });
            });
        }
    }
}
