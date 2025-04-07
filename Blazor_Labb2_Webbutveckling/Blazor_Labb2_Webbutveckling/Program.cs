using Blazor_Labb2_Webbutveckling;
using Blazor_Labb2_Webbutveckling.Services;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Authorization;
using Blazor_Labb2_Webbutveckling.Components;
using Microsoft.JSInterop;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddInteractiveServerComponents();
builder.Services.AddScoped<TokenDelegatingHandler>();

builder.Services.AddHttpClient("AuthorizedClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7018/");
});




builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.Configure<CircuitOptions>(options => options.DetailedErrors = true);
builder.Services.AddHttpContextAccessor();
builder.Logging.AddDebug();
builder.Logging.AddConsole();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
