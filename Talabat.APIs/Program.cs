
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middleware;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository;
using Talabat.Repository.Data;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            #region Configure Services
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            }); 
            builder.Services.AddScoped(typeof(IGenericRepository<>) , typeof(GenericRepository<>));
            //builder.Services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            //Validation Error handling
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                // Func(ActionContext , IActionResult) => typeof(InvalidModelStateResponseFactory)
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    //we need to get the errors from modelState and it's type is a dictionary of key value Pare
                    var errors = actionContext.ModelState.Where(P => P.Value!.Errors.Count() > 0)
                                                                .SelectMany(P => P.Value!.Errors)
                                                                .Select(E => E.ErrorMessage)
                                                                .ToList();
                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(response);    
                };
            });
            #endregion

            var app = builder.Build();

            #region Configure Kestrel Pipeline
            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStatusCodePagesWithReExecute("/errors/{0}");//just one request
            app.UseHttpsRedirection();
            //app.UseStatusCodePagesWithRedirects("/errors/{0}");//if the endpoint or unauthorize request happened it will generate redirect request 301 and go to the end point
            app.UseAuthorization();


            app.MapControllers();
            app.UseStaticFiles();
            #endregion

            using var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider;
            var _dbContext = service.GetRequiredService<StoreDbContext>();
            var _loggerFactory = service.GetRequiredService<ILoggerFactory>();
            try
            {
                await _dbContext.Database.MigrateAsync();//update DB
                await StoreContextSeeding.Seed(_dbContext);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occured when trying to apply the migration");
            }
            app.Run();
        }
    }
}
