using Microsoft.EntityFrameworkCore;
using NewCatalog.Controllers;
using NewCatalog.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CatalogContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CatalogContext")
    ?? throw new InvalidOperationException("Connection string 'CatalogContext' not found.")));

builder.AddServiceDefaults();
//builder.AddNpgsqlDbContext<CatalogContext>("catalogdb");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler();
}

app.MapCatalogApi();
app.MapDefaultEndpoints();

app.Run();