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
        services.AddControllers();
        services.AddSwaggerGenConfig(Settings.SwaggerTitle, Settings.SwaggerVersion);

        var retryCount = Settings.RetryCount;
        var retryInterval = Settings.RetryInterval;

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
                    e.UseMessageRetry(r => r.Interval(retryCount, retryInterval));
                });
            }));

            x.AddRequestClient<PeopleListRequest>();
            //Add any additional consumers here
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