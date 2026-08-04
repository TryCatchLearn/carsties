using AuctionService.Data;
using AuctionService.Errors;
using Contracts;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connString)) throw new Exception("Connection string is empty");

builder.Services.AddDbContextWithWolverineIntegration<AuctionDbContext>(options =>
{
    options.UseNpgsql(connString);
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Host.UseWolverine(opts =>
{
    opts.PersistMessagesWithPostgresql(connString, "auctions_rmq");
    opts.UseEntityFrameworkCoreTransactions();
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    
    opts.UseRabbitMq(rabbit =>
        {
            rabbit.HostName = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
            rabbit.UserName = builder.Configuration["RabbitMQ:Usernam"] ?? "guest";
            rabbit.Password = builder.Configuration["RabbitMQ:Password"] ?? "guest";
        })
        .DeclareExchange("auction-created", ex => ex.ExchangeType = ExchangeType.Fanout)
        .DeclareExchange("auction-updated", ex => ex.ExchangeType = ExchangeType.Fanout)
        .DeclareExchange("auction-deleted", ex => ex.ExchangeType = ExchangeType.Fanout)
        .AutoProvision();
    
    opts.PublishMessage<AuctionCreated>().ToRabbitExchange("auction-created");
    opts.PublishMessage<AuctionUpdated>().ToRabbitExchange("auction-updated");
    opts.PublishMessage<AuctionDeleted>().ToRabbitExchange("auction-deleted");
    
    opts.ListenToRabbitQueue("wolverine-dead-letter-queue");
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseExceptionHandler();

app.MapControllers();

try
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine($"Error initializing database: {e.Message}");
}

app.Run();