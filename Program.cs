using ReportBuilder.Services;
using ReportBuilder.Components;

var builder = WebApplication.CreateBuilder(args);

// Configure to listen on localhost:5000 for Tauri
builder.WebHost.UseUrls("http://localhost:5000");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add toast notification service (scoped for each Blazor circuit)
builder.Services.AddScoped<ToastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Skip HTTPS redirection when running in Tauri
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
