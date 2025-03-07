
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middleware;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.IdentityStore;
using Talabat.Service;

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

            #region DataBases
            builder.Services.AddDbContext<StoreDbContext>(options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                });
            builder.Services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            builder.Services.AddTransient<IConnectionMultiplexer>(options =>
            {
                var connection = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(connection!);
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CORS", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"/*builder.Configuration["FrontBaseUrl"]!*/);
                });
            });
            #endregion

            builder.Services.AddApplicationService();
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

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:securityKey"]!))
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


            app.MapControllers();
            app.UseStaticFiles();
            app.UseCors("CORS");
            app.UseAuthentication();
            app.UseAuthorization();
            #endregion

            using var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider;
            var _loggerFactory = service.GetRequiredService<ILoggerFactory>();
            try
            {
                var _dbContext = service.GetRequiredService<StoreDbContext>();
                var _IdentityDbContext = service.GetRequiredService<IdentityDbContext>();
                var _userManager = service.GetRequiredService<UserManager<ApplicationUser>>();

                await _dbContext.Database.MigrateAsync();//update DB
                await _IdentityDbContext.Database.MigrateAsync();
                await StoreContextSeeding.SeedAsync(_dbContext);
                await IdentityDbContextSeed.SeedAsync(_userManager);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred when trying to apply the migration");
            }
            app.Run();
        }
    }
}
