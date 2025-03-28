using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.NewtonsoftJson;
using ZiggyCreatures.Caching.Fusion;
using FusionCacheWebApi.Services;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 1. Add Redis first
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "MyApp_";
});

// 2. Build temporary provider
var tempProvider = builder.Services.BuildServiceProvider();

// 3. Configure FusionCache
builder.Services.AddFusionCache()
    .WithSerializer(new FusionCacheNewtonsoftJsonSerializer())
    .WithDistributedCache(builder.Services.BuildServiceProvider().GetRequiredService<IDistributedCache>())
    .WithBackplane(new RedisBackplane(new RedisBackplaneOptions
    {
        Configuration = builder.Configuration.GetConnectionString("Redis"),
        // Correct configuration options:
        ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
        {
            Password = builder.Configuration["Redis:Password"],
            AbortOnConnectFail = false
        }
    }))
    .WithDefaultEntryOptions(new FusionCacheEntryOptions
    {
        Duration = TimeSpan.FromMinutes(1),
        JitterMaxDuration = TimeSpan.FromSeconds(10)
    });


var redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"));
Console.WriteLine(redis.IsConnected); // Should be true



// Register our sample service
builder.Services.AddSingleton<IDataService, DataService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll"); // Apply CORS policy

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();
