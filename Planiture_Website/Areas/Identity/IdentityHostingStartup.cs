using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Planiture_Website.Areas.Identity.IdentityHostingStartup))]
namespace Planiture_Website.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                //services.AddDbContext<Planiture_WebsiteContext>(options =>
                  //  options.UseSqlServer(
                    //    context.Configuration.GetConnectionString("AuthDbContextConnection")));

                //THE FOLLOWING WAS COMMENTED OUT BY KINGZWILL...THIS CODE IS NOT NECESSARY BECAUSE IT WAS ALREADY ADDED TO THE Startup.cs FILE
                //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                  //  .AddEntityFrameworkStores<Planiture_WebsiteContext>();
            });
        }
    }
}