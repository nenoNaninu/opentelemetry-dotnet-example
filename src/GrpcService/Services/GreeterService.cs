using System.Data.Common;
using Dapper;
using Grpc.Core;
using StackExchange.Redis;

namespace GrpcService.Services;
public class GreeterService : Greeter.GreeterBase
{
    private readonly DbDataSource _dbDataSource;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(DbDataSource dbDataSource, IConnectionMultiplexer connectionMultiplexer, ILogger<GreeterService> logger)
    {
        _dbDataSource = dbDataSource;
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger;
    }

    public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        await using var connection = await _dbDataSource.OpenConnectionAsync(context.CancellationToken);

        await connection.QueryAsync("select 1");

        await _connectionMultiplexer.GetDatabase(0)
            .StringSetAsync(Guid.NewGuid().ToByteArray(), Guid.NewGuid().ToByteArray());

        return new HelloReply
        {
            Message = "Hello " + request.Name
        };
    }
}

