
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Talabat.APIs.Errors;
using Talabat.APIs.Extentions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Repository;
using Talabat.Repository.Data;


namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Configure Services

            builder.Services.AddControllers();

            builder.Services.AddSwaggerServices();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefultConnection"));
            });

            //ApplicationServicesExtentions.AddApplicationServices(builder.Services);
            builder.Services.AddApplicationServices();

            #endregion

            var app = builder.Build();

           using var Scope = app.Services.CreateScope();

            var services = Scope.ServiceProvider;

            var _dbContext = services.GetRequiredService<StoreContext>();

            var LoogerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                await _dbContext.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(_dbContext);
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
            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}
