using ReportBuilder;
using ReportBuilder.Services;
using ReportBuilder.Components;
using ReportBuilder.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddDebug();
}

// Configure to listen on localhost:5000 for Tauri
builder.WebHost.UseUrls(AppConstants.LocalhostUrl);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register application services
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<IReportGenerator<MobileAppReportConfig>, MobileAppReportGenerator>();
builder.Services.AddScoped<PdfReportParser>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation(LogEvents.AppStartup, "Application starting on {Url}", AppConstants.LocalhostUrl);

app.Run();
