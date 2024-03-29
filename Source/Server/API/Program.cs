using Autofac;
using Autofac.Extensions.DependencyInjection;
using BrassLoon.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using YardLight.CommonAPI;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer((ContainerBuilder builder) => builder.RegisterModule(new APIModule()));

            // Add services to the container.
            builder.Services.Configure<Settings>(builder.Configuration);
            
            builder.Services.AddLogging(b =>
            {
                Settings settings = new Settings();
                builder.Configuration.Bind(settings);
                if (settings.LogDomainId.HasValue && !string.IsNullOrEmpty(settings.BrassLoonLogApiBaseAddress) && settings.BrassLoonLogClientId.HasValue)
                {
                    b.AddBrassLoonLogger(c =>
                    {
                        Settings settings = new Settings();
                        builder.Configuration.Bind(settings);
                        c.LogApiBaseAddress = settings.BrassLoonLogRpcAddress;
                        c.LogDomainId = settings.LogDomainId.Value;
                        c.LogClientId = settings.BrassLoonLogClientId.Value;
                        c.LogClientSecret = settings.BrassLoonLogClientSecret;
                    });
                }
            });            
            
            builder.Services.AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
            })
            ;
            //builder.Services.AddCors(builder.Configuration);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Yard Light API"
                    });
                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                o.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                    },
                    new string[] { }
                }
                });
            });
            builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddAuthentication(builder.Configuration)
            .AddGoogleAuthentication(builder.Configuration);
            builder.Services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();
            builder.Services.AddAuthorization(builder.Configuration);

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            //app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.Run();
        }
    }
}