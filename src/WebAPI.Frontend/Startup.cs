using WebAPI.Frontend.Extensions;

namespace WebAPI.Frontend;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        services.AddControllers();
        services.AddSwaggerGenConfig(Settings.SwaggerTitle, Settings.SwaggerVersion, string.Empty, true, xmlPath);

        services.AddFrontEndRabbitMQ(Settings.RabbitMQVirtualHost, Settings.QueueNameRequest);
    }

    public void Configure(WebApplication app)
    {
        IWebHostEnvironment env = app.Environment;

        if (env.IsDevelopment())
        {
            app.UseSwaggerUINoEmptyRoutePrefix(Settings.SwaggerTitle);
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}