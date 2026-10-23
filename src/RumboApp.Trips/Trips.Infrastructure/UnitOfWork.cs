using Microsoft.EntityFrameworkCore;
using RumboApp.Trips.Domain.Ports;
using RumboApp.Trips.Infrastructure.Data;

namespace RumboApp.Trips.Infrastructure;

public sealed class UnitOfWork(TripsDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
