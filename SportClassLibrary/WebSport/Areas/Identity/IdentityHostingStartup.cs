using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebSport.Areas.Models;
using WebSport.Data;

[assembly: HostingStartup(typeof(WebSport.Areas.Identity.IdentityHostingStartup))]
namespace WebSport.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            //builder.ConfigureServices((context, services) => {
                //services.AddDbContext<WebSportContext>(options =>
                    //options.UseSqlServer(
                        //context.Configuration.GetConnectionString("WebSportContextConnection")));

                //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    //.AddEntityFrameworkStores<WebSportContext>();
            //});
        }
    }
}