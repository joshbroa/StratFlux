using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StratFlux;
using StratFlux.Backtesting;
using StratFlux.Data;
using StratFlux.Models;
using StratFlux.Services;

var builder = WebApplication.CreateBuilder(args);

// This is the new database connection and setup with the new Strat User
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

// This gets the secret keys for the market data api
var alpacaApiKey = builder.Configuration.GetSection("AlpacaKeys")["ApiKey"];
var alpacaSecretKey = builder.Configuration.GetSection("AlpacaKeys")["SecretKey"];

// This creates a single instance of a class which can be used to retrieve required market data
// It is defined like this so it can be retrieved via dependency injection
builder.Services.AddSingleton(new AlpacaDataService(alpacaApiKey, alpacaSecretKey));

// This instantiates the database context that the application will use
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// This adds Identity framework services to the application and stores the identity data with the database context
builder.Services.AddIdentity<StratUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();

// Added custom paths for logging in and out
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.LogoutPath = "/Login/SignOutUser";
    options.AccessDeniedPath = "/Error";
});

builder.Services.AddControllersWithViews();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Backtesting notification service notifies users if their backtests have failed or completed while waiting for it to load
builder.Services.AddSignalR();
builder.Services.AddSingleton<BacktestsNotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<BacktestsHub>("/backtestsHub");
});

app.Run();
