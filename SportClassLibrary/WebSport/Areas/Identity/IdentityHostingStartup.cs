using System;
using DataIdentity.DataContext;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(WebSport.Areas.Identity.IdentityHostingStartup))]
namespace WebSport.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<WebSportContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("WebSportContextConnection")));

               services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<IdentityRole>().AddEntityFrameworkStores<WebSportContext>();

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
            });


        }
    }
}