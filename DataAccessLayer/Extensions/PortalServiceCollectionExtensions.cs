using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.Extensions.Configuration;
using DataAccessLayer.Repositories.Interfaces;
using DataAccessLayer.Repositories;

namespace DataAccessLayer.Extensions
{
    public static class PortalServiceCollectionExtensions
    {
        public static IServiceCollection AddPortalServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            /*services.AddScoped<IAssessmentRepository, AssessmentRepository>();
            services.AddScoped<IPlaceRepository, PlaceRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<INoteRepository, NoteRepository>();
            services.AddScoped<IPossibleDateRepository, PossibleDateRepository>();
            services.AddScoped<ICoordinateRepository, CoordinateRepository>();
            services.AddScoped<IPaymentIntentRepository, PaymentIntentRepository>();*/

            //services.AddMvc().AddNewtonsoftJson();

            /*services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", builder =>
                {
                    builder.WithOrigins("http://localhost:19006/", "http://another-example.com")
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });*/
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            //services.AddScoped<IAppSettings, AppSettings>();

            //serilogger
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
            services.AddControllers();

            return services;
        }
    }
}
