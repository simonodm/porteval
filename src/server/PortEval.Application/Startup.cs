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
using PortEval.Application.Models.DTOs.JsonConverters;
using PortEval.Application.Models.Validators;
using PortEval.BackgroundJobs.DatabaseCleanup;
using PortEval.BackgroundJobs.LatestPricesFetch;
using PortEval.BackgroundJobs.MissingPricesFetch;
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
                    options.SerializerSettings.Converters.Add(new ToDateRangeJsonConverter());
                    options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy(), false));
                    options.SerializerSettings.Converters.Add(new ColorJsonConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddFluentValidation(v => v.RegisterValidatorsFromAssemblyContaining<PortfolioDtoValidator>());

            services.ConfigureDbContext(Configuration);

            services.ConfigureDapper();

            services.ConfigureServices();

            services.ConfigureQueries();

            services.ConfigureHangfire(Configuration);

            services.ConfigurePriceFetcher(Configuration);

            services.AddLogging();

            services.AddAutoMapper(typeof(Startup), typeof(PortEvalDbContext), typeof(PortfolioDto));


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PortEvalAPI", Version = "v1" });
            });
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

            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ConfigureBackgroundJobs();
        }

        private void ConfigureBackgroundJobs()
        {
            RecurringJob.AddOrUpdate<LatestPricesFetchJob>("latest_prices", job => job.Run(), "*/5 * * * *");
            RecurringJob.AddOrUpdate<LatestExchangeRatesFetchJob>("latest_exchange_rates", job => job.Run(),
                Cron.Daily);
            RecurringJob.AddOrUpdate<MissingInstrumentPricesFetchJob>("fetch_missing_prices", job => job.Run(), Cron.Daily);
            RecurringJob.AddOrUpdate<MissingExchangeRatesFetchJob>("fetch_missing_exchange_rates",
                job => job.Run(), Cron.Daily);
            RecurringJob.AddOrUpdate<InstrumentPriceCleanupJob>("db_cleanup", job => job.Run(), Cron.Daily);

            RecurringJob.Trigger("db_cleanup");
            RecurringJob.Trigger("fetch_missing_prices");
            RecurringJob.Trigger("fetch_missing_exchange_rates");
        }
    }
}
