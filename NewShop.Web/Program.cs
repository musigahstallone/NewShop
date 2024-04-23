var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// builder.AddApplicationServices();

builder.Services.AddHttpForwarderWithServiceDiscovery();

// Application services
// builder.Services.AddScoped<BasketState>();
builder.Services.AddScoped<LogOutService>();
builder.Services.AddSingleton<OrderStatusNotificationService>();
builder.Services.AddSingleton<IProductImageUrlProvider, ProductImageUrlProvider>();

// HTTP and GRPC client registrations
builder.Services.AddHttpServiceReference<CatalogService>("https+http://newcatalog", healthRelativePath: "health");

var isHttps = builder.Configuration["DOTNET_LAUNCH_PROFILE"] == "https";
/*
builder.Services.AddSingleton<BasketService>()
    .AddGrpcServiceReference<Basket.BasketClient>($"{(isHttps ? "https" : "http")}://newbasket", failureStatus: HealthStatus.Degraded);*/

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
