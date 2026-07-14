using GodotXR.Application.Mapper;
using GodotXR.Application.Services;
using GodotXR.Domain.IRepositories;
using GodotXR.Domain.IUnitOfWork;
using GodotXR.Infrastructure.Configurations;
using GodotXR.Infrastructure.Core;
using GodotXR.Infrastructure.Data;
using GodotXR.Infrastructure.Mapper;
using GodotXR.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Minio;
using System.Text;

namespace GodotXR.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            DbFactory.RegisterContext(services, configuration);

            // Cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "GodotXR_";
            });

            // Object Storage
            services.Configure<StorageOptions>(
             configuration.GetSection(StorageOptions.SectionName));

            var options =
                configuration
                    .GetSection(StorageOptions.SectionName)
                    .Get<StorageOptions>()!;

            services.AddSingleton<IMinioClient>(_ =>
            {
                return new MinioClient()
                    .WithEndpoint(options.Endpoint)
                    .WithCredentials(
                        options.AccessKey,
                        options.SecretKey)
                    .Build();
            });

            services.AddScoped<IStorageService, MinIOService>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IProgramService, ProgramService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<ISchoolYearService, SchoolYearService>();
            services.AddScoped<ISemesterService, SemesterService>();
            services.AddScoped<IClassroomService, ClassroomService>();
            services.AddScoped<IChildProfileService, ChildProfileService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IExerciseTypeService, ExerciseTypeService>();
            services.AddScoped<IExerciseService, ExerciseService>();
            services.AddScoped<IExerciseQuestionService, ExerciseQuestionService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IAnalyzeService, AnalyzeService>();
            services.AddScoped<IReportService, ReportService>();

            // AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(InfrastructureProfile).Assembly);
                cfg.AddMaps(typeof(ApplicationProfile).Assembly);
            });

            // Email
            services.Configure<EmailOptions>(
                configuration.GetSection("Email"));

            services.AddHttpClient<IMailService, BrevoEmailService>(client =>
            {
                client.BaseAddress = new Uri("https://api.brevo.com/");
            });

            // JWT
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? "SecretKeyMustBeAtLeast32CharactersLong");

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
    
}
