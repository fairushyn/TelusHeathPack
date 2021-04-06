using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace TelusHeathPack.Configurations
{
    public class SwaggerConfiguration
    {
        public static void ConfigureSwagger(IServiceCollection services)
        {
            // TODO services.AddSwaggerExamplesFromAssemblyOf<ModelExample>();
            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "Telus API", Version = "v1" });
                // TODO config.ExampleFilters();
            });
        }
    }
}