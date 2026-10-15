
using OrderFlow.ShipmentWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<OrderApprovedConsumer>();

var host = builder.Build();
host.Run();
