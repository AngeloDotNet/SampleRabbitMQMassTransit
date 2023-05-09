namespace WebAPI.Frontend.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddFrontEndRabbitMQ(this IServiceCollection services)
    {
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

        return services;
    }
}