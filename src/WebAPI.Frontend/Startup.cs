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

        services.AddMassTransit(x =>
        {
            x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(c =>
            {
                c.QueueExpiration = TimeSpan.FromSeconds(Settings.QueueExpiration);
                c.Host(Settings.RabbitMQHost, Settings.RabbitMQVirtualHost, h =>
                {
                    h.Username(Settings.RabbitMQUsername);
                    h.Password(Settings.RabbitMQPassword);
                });

                c.ConfigureEndpoints(context);
                c.ReceiveEndpoint(Settings.QueueNameRequest, e =>
                {
                    e.Durable = Settings.Durable;
                    e.AutoDelete = Settings.AutoDelete;
                    e.ExchangeType = Settings.ExchangeType;
                    e.PrefetchCount = Settings.PrefetchCount;

                    e.UseMessageRetry(r => r.Interval(Settings.RetryCount, Settings.RetryInterval));
                });
            }));

            x.AddRequestClient<PeopleListRequest>();
            x.AddRequestClient<PersonRequest>();
        });
    }

    public void Configure(WebApplication app)
    {
        IWebHostEnvironment env = app.Environment;

        if (env.IsDevelopment())
        {
            app.AddUseSwaggerUI(Settings.SwaggerTitle);
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}