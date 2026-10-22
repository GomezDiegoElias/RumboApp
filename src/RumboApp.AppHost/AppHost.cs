var builder = DistributedApplication.CreateBuilder(args);

// Infraestructura
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var identityDb = postgres.AddDatabase("identitydb");
var locationDb = postgres.AddDatabase("locationdb");
var tripsDb    = postgres.AddDatabase("tripsdb");

// Microservicios
var identity = builder.AddProject<Projects.Identity_Api>("identity")
    .WithReference(identityDb)
    .WaitFor(identityDb);

var location = builder.AddProject<Projects.Location_Api>("location")
    .WithReference(locationDb)
    .WaitFor(locationDb);

var trips = builder.AddProject<Projects.Trips_Api>("trips")
    .WithReference(tripsDb)
    .WithReference(location)   // Trips consume Location
    .WithReference(identity)   // y validan contra Identity
    .WaitFor(tripsDb)
    .WaitFor(location);

// Frontend
builder.AddProject<Projects.RumboApp_Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(identity)
    .WithReference(location)
    .WithReference(trips)
    .WaitFor(trips);

builder.Build().Run();