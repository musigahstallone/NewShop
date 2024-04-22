using Microsoft.Extensions.Diagnostics.HealthChecks;
using NewShop.Web.Components;
using NewBasket.GrpcBasket;
using NewShop.Web.Services.BasketServices;
using NewShop.Web.Services.CatalogServices;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpForwarderWithServiceDiscovery();

builder.Services.AddHttpServiceReference<CatalogServiceClient>("https+http://newcatalog", healthRelativePath: "health");

var isHttps = builder.Configuration["DOTNET_LAUNCH_PROFILE"] == "https";

builder.Services.AddSingleton<BasketServiceClient>()
    .AddGrpcServiceReference<Basket.BasketClient>($"{(isHttps ? "https" : "http")}://newbasket", failureStatus: HealthStatus.Degraded);

builder.Services.AddRazorComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseStaticFiles();

app.MapRazorComponents<App>();

app.MapForwarder("/catalog/images/{id}", "https+http://newcatalog", "/api/v1/catalog/items/{id}/image");

app.MapDefaultEndpoints();

app.Run();
