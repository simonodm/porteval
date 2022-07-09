using System.ComponentModel;
using FluentValidation.AspNetCore;
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
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Converters;
using PortEval.Application.Models.Validators;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure;

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
                    options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy(), false));
                })
                .AddFluentValidation(v => v.RegisterValidatorsFromAssemblyContaining<PortfolioDtoValidator>());

            services.AddLogging();

            services.ConfigureDbContext(Configuration);

            services.ConfigurePriceFetcher(Configuration);

            services.ConfigureDapper();

            services.AddRepositories();

            services.AddBackgroundJobs();

            services.AddServices();

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
                    Authorization = new [] { new HangfireDevAuthorizationFilter() }
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
            });

            AddTypeConverter<AggregationFrequency, AggregationFrequencyTypeConverter>();

            ScheduleBackgroundJobs();
        }

        private void AddTypeConverter<TType, TConverterType>()
        {
            TypeDescriptor.AddAttributes(typeof(TType), new TypeConverterAttribute(typeof(TConverterType)));
        }

        private void ScheduleBackgroundJobs()
        {
            RecurringJob.AddOrUpdate<ILatestPricesFetchJob>("latest_prices", job => job.Run(), "*/5 * * * *");
            RecurringJob.AddOrUpdate<ILatestExchangeRatesFetchJob>("latest_exchange_rates", job => job.Run(),
                Cron.Daily);
            RecurringJob.AddOrUpdate<IMissingInstrumentPricesFetchJob>("fetch_missing_prices", job => job.Run(), Cron.Daily);
            RecurringJob.AddOrUpdate<IMissingExchangeRatesFetchJob>("fetch_missing_exchange_rates",
                job => job.Run(), Cron.Daily);
            RecurringJob.AddOrUpdate<IInstrumentPriceCleanupJob>("db_cleanup", job => job.Run(), Cron.Daily);
            RecurringJob.AddOrUpdate<IImportCleanupJob>("import_cleanup", job => job.Run(), Cron.Daily);

            RecurringJob.Trigger("db_cleanup");
            RecurringJob.Trigger("fetch_missing_prices");
            RecurringJob.Trigger("fetch_missing_exchange_rates");
            RecurringJob.Trigger("import_cleanup");
        }
    }
}
