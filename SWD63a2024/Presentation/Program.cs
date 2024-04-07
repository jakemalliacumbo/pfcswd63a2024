using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Presentation.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Presentation.Repositories;
using PdfSharp.Fonts;
using Presentation.Controllers;

namespace Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string pathToKeyFile = builder.Environment.ContentRootPath + "pfc-jmc-2024-27d299dd5030.json";


            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToKeyFile );


            // Add services to the container.
            /* var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
             builder.Services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlServer(connectionString));
             builder.Services.AddDatabaseDeveloperPageExceptionFilter();

             builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                 .AddEntityFrameworkStores<ApplicationDbContext>();
            */
            builder.Services.AddControllersWithViews();
           
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                });


            string project = builder.Configuration["project"];

            //these lines will register application services Or services built by the developer with the services collection
            //so the application knows about them when they will requested via constructor/method injection

            PostsRepository pr = new PostsRepository(project);

            builder.Services.AddScoped(x=>new BlogsRepository(project, pr));
            builder.Services.AddScoped(x => pr);

            //uniform bucket =swd63apfc2024ra
            //finegraned bucket = swd63apfc2024ra_fg
            builder.Services.AddScoped(x => new BucketRepository(project, "pfc-jmc-2024-fg"));
            builder.Services.AddScoped(x => new PubSubRepository("pfc-jmc-2024", project));
            builder.Services.AddScoped<IFontResolver, FileFontResolver>();

            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}