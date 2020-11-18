
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SportClassLibrary.Services;
using System;
using System.Security.Claims;
using WebSport.Areas.Models;
using WebSport.Data;

namespace WebSport
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<WebSportContext>(options =>
               options.UseSqlServer(
                   Configuration.GetConnectionString("WebSportContextConnection")));
            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<WebSportContext>();
            services.AddRazorPages();

            // Envio de Email
            services.AddScoped<IEmailSender, EmailSender>();
            var emailConfig = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);

            // Protecao contra ataque de força bruta,5 min para poder voltar a tentar utilizar uma nova tentativa para entrar
            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });

            // By default, Identity requires that passwords contain an uppercase character, lowercase character, a digit, and 
            // a non-alphanumeric character. Passwords must be at least six characters long.
            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });

            services.AddAuthentication()
              .AddGoogle(googleOptions =>
              {
                  googleOptions.ClientId = "47342804999-f6j2jqad4ntkpg3gtgdnk8q6e55l5ot7.apps.googleusercontent.com";
                  googleOptions.ClientSecret = "IeqPs8yqKTi6JVBpGjb2z41v";
                  googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "displayName");
                  googleOptions.ClaimActions.MapJsonSubKey(ClaimTypes.Surname, "Surname", "givenName");
                  googleOptions.ClaimActions.MapJsonKey(ClaimTypes.DateOfBirth, "DateOfBirth");
                  googleOptions.ClaimActions.MapJsonKey(ClaimTypes.PostalCode, "ZipCode");
                  googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "Email");
                  googleOptions.ClaimActions.MapJsonKey(ClaimTypes.MobilePhone, "MobilePhone");
                  googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Hash, "Password");
                  googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Locality, "Street");
                  googleOptions.ClaimActions.MapJsonKey(ClaimTypes.StateOrProvince, "City");
              });

            // Cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.Cookie.Name = "YourAppCookieName";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Identity/Account/Login";
                // ReturnUrlParameter requires 
                //using Microsoft.AspNetCore.Authentication.Cookies;
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
