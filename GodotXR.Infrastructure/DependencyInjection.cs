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

            // In-Memory Distributed Cache (thay thế Redis hoàn toàn)
            services.AddDistributedMemoryCache();

            // Object Storage
            services.Configure<StorageOptions>(
             configuration.GetSection(StorageOptions.SectionName));

            var options =
                configuration
                    .GetSection(StorageOptions.SectionName)
                    .Get<StorageOptions>();

            if (options != null &&
                !string.IsNullOrWhiteSpace(options.Endpoint) &&
                !options.Endpoint.Contains("supabase.co", StringComparison.OrdinalIgnoreCase))
            {
                services.AddSingleton<IMinioClient>(_ =>
                {
                    var cleanEndpoint = options.Endpoint
                        .Replace("https://", string.Empty)
                        .Replace("http://", string.Empty)
                        .TrimEnd('/');

                    var client = new MinioClient()
                        .WithEndpoint(cleanEndpoint)
                        .WithCredentials(
                            options.AccessKey,
                            options.SecretKey);

                    if (options.UseSSL)
                    {
                        client = client.WithSSL();
                    }

                    .WithSSL(options.UseSSL)
                    return client.Build();
                });
            }

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
            services.AddScoped<IItemAssetService, ItemAssetService>();
            services.AddScoped<ILessonSlotService, LessonSlotService>();
            services.AddScoped<IChildSpeechAccuracyRepository, ChildSpeechAccuracyRepository>();
            services.AddScoped<IChildSpeechAccuracyService, ChildSpeechAccuracyService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IAnalyzeService, AnalyzeService>();

            // AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(InfrastructureProfile).Assembly);
                cfg.AddMaps(typeof(ApplicationProfile).Assembly);
            });

            // Email
            services.Configure<EmailOptions>(
                configuration.GetSection("Email"));

            services.AddScoped<IMailService, SmtpEmailService>();

            // JWT
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "SecretKeyMustBeAtLeast32CharactersLong");

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
