using System.Net;
using AspNetCore.SignalR.OpenTelemetry;
using GrpcService;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TypedSignalR.Client.DevTools;
using WebApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpLogging(static options =>
{
    options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.Duration;
});

builder.Services.AddSignalR(options =>
{
    options.AddFilter<HubInstrumentationFilter>();
});

builder.Services
    .AddGrpcClient<Greeter.GreeterClient>(options =>
    {
        options.Address = new Uri("http://grpcservice:8080");
    });

builder.Services.AddSingleton<IMessageRepository, MessageRepository>();

var serviceName = "my-app-server";
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
            .AddSignalRInstrumentation()
            .AddRedisInstrumentation()
            .AddNpgsql()
            .AddOtlpExporter();
    });


builder.Services.AddNpgsqlDataSource("Host=postgres;Port=5432;Username=postgres;Password=postgres;");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseSignalRHubSpecification();
    app.UseSignalRHubDevelopmentUI();
}

app.UseHttpLogging();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ExampleHub>("/hubs/ExampleHub");
app.MapHub<ChatHub>("/hubs/ChatHub");

app.Run();
