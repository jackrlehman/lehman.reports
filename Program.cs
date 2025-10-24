using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ReportBuilder;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<ReportBuilder.Components.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.Build().RunAsync();
