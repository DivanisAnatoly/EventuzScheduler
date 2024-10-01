using CrystalQuartz.AspNetCore;
using EventuzScheduler.Application.Dapper;
using EventuzScheduler.Application.Enums;
using EventuzScheduler.Application.Interfaces;
using EventuzScheduler.Infrastructure.Database;
using EventuzScheduler.Modules;
using EventuzScheduler.Services.Scheduler.Hangfire;
using EventuzScheduler.Services.Scheduler.Quartz;
using Hangfire;
using Microsoft.Data.SqlClient;
using Quartz;
using Quartz.AspNetCore;
using Quartz.Spi;
using System.Data;
using System.Text.Json.Serialization;

namespace EventuzScheduler
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddSwaggerModule();

            services.AddScoped<IDbConnection>(sp => new SqlConnection(Configuration.GetConnectionString("DefaultConnection")));

            var schedulerType = Configuration["Scheduler:Type"];

            if (schedulerType == "Hangfire")
            {
                services.AddSingleton<ICustomScheduler, HangfireScheduler>();

                services.AddHangfire(config =>
                {
                    config.UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"));
                    GlobalJobFilters.Filters.Add(new CanBePausedAttribute());
                });
                services.AddHangfireServer();
            }
            else if (schedulerType == "Quartz")
            {
                services.AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();

                    q.UsePersistentStore(s =>
                    {
                        s.UseProperties = true;
                        s.UseSqlServer(sqlServer =>
                        {
                            sqlServer.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
                            sqlServer.TablePrefix = "QRTZ_";
                        });
                        s.UseJsonSerializer();
                    });
                });
                services.AddQuartzServer(q => q.WaitForJobsToComplete = true);

                services.AddSingleton<ICustomScheduler, QuartzScheduler>();
            }



            services.AddHttpContextAccessor();
            services.AddHttpClient();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigin", builder => builder.AllowAnyOrigin());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(options => options.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseAuthorization();

            var schedulerType = Configuration["Scheduler:Type"];

            if (schedulerType == "Hangfire")
            {
                app.UseHangfireDashboard();
                app.UseHangfireServer();
            }
            else if (schedulerType == "Quartz")
            {
                var schedulerFactory = app.ApplicationServices.GetRequiredService<ISchedulerFactory>();
                var scheduler = schedulerFactory.GetScheduler().Result;

                if (scheduler != null && !scheduler.IsStarted)
                {
                    scheduler.Start().GetAwaiter().GetResult();
                }

                app.UseCrystalQuartz(() => scheduler);
            }




            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}