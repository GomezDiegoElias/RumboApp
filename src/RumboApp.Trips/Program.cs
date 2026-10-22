using RumboApp.Trips.Clients;
using RumboApp.Trips.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<TripsDbContext>("tripsdb");

builder.Services.AddHttpClient<ILocationApi, LocationApi>(client =>
{
    client.BaseAddress = new Uri("https+http://location");
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

app.Run();