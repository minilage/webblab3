using Blazored.LocalStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using TheLoadingBean.Client;
using TheLoadingBean.Client.Auth;
using TheLoadingBean.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7102/") });

// Add MudBlazor services
builder.Services.AddMudServices();

// Add Blazored packages
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();

// Add authentication services
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();

// Add authorization
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
