using aidemo.Endpoints.Configuration;
using aidemo.Integrations.Configuration;
using aidemo.Models.Configuration;
using aidemo.Server.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.Services.AddOpenApiServices();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddDbContextAndRepositories();
builder.Services.AddValidators();
builder.Services.ConfigureJsonOptions();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseOpenApiReference();

app.UseUserInterface();

app.MapDefaultEndpoints();
app.MapAllEndpoints();

await app.RunAsync();