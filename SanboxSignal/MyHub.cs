using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace SanboxSignal;

public class MyHub : Hub
{
    private readonly ILogger<MyHub> _logger;

    public MyHub(ILogger<MyHub> logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("Connected {@id}", Context.ConnectionId);

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {

        _logger.LogInformation("Disconnected {@id}", Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }

    [HubMethodName("RandData")]
    public async IAsyncEnumerable<Data> RandDataAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var rand = new Random();

        while (true)
        {
            yield return new Data
            {
                Now = DateTime.Now,
                Rand = rand.Next()
            };

            await Task.Delay(500, cancellationToken);

            await Clients.Caller.SendAsync("other-call", cancellationToken);

            await Task.Delay(2000, cancellationToken);
        }
    }
}
