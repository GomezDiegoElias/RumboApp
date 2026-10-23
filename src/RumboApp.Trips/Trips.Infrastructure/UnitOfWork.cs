using Microsoft.EntityFrameworkCore;
using Npgsql;
using RumboApp.Trips.Domain;
using RumboApp.Trips.Domain.Ports;
using RumboApp.Trips.Infrastructure.Data;

namespace RumboApp.Trips.Infrastructure;

public sealed class UnitOfWork(TripsDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException postgresException
                && postgresException.SqlState == PostgresErrorCodes.ExclusionViolation)
        {
            throw new DriverNotAvailableException(
                "El chofer no está disponible para el intervalo solicitado.");
        }
    }
}
