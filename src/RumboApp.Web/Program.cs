using RumboApp.Web.Clients;
using RumboApp.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient<IIdentityApi, IdentityApi>(c =>
    c.BaseAddress = new Uri("https+http://identity"));

builder.Services.AddHttpClient<ILocationApi, LocationApi>(c =>
    c.BaseAddress = new Uri("https+http://location"));

builder.Services.AddHttpClient<ITripsApi, TripsApi>(c =>
    c.BaseAddress = new Uri("https+http://trips"));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
