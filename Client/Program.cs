// Create a WebAssemblyHostBuilder with default configuration
var builder = WebAssemblyHostBuilder.CreateDefault(args);
// Add the root component "App" to the builder
builder.RootComponents.Add<App>("#app");
// Add the root component "HeadOutlet" to the builder
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiHostAddress = new Uri(builder.HostEnvironment.BaseAddress);  // builder.Configuration["API_Prefix"] ??
// Add a singleton service of type Model to the builder
builder.Services.AddSingleton(s => GetModel(apiHostAddress));
// Add MudBlazor services to the builder
builder.Services.AddMudServices();

// Build and run the WebAssembly host
await builder.Build().RunAsync();

/// <summary>
/// Fetches the model from the server and derives the tags from frequent terms used in the comments.
/// </summary>
/// <param name="baseAddress">The base address of the server.</param>
/// <returns>The fetched model.</returns>
async static Task<Model> GetModel(Uri baseAddress)
{
    // Fetch the JSON file from the server
    // see https://stackoverflow.com/questions/58523617/blazor-client-side-webassembly-reading-a-json-file-on-startup-cs

    using var httpClient = new HttpClient();
    //var url = new Uri(baseAddress, "/api/CommentDataFunction");
    var url = new Uri(baseAddress, "/Model/comments.json");
    Debug.WriteLine(url.AbsoluteUri);
    var model = await httpClient.GetFromJsonAsync<Model>(url)
        .ConfigureAwait(false);

    // derive the tags from frequent terms used in the comments
    foreach (var area in model.Areas)
    {
        area.Tags = area.Comments
            .SelectMany(c => c.Text.Split(" \t;.:,'?()[]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            .Where(w => w.Length > 4)
            .Select(w => w.ToLower())
            .GroupBy(w => w)
            .Select(g => new Tag { Text = g.Key, Count = g.Count() })
            .OrderByDescending(t => t.Count)
            .Take(15)
            .ToList();
    }
    return model;
}