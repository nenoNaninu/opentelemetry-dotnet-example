using System.Data.Common;
using Dapper;
using GrpcService;
using Microsoft.AspNetCore.SignalR;
using TypedSignalR.Client;

namespace WebApi.Hubs;

[Receiver]
public interface IExampleHubReceiver
{
    Task OnPostMessage(string message);
}

[Hub]
public interface IExampleHub
{
    Task<string> EnterRoom(string name);
    Task<string> PostMessage(string message);
}

public class ExampleHub : Hub<IExampleHubReceiver>, IExampleHub
{
    private readonly Greeter.GreeterClient _greeterClient;
    private readonly DbDataSource _dbDataSource;

    public ExampleHub(Greeter.GreeterClient greeterClient, DbDataSource dbDataSource)
    {
        _greeterClient = greeterClient;
        _dbDataSource = dbDataSource;
    }

    public async Task<string> EnterRoom(string name)
    {
        var res = await _greeterClient.SayHelloAsync(
            new HelloRequest { Name = name },
            cancellationToken: this.Context.ConnectionAborted
        );

        return res.Message;
    }

    public async Task<string> PostMessage(string message)
    {
        await using var connection = await _dbDataSource.OpenConnectionAsync(this.Context.ConnectionAborted);

        await connection.QueryAsync("select 1");

        return message;
    }
}






