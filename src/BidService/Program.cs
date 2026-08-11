using BidService.Data;
using BidService.Endpoints;
using BidService.Services;
using Contracts;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Wolverine;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<BidDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";
    });

var connString = builder.Configuration.GetConnectionString("BidDbConnection");
if (string.IsNullOrWhiteSpace(connString)) throw new ArgumentException("Connection string is empty");

builder.Host.UseWolverine(opts =>
{
    opts.PersistMessagesWithPostgresql(connString, "bids");
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.AlwaysMakeScheduledMessagesDurable();
    
    opts.UseRabbitMq(rabbit =>
        {
            rabbit.HostName = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
            rabbit.UserName = builder.Configuration["RabbitMQ:Username"] ?? "guest";
            rabbit.Password = builder.Configuration["RabbitMQ:Password"] ?? "guest";
        })
        .DeclareExchange("auction-created", ex => ex.ExchangeType = ExchangeType.Fanout)
        .DeclareExchange("bid-placed", ex => ex.ExchangeType = ExchangeType.Fanout)
        .DeclareExchange("auction-finished", ex => ex.ExchangeType = ExchangeType.Fanout)
        .BindExchange("auction-created").ToQueue("bid-auction-created")
        .AutoProvision();
    
    opts.ListenToRabbitQueue("bid-auction-created");
    opts.PublishMessage<BidPlaced>().ToRabbitExchange("bid-placed");
    opts.PublishMessage<AuctionFinished>().ToRabbitExchange("auction-finished");
});

builder.Services.AddScoped<IBidRepository, BidRepository>();
builder.Services.AddScoped<GrpcAuctionClient>();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapBidEndpoints();

try
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine($"Error initializing bid db: {e.Message}");
}

app.Run();