using NewBasket.Models;
using NewBasket.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddRedisClient("redis");

builder.Services.AddSingleton<IBasketRepository, RedisBasketRepository>();


builder.Services.AddGrpc();
var app = builder.Build();
app.MapDefaultEndpoints();
// app.UseAuthorization();

app.MapGrpcService<BasketService>();
app.Run();

