global using EcommerceBlazor.Shared;
global using System.Net.Http.Json;
global using Microsoft.AspNetCore.Components.Authorization;
using EcommerceBlazor.Client;
using EcommerceBlazor.Client.Services.CategoryService;
using EcommerceBlazor.Client.Services.ProductService;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using EcommerceBlazor.Client.Services.CartService;
using EcommerceBlazor.Client.Services.AuthService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
await builder.Build().RunAsync();
