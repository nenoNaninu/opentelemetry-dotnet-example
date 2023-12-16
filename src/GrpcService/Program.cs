using GrpcService.Services;
using Microsoft.AspNetCore.HttpLogging;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddHttpLogging(static options =>
{
    options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.Duration;
});

var serviceName = "grpc-service";
var instanceId = $"{Environment.MachineName}-{Guid.NewGuid()}";

builder.Logging
    .ClearProviders()
    .AddSimpleConsole(options =>
    {
        options.IncludeScopes = true;
    })
    .AddOpenTelemetry(options =>
    {
        options.IncludeScopes = true;
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;

        options.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: serviceName,
                serviceInstanceId: instanceId
            ));

        options.AddOtlpExporter();
    });

builder.Services.AddOpenTelemetry()
    .ConfigureResource(x =>
    {
        x.AddService(
            serviceName: serviceName,
            serviceInstanceId: instanceId
        );
    })
    .WithMetrics(providerBuilder =>
    {
        providerBuilder
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter();
    })
    .WithTracing(providerBuilder =>
    {
        providerBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddGrpcClientInstrumentation()
            .AddRedisInstrumentation()
            .AddNpgsql()
            .AddOtlpExporter();
    });

builder.Services.AddNpgsqlDataSource("Host=postgres;Port=5432;Username=postgres;Password=postgres;");
builder.Services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect("redis:6379"));

var app = builder.Build();

app.UseHttpLogging();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
