using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Presentation.Repositories;
using PdfSharp.Fonts;
using Presentation.Controllers;
using Google.Cloud.SecretManager.V1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string pathToKeyFile = builder.Environment.ContentRootPath + "pfc-jmc-2024-48052f5bc0a0.json";


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
            
            string project = builder.Configuration["project"];

            string allKeys = AccessSecretVersion(project, "loginkeys", "2");
            object myJsonObject = JsonConvert.DeserializeObject(allKeys);

            JObject jsonObject = JObject.Parse(allKeys);

            string clientId = (string)jsonObject["Authentication:Google:ClientId"];
            string clientSecret = (string)jsonObject["Authentication:Google:ClientSecret"];
            string redisPassword = (string)jsonObject["redis_password"];

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddGoogle(options =>
                {
                    options.ClientId = clientId; 
                    options.ClientSecret = clientSecret ; 
                });

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });

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
            builder.Services.AddScoped<RedisRepository>(x => new RedisRepository(redisPassword));  

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

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }

        public static String AccessSecretVersion(string projectId = "my-project", string secretId = "my-secret", string secretVersionId = "123")
        {
            // Create the client.
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Build the resource name.
            SecretVersionName secretVersionName = new SecretVersionName(projectId, secretId, secretVersionId);

            // Call the API.
            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

            // Convert the payload to a string. Payloads are bytes by default.
            String payload = result.Payload.Data.ToStringUtf8();
            return payload;
        }

        private static void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                // TODO: Use your User Agent library of choice here.
                if(true)
                {
                    // For .NET Core < 3.1 set SameSite = (SameSiteMode)(-1)
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }
    }
}