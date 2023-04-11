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

        services.AddDbContext<DataDbContext>(option =>
        {
            option.UseInMemoryDatabase(Settings.DatabaseName);
        });

        services.AddDbContextGenericsMethods<DataDbContext>();
        services.AddTransient<IPeopleService, PeopleService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ConsumerPersonListRequest>();
            //Add any additional consumers here

            x.SetKebabCaseEndpointNameFormatter();
            x.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host(Settings.RabbitMQHost, Settings.RabbitMQVirtualHost, h =>
                {
                    h.Username(Settings.RabbitMQUsername);
                    h.Password(Settings.RabbitMQPassword);
                });

                rabbit.ReceiveEndpoint(Settings.QueueNameResponse, e =>
                {
                    e.Durable = Settings.Durable;
                    e.AutoDelete = Settings.AutoDelete;
                    e.ExchangeType = Settings.ExchangeType;

                    e.PrefetchCount = Settings.PrefetchCount;
                    e.ConfigureConsumer<ConsumerPersonListRequest>(context);
                    //Add any additional consumers here
                });
            });
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