using WebAPI.Backend.Extensions;

namespace WebAPI.Backend;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGenConfig(Settings.SwaggerTitle, Settings.SwaggerVersion);

        services.AddDbContext<DataDbContext>(option => { option.UseInMemoryDatabase(Settings.DatabaseName); });
        services.AddDbContextGenericsMethods<DataDbContext>();

        services.AddTransient<IPeopleService, PeopleService>();
        services.AddBackEndRabbitMQ();
    }

    public void Configure(WebApplication app)
    {
        IWebHostEnvironment env = app.Environment;

        if (env.IsDevelopment())
        {
            app.AddUseSwaggerUI(Settings.SwaggerTitle);
        }

        app.AddDataPeopleDemo();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}