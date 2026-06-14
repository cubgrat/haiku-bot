using HaikuBot.Application;
using HaikuBot.Infrastructure;
using HaikuBot.Infrastructure.Persistence;
using HaikuBot.Telegram;

var builder = Host.CreateApplicationBuilder(args);

// Composition root: собираем порты и адаптеры гексагона.
builder.Services.AddApplication(builder.Configuration);     // use-cases (ядро приложения)
builder.Services.AddInfrastructure(builder.Configuration);  // driven-адаптеры: БД + нейросети
builder.Services.AddTelegram(builder.Configuration);        // driving-адаптер: Telegram

var host = builder.Build();

await host.InitializeDatabaseAsync();
await host.RunAsync();
