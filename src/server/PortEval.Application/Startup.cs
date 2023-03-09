using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PortEval.Application.Extensions;
using PortEval.Application.Features.Hubs;
using PortEval.Application.Filters;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Converters;
using PortEval.Application.Models.Validators;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure;
using System.ComponentModel;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace PortEval.Application
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddSignalR()
                .AddNewtonsoftJsonProtocol(options =>
                {
                    options.PayloadSerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy(), false));
                });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new AggregationFrequencyJsonConverter());
                    options.SerializerSettings.Converters.Add(new TemplateTypeJsonConverter());
                    options.SerializerSettings.Converters.Add(new ImportStatusJsonConverter());
                    options.SerializerSettings.Converters.Add(new ColorJsonConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy(),
                        false));
                });

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<PortfolioDtoValidator>();
            services.AddLogging();
            services.AddDomainServices();
            services.AddDomainEventHandlers();
            services.ConfigureDbContext(Configuration);
            services.ConfigurePriceFetcher(Configuration);
            services.ConfigureDapper();
            services.AddRepositories();
            services.AddBackgroundJobs();
            services.AddServices(Configuration);
            services.AddCalculators();
            services.AddChartGenerators();
            services.AddQueries();

            services.ConfigureHangfire(Configuration);
            services.AddAutoMapper(typeof(Startup), typeof(PortEvalDbContext), typeof(PortfolioDto));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PortEvalAPI", Version = "v1" });
                c.EnableAnnotations();
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PortEvalAPI v1"));
                app.UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[] { new HangfireDevAuthorizationFilter() }
                });
            }

            app.UseCors();

            app.ConfigureExceptionMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notifications");
            });

            AddTypeConverter<AggregationFrequency, AggregationFrequencyTypeConverter>();
        }

        private void AddTypeConverter<TType, TConverterType>()
        {
            TypeDescriptor.AddAttributes(typeof(TType), new TypeConverterAttribute(typeof(TConverterType)));
        }
    }
}
