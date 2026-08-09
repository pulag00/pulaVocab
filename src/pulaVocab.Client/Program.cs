using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using pulaVocab.Client;
using pulaVocab.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("http://localhost:5000/") });
builder.Services.AddScoped<IVocabularyApiClient, VocabularyApiClient>();
builder.Services.AddScoped<IPracticeApiClient, PracticeApiClient>();

await builder.Build().RunAsync();
