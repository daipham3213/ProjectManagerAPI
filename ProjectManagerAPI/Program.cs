using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectManagerAPI.Core.Models;
using ProjectManagerAPI.Persistence;
using System;

namespace ProjectManagerAPI
{
    public class Program
    {
        public static async System.Threading.Tasks.Task Main(string[] args)
        {

            //run that when initializing some sample datas

            //var host = CreateHostBuilder(args).Build();
            //using var scope = host.Services.CreateScope();

            //var services = scope.ServiceProvider;

            //try
            //{
            //    var context = services.GetRequiredService<ProjectManagerDBContext>();
            //    var userManager = services.GetRequiredService<UserManager<User>>();
            //    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            //    await context.Database.MigrateAsync();
            //    await Seed.SeedData(userManager, roleManager);
            //}
            //catch (Exception ex)
            //{
            //    var logger = services.GetRequiredService<ILogger<Program>>();
            //    logger.LogError(ex, "An error occured during migraiton");
            //}

            //await host.RunAsync();

            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
