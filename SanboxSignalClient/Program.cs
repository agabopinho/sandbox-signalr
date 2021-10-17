using Microsoft.AspNetCore.SignalR.Client;
using SanboxSignalClient;

await using var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7182/my-hub")
    .WithAutomaticReconnect(new InfiniteRetry())
    .Build();

connection.Closed += Closed;
connection.Reconnected += Reconnected;
connection.Reconnecting += Reconnecting;

connection.On("other-call", () =>
{
    Console.WriteLine("other-call");
});

var started = false;

do
{
    Console.WriteLine("Try connecting...");

    try
    {
        await connection.StartAsync();
        started = true;

        Console.WriteLine("Connected...");
    }
    catch (Exception)
    {
        await Task.Delay(new Random(DateTime.Now.Millisecond).Next(0, 2500));
    }
} while (!started);

do
{
    if (connection.State != HubConnectionState.Connected)
    {
        await Task.Delay(new Random(DateTime.Now.Millisecond).Next(0, 2500));

        continue;
    }

    try
    {
        await foreach (var item in connection.StreamAsync<Data>("RandData"))
        {
            Console.WriteLine($"Stream - {item.Now} - {item.Rand}");
        }
    }
    catch (Exception)
    {
    }
}
while (true);

static Task Closed(Exception arg)
{
    Console.WriteLine("Connection closed...");

    return Task.CompletedTask;
}

static Task Reconnected(string arg)
{
    Console.WriteLine("Reconnected...");

    return Task.CompletedTask;
}

static Task Reconnecting(Exception arg)
{
    Console.WriteLine("Reconnecting...");

    return Task.CompletedTask;
}

public class InfiniteRetry : IRetryPolicy
{
    public TimeSpan? NextRetryDelay(RetryContext retryContext)
    {
        return TimeSpan.FromMilliseconds(new Random(DateTime.Now.Millisecond).Next(0, 2500));
    }
}
