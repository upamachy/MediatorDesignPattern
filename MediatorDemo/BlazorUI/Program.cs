using BlazorUI.Data;
using BlazorUI.Services;
using DemoLibrary;
using DemoLibrary.DataAccess;
using MongoDB.Driver;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// Configure MongoDB
builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("MongoDbSettings:ConnectionString")));
builder.Services.AddSingleton<IMongoDatabase>(s =>
    s.GetRequiredService<IMongoClient>().GetDatabase(builder.Configuration.GetValue<string>("MongoDbSettings:DatabaseName")));
builder.Services.AddSingleton<IMongoDataAccess, MongoDataAccess>();

// Configure HTTP Client for API communication
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7075/");
});

// Add Authentication services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

// MediatR registration for version 12.5.0
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(DemoLibraryMediatREntryPoint).Assembly);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
