using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FLLJudge.Client;
using FLLJudge.Client.Pages;
using MudBlazor.Services;

// Create a WebAssemblyHostBuilder with default configuration
var builder = WebAssemblyHostBuilder.CreateDefault(args);
// Add the root component "App" to the builder
builder.RootComponents.Add<App>("#app");
// Add the root component "HeadOutlet" to the builder
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add a HttpClient service to the builder with the base address set to the API prefix or the host environment base address
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress) });
// Add a singleton service of type Model to the builder
builder.Services.AddSingleton(s => Model.GetModel(builder.HostEnvironment.BaseAddress));
// Add MudBlazor services to the builder
builder.Services.AddMudServices();

// Build and run the WebAssembly host
await builder.Build().RunAsync();
