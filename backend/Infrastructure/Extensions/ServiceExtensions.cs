using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using backend.Infrastructure.Media;
using backend.Infrastructure.Services;
using backend.Infrastructure.Storage.Abstractions;
using backend.Infrastructure.Storage.Options;
using backend.Infrastructure.Storage.Providers;
using backend.Infrastructure.Storage.Services;

namespace backend.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssemblyContaining<Program>();
            services.Configure<StorageOptions>(configuration.GetSection("Storage"));
            services.Configure<S3Options>(configuration.GetSection("AWS"));

            services.AddSingleton<IStorageProviderResolver, StorageProviderResolver>();
            services.AddSingleton<StorageUrlResolver>();
            services.AddSingleton<IStorageService, StorageService>();
            services.AddSingleton<IImageValidator, ImageValidator>();
            services.AddSingleton<IUploadService, UploadService>();
            services.AddScoped<CsvImportService>();

            services.AddSingleton<IAmazonS3>(sp =>
            {
                var aws = sp.GetRequiredService<IOptions<S3Options>>().Value;
                if (string.IsNullOrWhiteSpace(aws.Region))
                    throw new InvalidOperationException("AWS:Region is required when Storage:Provider is s3.");
                if (string.IsNullOrWhiteSpace(aws.AccessKey) || string.IsNullOrWhiteSpace(aws.SecretKey))
                    throw new InvalidOperationException("AWS access keys are required when Storage:Provider is s3.");

                var credentials = new BasicAWSCredentials(aws.AccessKey, aws.SecretKey);
                var region = RegionEndpoint.GetBySystemName(aws.Region);
                return new AmazonS3Client(credentials, region);
            });

            services.AddSingleton<IStorageProvider, S3StorageProvider>();
            
            services.AddCors(opt =>
            {
                opt.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin()
                          .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                          .WithHeaders("Authorization", "Content-Type", "Accept", "Origin", "X-Requested-With"));
            });

            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("login", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("issue", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                });
            });

            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

            services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

            services.AddMemoryCache();

            return services;
        }
    }
}
