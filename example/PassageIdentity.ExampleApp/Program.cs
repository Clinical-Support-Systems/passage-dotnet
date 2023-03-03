using System.Security.Claims;
using AspNet.Security.Identity.Passage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PassageIdentity.ExampleApp.Data;

namespace PassageIdentity.ExampleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.Development.json", true, true)
             .AddEnvironmentVariables();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(o =>
            {
                o.LoginPath = new PathString("/login");

                o.AccessDeniedPath = new PathString("/access-denied");

                o.Cookie = new CookieBuilder
                {
                    Name = ".AspNetCore.Passage"
                };
            })
            .AddPassage(options =>
            {
                options.AppId = builder.Configuration.GetValue<string>("Passage:AppId", string.Empty);
                options.ApiKey = builder.Configuration.GetValue<string>("Passage:ApiKey", string.Empty);
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.SaveTokens = true;
            });

            builder.Services.AddAuthorization();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
