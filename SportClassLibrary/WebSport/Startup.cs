using Domain.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Claims;
using WebSport.Models;

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
            // Envio de Email
            services.AddScoped<IEmailSender, EmailSender>();
            var emailConfig = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);

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

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IUser, AspNetUser>();

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
