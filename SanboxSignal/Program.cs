using SanboxSignal;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapHub<MyHub>("my-hub");

app.Run();
