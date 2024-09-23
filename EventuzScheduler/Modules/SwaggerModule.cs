using Microsoft.OpenApi.Models;

namespace EventuzScheduler.Modules
{
    public static class SwaggerModule
    {
        public static IServiceCollection AddSwaggerModule(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Payberry.Eventuz.Scheduler.API",
                    Version = "v1",
                });

                c.UseInlineDefinitionsForEnums();

                List<string> xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
                xmlFiles.ForEach(xmlFile => c.IncludeXmlComments(xmlFile));
            });



            return services;
        }
    }
}
