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
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IArtpieceRepository, ArtpieceRepository>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<IPlaceRepository, PlaceRepository>();
            services.AddScoped<IProjectorRepository, ProjectorRepository>();
            services.AddScoped<IRentalAgreementRepository, RentalAgreementRepository>();
            services.AddScoped<IExpositionRepository, ExpositionRepository>();
            services.AddScoped<IArtistRepository, ArtistRepository>();
            services.AddScoped<IExhibitorRepository, ExhibitorRepository>();


            //services.AddMvc().AddNewtonsoftJson();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", builder =>
                {
                    builder.WithOrigins("http://localhost:8081", "http://localhost", "https://localhost:8081", "http://localhost:8082", "https://localhost:8082")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });

                options.AddPolicy("AllowAnyOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            /*services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });*/

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
