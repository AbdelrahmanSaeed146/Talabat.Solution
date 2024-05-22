
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Extentions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;
using Talabat.Service;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Configure Services

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            builder.Services.AddSwaggerServices();

            builder.Services.AddApplicationServices();


            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefultConnection"));
            });

            builder.Services.AddDbContext<AppIdentityDbConext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(options =>
            {
                var Connections = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(Connections);
            });
      
            builder.Services.AddIdentityService(builder.Configuration);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", PolicyOptions =>
                {
                    PolicyOptions.AllowAnyHeader().AllowAnyMethod().WithOrigins(builder.Configuration["FrontBaseUrl"]);
                });
            });

            #endregion






            var app = builder.Build();

           using var Scope = app.Services.CreateScope();

            var services = Scope.ServiceProvider;

            var LoogerFactory = services.GetRequiredService<ILoggerFactory>();
            var _dbContext = services.GetRequiredService<StoreContext>();
            var IdentityDbContext = services.GetRequiredService<AppIdentityDbConext>();

            try
            {
                await _dbContext.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(_dbContext);


                await IdentityDbContext.Database.MigrateAsync();
                var UserManager = services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUserAsync(UserManager);



            }
            catch (Exception ex)
            {
                var looger = LoogerFactory.CreateLogger<Program>();

                looger.LogError(ex, "An Error Has been occured during Apply Migration");

            }




            #region Configure Kestrel Middlewares

            // Configure the HTTP request pipeline.

            app.UseMiddleware<ExceptionsMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddlewares();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}
