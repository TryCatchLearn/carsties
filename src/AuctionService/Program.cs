using AuctionService.Data;
using AuctionService.Errors;
using Contracts;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";
    });

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
        .DeclareExchange("auction-finished", ex => ex.ExchangeType = ExchangeType.Fanout)
        .DeclareExchange("bid-placed", ex => ex.ExchangeType = ExchangeType.Fanout)
        .BindExchange("auction-finished").ToQueue("auction-auction-finished")
        .BindExchange("bid-placed").ToQueue("auction-bid-placed")
        .AutoProvision();
    
    opts.PublishMessage<AuctionCreated>().ToRabbitExchange("auction-created");
    opts.PublishMessage<AuctionUpdated>().ToRabbitExchange("auction-updated");
    opts.PublishMessage<AuctionDeleted>().ToRabbitExchange("auction-deleted");
    
    opts.ListenToRabbitQueue("wolverine-dead-letter-queue");
    opts.ListenToRabbitQueue("auction-auction-finished");
    opts.ListenToRabbitQueue("auction-bid-placed");
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