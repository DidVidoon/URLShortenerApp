using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Models;

namespace Services
{
    public static class PresetDatabase
    {
        public static void Configurate(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<URLContext>(Options =>
                     Options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                     b => b.MigrationsAssembly(typeof(URLContext).Assembly.FullName)));
        }

        public static void Fill(IApplicationBuilder applicationBuilder)
        {
            using (var serviseScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviseScope.ServiceProvider.GetService<URLContext>();

                context.Database.EnsureCreated();

                if (!context.URLs.Any())
                {
                    context.URLs.AddRange(
                        new URL
                        {
                            Url = "https://www.google.com/",
                            UrlShort = "https://localhost:7267/a1b2c3",
                            Identifier = "a1b2c3"
                        });
                }

                context.SaveChanges();
            }
        }
    }
}
