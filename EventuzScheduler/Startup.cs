using EventuzScheduler.Application.Dapper;
using EventuzScheduler.Application.Enums;
using EventuzScheduler.Application.Interfaces;
using EventuzScheduler.Infrastructure.Database;
using EventuzScheduler.Modules;
using EventuzScheduler.Services.Scheduler.Quartz;
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



            // Регистрация IDbConnection
            services.AddScoped<IDbConnection>(sp => new SqlConnection(Configuration.GetConnectionString("DefaultConnection")));

            // Регистрация репозитория
            //services.AddScoped<ISchedulerTaskInfoRepository, SchedulerTaskInfoRepository>();

            //var config = SchedulerBuilder.Create();
            //config.UsePersistentStore(store =>
            //{
            //    // it's generally recommended to stick with
            //    // string property keys and values when serializing
            //    store.UseProperties = true;

            //    store.UseSystemTextJsonSerializer();
            //});


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

            // Register your scheduler
            services.AddSingleton<ICustomScheduler, QuartzScheduler>();



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

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}



//using Microsoft.AspNetCore.Hosting;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();