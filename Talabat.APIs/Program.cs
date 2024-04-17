
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefultConnection"));
            });


            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //builder.Services.AddAutoMapper(M => M.AddProfile( new MappingProfiles()));
            builder.Services.AddAutoMapper(typeof(MappingProfiles));
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (ActionContext) =>
                {
                    var errors = ActionContext.ModelState.Where(p => p.Value.Errors.Count() > 0)
                                                         .SelectMany(p => p.Value.Errors)
                                                         .Select(e => e.ErrorMessage)
                                                         .ToArray();
                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);


                };
            });
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
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}
