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

// Blazored
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();

// MudBlazor
builder.Services.AddMudServices();

// HttpClient med vår custom AuthorizationMessageHandler
builder.Services.AddScoped(sp =>
{
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    var handler = new AuthorizationMessageHandler(localStorage)
    {
        InnerHandler = new HttpClientHandler()
    };

    return new HttpClient(handler)
    {
        BaseAddress = new Uri("https://localhost:7102/")
    };
});

// Applikationstjänster
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Autentisering
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
